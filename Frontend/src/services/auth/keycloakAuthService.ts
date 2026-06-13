/**
 * Keycloak Auth Service
 *      Generate a valid PKCE handshake URL for the Doorlist Keycloak service
 *
 * https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow-with-pkce
 */

import { generateCodeVerifier, generateCodeChallenge } from '@/utils/auth/pkce';

interface RedirectUrlConfig {
    keycloakUrl?: string;
    realm?: string;
    clientId?: string;
    frontendUrl?: string;
}

/**
 * Build a valid URL for the PKCE handshake.
 * @param config - replacable config for testability reasons
 */
export const buildKeycloakAuthorizationUrl = async (config: RedirectUrlConfig = {}): Promise<string> => {
    const KEYCLOAK_URL = config.keycloakUrl || process.env.NEXT_PUBLIC_KEYCLOAK_URL || "https://localhost:8443";
    const REALM = config.realm || "doorlist";
    const CLIENT_ID = config.clientId || "doorlist-frontend";
    const FRONTEND_URL = config.frontendUrl || process.env.NEXT_PUBLIC_FRONTEND_URL || window.location.origin;
    const REDIRECT_URI = `${FRONTEND_URL}/auth/callback`;

    // Generate PKCE secrets
    const verifier = generateCodeVerifier();
    const challenge = await generateCodeChallenge(verifier);

    // Persist the PKCE handshake proofs
    window.sessionStorage.setItem("doorlist_pkce_verifier", verifier);
    const state = crypto.randomUUID();
    window.sessionStorage.setItem("doorlist_csrf_nonce", state);

    // Construct the Keycloak endpoint URL
    const authorisationUrl = new URL(`${KEYCLOAK_URL}/realms/${REALM}/protocol/openid-connect/auth`);
    authorisationUrl.searchParams.append("response_type", "code");
    authorisationUrl.searchParams.append("client_id", CLIENT_ID);
    authorisationUrl.searchParams.append("redirect_uri", REDIRECT_URI);
    authorisationUrl.searchParams.append("state", state);
    authorisationUrl.searchParams.append("scope", "openid profile email");

    authorisationUrl.searchParams.append("code_challenge", challenge);
    authorisationUrl.searchParams.append("code_challenge_method", "S256");

    return authorisationUrl.toString();
}



export interface TokenExchangeResult {
    accessToken: string;
    idToken: string;
    refreshToken: string;
}

/**
 * Yield the token details from the auth callback step.
 * @param code - the PKCE
 * @param incomingState - an anti-CSRF token carried by Keycloak from the first handshake steps
 */
export const handleAuthCallbackExchange = async (
    code: string | null,
    incomingState: string | null
): Promise<TokenExchangeResult> => {
    const savedVerifier = window.sessionStorage.getItem("doorlist_pkce_verifier");
    const savedNonce = window.sessionStorage.getItem("doorlist_csrf_nonce");

    // Check the PKCE verifier
    if (!code || !savedVerifier) {
        throw new Error("Security validation failed: authorisation context has been lost");
    }

    // Check against CSRF (equality)
    if (!incomingState || !savedNonce || incomingState !== savedNonce) {
        throw new Error("Security validation failed: anti-CSRF state token mismatch");
    }

    const KEYCLOAK_URL = process.env.NEXT_PUBLIC_KEYCLOAK_URL || "https://localhost:8443";
    const REALM = "doorlist";
    const CLIENT_ID = "doorlist-frontend";
    const FRONTEND_URL = process.env.NEXT_PUBLIC_FRONTEND_URL || window.location.origin;
    const REDIRECT_URI = `${FRONTEND_URL}/auth/callback`;

    // Assemble the RFC 6749 request and yield the access token.
    const bodyParams = new URLSearchParams();
    bodyParams.append("grant_type", "authorization_code");
    bodyParams.append("client_id", CLIENT_ID);
    bodyParams.append("code", code);
    bodyParams.append("redirect_uri", REDIRECT_URI);
    bodyParams.append("code_verifier", savedVerifier);

    const response = await fetch(`${KEYCLOAK_URL}/realms/${REALM}/protocol/openid-connect/token`, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: bodyParams.toString(),
    });

    if (!response.ok) {
        throw new Error(`Keycloak rejected token handshake: ${response.status} (${response.statusText})`);
    }

    const tokens = await response.json();
    if (!tokens.access_token || !tokens.id_token || !tokens.refresh_token) {
        throw new Error(`One or more tokens was not found`);
    }

    // Flush temporary handshake values out of memory
    window.sessionStorage.removeItem("doorlist_pkce_verifier");
    window.sessionStorage.removeItem("doorlist_csrf_nonce");

    return {
        accessToken: tokens.access_token,
        idToken: tokens.id_token,
        refreshToken: tokens.refresh_token,
    };
}