import { IOidcManager, OidcProviderConfig, OidcTokenResponse } from "@/domain/interfaces/IOidcManager";
import { generateCodeChallenge, generateCodeVerifier } from "@/utils/auth/pkce";
import { jwtDecode, JwtPayload } from "jwt-decode";

/**
 * Keycloak Auth Manager
 * This class handles the authentication OIDC process for Keycloak specifically.
 */
export class KeycloakAuthManager implements IOidcManager {
    private config!: OidcProviderConfig;
    private isRefreshing = false;
    private refreshQueue: Array<(token: string) => void> = [];

    public initialise(config: OidcProviderConfig): Promise<void> {
        this.config = config;
        /* 1. OIDC Discovery Loop? '/.well-known/openid-configuration' */
        /* 2. Prepare Crypto primitives for DPoP? */
        /* 3. Instantiate the module synchronously by awaiting promise - ready gate? */
        return Promise.resolve();
    }

    private getTokenEndpoint(): string {
        return `${this.config.authorityUrl}/protocol/openid-connect/token`;
    }

    private getLoginEndpoint(): string {
        return `${this.config.authorityUrl}/protocol/openid-connect/auth`;
    }

    private getLogoutEndpoint(): string {
        return `${this.config.authorityUrl}/protocol/openid-connect/logout`;
    }

    private storeTokens(tokens: OidcTokenResponse): void {
        window.sessionStorage.setItem("doorlist_access_token", tokens.access_token);
        window.sessionStorage.setItem("doorlist_refresh_token", tokens.refresh_token);
        if (tokens.id_token) {
            window.sessionStorage.setItem("doorlist_id_token", tokens.id_token);
        }
    }

    // Return true if the token is going to expire in less than 30 seconds.
    private isTokenExpiredSoon(token: string): boolean {
        try {
            const decoded = jwtDecode<JwtPayload>(token);
            if (!decoded.exp) { return true; } // Assume expired if no expiry present

            const executionBufferInSeconds = 30;
            return (Date.now() / 1000) > (decoded.exp - executionBufferInSeconds);
        } catch (err) {
            console.warn("Unparseable or malformed JWT token: ", err);
            return true;
        }
    }

    public getRawAccessToken(): string | null {
        return window.sessionStorage.getItem("doorlist_access_token");
    }

    // Checks if you have a valid and current access token.
    public async isAuthenticated(): Promise<boolean> {
        const accessToken = this.getRawAccessToken();
        if (!accessToken) { return false; }

        return !this.isTokenExpiredSoon(accessToken);
    }

    // Build the login redirect URL for Keycloak
    public async buildAuthorisationUrl(): Promise<string> {
        // Generate PKCE secrets
        const verifier = generateCodeVerifier();
        const challenge = await generateCodeChallenge(verifier);

        // Persist the PKCE handshake proofs
        window.sessionStorage.setItem("doorlist_pkce_verifier", verifier);
        const state = crypto.randomUUID();
        window.sessionStorage.setItem("doorlist_csrf_nonce", state);

        // Construct the Keycloak endpoint URL
        const authorisationUrl = new URL(this.getLoginEndpoint());
        authorisationUrl.searchParams.append("response_type", "code");
        authorisationUrl.searchParams.append("client_id", this.config.clientId);
        authorisationUrl.searchParams.append("redirect_uri", this.config.redirectUri);
        authorisationUrl.searchParams.append("state", state);

        authorisationUrl.searchParams.append("scope", this.config.scopes.join(" "));

        authorisationUrl.searchParams.append("code_challenge", challenge);
        authorisationUrl.searchParams.append("code_challenge_method", "S256");

        return authorisationUrl.toString();
    }

