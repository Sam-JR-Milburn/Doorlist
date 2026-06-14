export interface TokenExchangeResult {
    accessToken: string;
    refreshToken: string;
    idToken?: string;
}

/**
 * IAuthManager
 * For managing token lifecycle abstractly
 */
export interface IIdentitySessionManager {
    buildAuthorisationUrl(): Promise<string>;

    handleCallbackExchange(code: string | null, incomingState: string | null): Promise<string>;

    getAccessToken(): Promise<string>;
    getRawAccessToken(): string | null;

    isAuthenticated(): Promise<boolean>;

    logout(): Promise<void>;
}