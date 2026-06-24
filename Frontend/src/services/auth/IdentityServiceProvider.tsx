"use client";

import React, { createContext, useContext, useEffect, useState, useCallback } from "react";
import { IOidcManager } from "@/domain/interfaces/IOidcManager";
import { getKeycloakIdentityService } from "@/services/auth/AuthProviders";

interface IdentityContext {
    manager: IOidcManager;
    isAuthenticated: boolean;
    isReady: boolean;
    syncAuth: () => Promise<{ isAuthenticated: boolean; isReady: boolean; }>;
}

const IdentitySessionContext: React.Context<IdentityContext | null> = createContext<IdentityContext | null>(null);
let trackedManagerRef: IOidcManager | null = null; // Once-loaded defensive reference

export const IdentitySessionProvider = ({ children }: { children: React.ReactNode }) => {
    const manager = getKeycloakIdentityService();
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
    const [isReady, setIsReady] = useState<boolean>(false);

    // Memoize the sync auth function via a callback
    const syncAuth = useCallback(async () => {
        if (typeof window === "undefined") { return { isAuthenticated: false, isReady: false }; }

        // Defensively check the memory address of the OIDC manager
        if (trackedManagerRef && trackedManagerRef !== manager) {
            console.error("Critical: The OIDC manager memory address has mutated between render frames", {
                original: trackedManagerRef, current: manager
            });
        }

        const authStatus = await manager.isAuthenticated();

        setIsAuthenticated(authStatus);
        setIsReady(true);
        return { isAuthenticated: authStatus, isReady: true };
    }, [manager]);

    // Initial check
    useEffect(() => {
        // Capture the manager reference on initial component mount - prevent infinite re-render
        if (trackedManagerRef === null && typeof window !== "undefined") {
            trackedManagerRef = manager;
        }

        // eslint-disable-next-line react-hooks/set-state-in-effect
        syncAuth();
    }, [syncAuth, manager]);

    // Mount the Keycloak Auth Manager service exactly once on mount
    return (
        <IdentitySessionContext value={{ manager, isAuthenticated, isReady, syncAuth }}>
            {children}
        </IdentitySessionContext>
    );
}

// Identity consumer hook
export const useIdentitySession = () => {
    const context = useContext(IdentitySessionContext);
    if (!context) {
        throw new Error("Identity session provider must be used within the IdentitySessionProvider wrapper component");
    }
    return context;
}