    // Handle the PKCE callback for Keycloak
    public async handleCallbackExchange(code: string | null, incomingState: string | null): Promise<OidcTokenResponse> {
        const savedVerifier = window.sessionStorage.getItem("doorlist_pkce_verifier");
        const savedNonce = window.sessionStorage.getItem("doorlist_csrf_nonce");

        // Check that the PKCE verifier (auth state) hasn't been lost
        if (!code || !savedVerifier) {
            throw new Error("Security validation failed: authorisation context has been lost");
        }
        // Check the anti-CSRF token, and that it's untampered
        if (!incomingState || !savedNonce || incomingState !== savedNonce) {
            throw new Error("Security validation failed: anti-CSRF state token mismatch");
        }

        // Grab the access token from Keycloak
        const bodyParams = new URLSearchParams();
        bodyParams.append("grant_type", "authorization_code");
        bodyParams.append("client_id", this.config.clientId);
        bodyParams.append("code", code);
        bodyParams.append("redirect_uri", this.config.redirectUri);
        bodyParams.append("code_verifier", savedVerifier);

        const response = await fetch(this.getTokenEndpoint(), {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: bodyParams.toString(),
        });
        if (!response.ok) {
            throw new Error(`Keycloak rejected token handshake: ${response.status} (${response.statusText})`);
        }

        // Persist the tokens
        const tokens: OidcTokenResponse = await response.json();
        if (
            typeof tokens.access_token !== "string" ||
            typeof tokens.refresh_token !== "string" ||
            typeof tokens.expires_in !== "number" ||
            typeof tokens.token_type !== "string")
        {
            throw new Error("OIDC token response missing key token elements.");
        }
        this.storeTokens(tokens);

        // Remove PKCE proofs
        window.sessionStorage.removeItem("doorlist_pkce_verifier");
        window.sessionStorage.removeItem("doorlist_csrf_nonce");

        return tokens;
    }

    /**
     * Executes the RFC 6749 refresh_token POST to Keycloak
     * @param refreshToken requiring an existing session of course
     */
    private async executeRefreshTokenGrant(refreshToken: string | null): Promise<OidcTokenResponse> {
        if (!refreshToken) { throw new Error("Refresh token unavailable."); }

        const bodyParams = new URLSearchParams();
        bodyParams.append("grant_type", "refresh_token");
        bodyParams.append("client_id", this.config.clientId);
        bodyParams.append("refresh_token", refreshToken);

        const response = await fetch(this.getTokenEndpoint(), {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: bodyParams.toString(),
        });
        if (!response.ok) {
            throw new Error(`Keycloak rejected the refresh grant: ${response.status} (${response.statusText})`);
        }

        const tokens: OidcTokenResponse = await response.json();
        if (
            typeof tokens.access_token !== "string" ||
            typeof tokens.refresh_token !== "string" ||
            typeof tokens.expires_in !== "number" ||
            typeof tokens.token_type !== "string")
        {
            throw new Error("OIDC token response missing key token elements.");
        }
        this.storeTokens(tokens);
        return tokens;
    }

    /**
     * Manages the access token lifecycle - if expired, negotiates for a new one.
     */
    public async getAccessToken(): Promise<string> {
        const accessToken = window.sessionStorage.getItem("doorlist_access_token");
        const refreshToken = window.sessionStorage.getItem("doorlist_refresh_token");

        // Return or error
        if (!accessToken) {
            throw new Error("User session is unauthenticated");
        }
        if (!this.isTokenExpiredSoon(accessToken)) {
            return accessToken;
        }

        // Refresh: ensure that only one network request is made regardless of concurrent access
        if (this.isRefreshing) {
            return new Promise((resolve) => {
                this.refreshQueue.push((newToken) => resolve(newToken));
            });
        }
        this.isRefreshing = true;

        try {
            const tokenResponse: OidcTokenResponse = await this.executeRefreshTokenGrant(refreshToken);

            this.refreshQueue.forEach((callback) => callback(tokenResponse.access_token));
            this.refreshQueue = [];

            return tokenResponse.access_token;
        } catch (err) {
            await this.logout();
            throw err;
        } finally {
            this.isRefreshing = false;
        }
    }

    public async logout(): Promise<void> {
        const idToken = window.sessionStorage.getItem("doorlist_id_token") || "";

        window.sessionStorage.removeItem("doorlist_access_token");
        window.sessionStorage.removeItem("doorlist_refresh_token");
        window.sessionStorage.removeItem("doorlist_id_token");

        const logoutUrl = new URL(this.getLogoutEndpoint());
        logoutUrl.searchParams.set("client_id", this.config.clientId);
        logoutUrl.searchParams.set("post_logout_redirect_uri", `${window.location.origin}/`);

        if (idToken) {
            logoutUrl.searchParams.set("id_token_hint", idToken);
        }

        window.location.href = logoutUrl.toString();
    }
}