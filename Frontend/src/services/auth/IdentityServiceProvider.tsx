"use client";

import React, { createContext, useContext } from "react";
import { IIdentitySessionManager } from "@/domain/interfaces/IIdentitySessionManager";
import {getKeycloakIdentityService} from "@/services/auth/AuthProviders";

const IdentitySessionContext: React.Context<IIdentitySessionManager | null> = createContext<IIdentitySessionManager | null>(getKeycloakIdentityService());

export const IdentitySessionProvider = ({ children }: { children: React.ReactNode }) => {
    // Mount the Keycloak Auth Manager service exactly once on layout mount (lambda initialiser)
    return (
        <IdentitySessionContext value={getKeycloakIdentityService()}>
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
