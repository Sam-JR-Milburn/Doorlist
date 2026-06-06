"use client";

import React, { useState } from "react";
import { NavLink } from "@/components/Navigation/NavComponents/NavLink/NavLink";
import { generateCodeVerifier, generateCodeChallenge } from "@/utils/auth/pkce";

import styles from "./LoginRedirect.module.css";

// Understanding: https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow-with-pkce
export const LoginRedirect = () => {
    const [isRedirecting, setIsRedirecting] = useState<boolean>(false);

    const handleKeycloakLogin = async () => {
        setIsRedirecting(true);

        // Grab the params from env
        const KEYCLOAK_URL = process.env.NEXT_PUBLIC_KEYCLOAK_URL || "https://localhost:8443";
        const REALM = "doorlist";
        const CLIENT_ID = "doorlist-frontend";
        const FRONTEND_URL = process.env.NEXT_PUBLIC_FRONTEND_URL || window.location.origin;
        const REDIRECT_URI = `${FRONTEND_URL}/auth/callback`;

        // Implement PKCE flow, construct redirect URL
        try {
            const verifier = generateCodeVerifier();
            const challenge = await generateCodeChallenge(verifier);

            // Stash the verifier for the callback route to complete the handshake
            window.sessionStorage.setItem("doorlist_pkce_verifier", verifier);
            // Stash a random nonce to prevent CSRF.
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

            // Redirect
            window.location.href = authorisationUrl.toString();
        } catch (error) {
            console.error("Failed to assemble PKCE redirect URL for login");
            setIsRedirecting(false);
        }
    }


    // ADJUST ME... FIGURE OUT A BETTER METHODOLOGY HERE -
    // Don't want to repeat myself and reimplement the NavLink CSS and so on.
    // Needs to basically dynamically redirect here. In fact, it should be disabled if it can't construct the URL
    return (
        <div
            onClick={handleKeycloakLogin}
            className={`${styles.loginRedirectButton} ${isRedirecting ? styles.loginRedirectButtonDisabled : null}`}
            role="button"
            tabIndex={isRedirecting ? -1 : 0}
            aria-disabled={isRedirecting}
            onKeyDown={async (e) => {
                if (e.key === "Enter" || e.key === " ") {
                    e.preventDefault();
                    await handleKeycloakLogin();
                }
            }}
        >
            <h4>Login</h4>
        </div>
    );
}

