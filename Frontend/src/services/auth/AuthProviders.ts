import { IOidcManager } from "@/domain/interfaces/IOidcManager";
import { KeycloakAuthManager } from "./KeycloakAuthManager";

declare global {
    var __oidcIdentityInstance: IOidcManager | undefined;
}

export const getKeycloakIdentityService = (): IOidcManager => {
    if (typeof window === "undefined") {
        return {
            initialise: () => Promise.resolve(),
            isAuthenticated: () => Promise.resolve(false),
            getRawAccessToken: () => null,
            getAccessToken: () => Promise.resolve(""),
            buildAuthorisationUrl: () => Promise.resolve(""),
            handleCallbackExchange: () => Promise.reject(new Error("Server-side content execution blocked")),
            logout: () => Promise.resolve(),
        }; // Prevent crash on server execution
    }

    // Globally instantiate once
    if (!globalThis.__oidcIdentityInstance) {
        const manager: IOidcManager = new KeycloakAuthManager();

        manager.initialise({
            authorityUrl: `${process.env.NEXT_PUBLIC_KEYCLOAK_URL}/realms/doorlist`,
            clientId: "doorlist-frontend",
            redirectUri: `${process.env.NEXT_PUBLIC_FRONTEND_URL || "http://localhost:9080"}/auth/callback`,
            postLogoutRedirectUri: `${process.env.NEXT_PUBLIC_FRONTEND_URL || "http://localhost:9080"}/`,
            scopes: ["openid", "profile", "email"],
        });
        globalThis.__oidcIdentityInstance = manager;
    }

    return globalThis.__oidcIdentityInstance;
};
