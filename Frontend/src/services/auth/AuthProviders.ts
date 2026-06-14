import { IIdentitySessionManager } from "@/domain/interfaces/IIdentitySessionManager";
import { KeycloakAuthManager } from "./KeycloakAuthManager";

let keycloakInstance: IIdentitySessionManager | null = null;

export const getKeycloakIdentityService = (): IIdentitySessionManager => {
    if (typeof window === "undefined") {
        return {} as IIdentitySessionManager; // Prevent crash on server execution
    }
    // Safe client-only return
    if (!keycloakInstance) {
        keycloakInstance = new KeycloakAuthManager();
    }
    return keycloakInstance;
};
