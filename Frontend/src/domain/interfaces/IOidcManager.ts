export interface OidcTokenResponse {
    access_token: string;
    refresh_token: string;
    id_token?: string;
    expires_in: number;
    refresh_expires_in?: number;
    token_type: string;
}

export interface OidcProviderConfig {
    authorityUrl: string;
    clientId: string;
    redirectUri: string;
    postLogoutRedirectUri: string;
    scopes: string[]; // e.g. ["openid", "profile", "offline_access"]
}

/**
 * <b>IOidcManager</b> is a generic OIDC manager interface for implementing OIDC providers against.
 */
export interface IOidcManager {
    initialise(config: OidcProviderConfig): Promise<void>;

    buildAuthorisationUrl(): Promise<string>;

    handleCallbackExchange(code: string | null, incomingState: string | null): Promise<OidcTokenResponse>;

    getAccessToken(): Promise<string>;
    getRawAccessToken(): string | null;

    isAuthenticated(): Promise<boolean>;

    logout(): Promise<void>;
}