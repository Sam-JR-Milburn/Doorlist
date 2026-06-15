"use client";

import React, { createContext, useContext, useEffect, useState } from "react";
import { IIdentitySessionManager } from "@/domain/interfaces/IIdentitySessionManager";
import { getKeycloakIdentityService } from "@/services/auth/AuthProviders";

interface IdentityContext {
    manager: IIdentitySessionManager;
    isAuthenticated: boolean;
    rawToken: string | null;
    syncAuth: () => Promise<{ isAuthenticated: boolean; accessToken: string | null; }>;
}

const IdentitySessionContext: React.Context<IdentityContext | null> = createContext<IdentityContext | null>(null);

export const IdentitySessionProvider = ({ children }: { children: React.ReactNode }) => {
    const manager = getKeycloakIdentityService();
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
    const [rawToken, setRawToken] = useState<string | null>(null);

    const syncAuth = async () => {
        if (typeof window === "undefined") { return { isAuthenticated: false, accessToken: null }; }
        const authStatus = await manager.isAuthenticated();
        const tokenStr = manager.getRawAccessToken();

        setIsAuthenticated(authStatus);
        setRawToken(tokenStr);
        return { isAuthenticated: authStatus, accessToken: tokenStr };
    }

    // Initial check
    useEffect(() => {
        // eslint-disable-next-line react-hooks/set-state-in-effect
        syncAuth();
    }, []);

    // Mount the Keycloak Auth Manager service exactly once on mount
    return (
        <IdentitySessionContext value={{ manager, isAuthenticated, rawToken, syncAuth }}>
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
