"use client";

import { useEffect, useRef, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";

// ----





export default function AuthCallbackPage() {
    const searchParams = useSearchParams();
    const router = useRouter();
    const exchangeAttempted = useRef(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (exchangeAttempted.current) { return; } // Guard-clause against multiple handshakes
        exchangeAttempted.current = true;

        const code = searchParams.get("code");
        const incomingState = searchParams.get("state");
        const savedVerifier = window.sessionStorage.getItem("doorlist_pkce_verifier");
        const savedNonce = window.sessionStorage.getItem("doorlist_csrf_nonce");

        if (!code || !savedVerifier) {
            setError("Authorization context lost. Please log-in again.");
            return;
        }
        if (incomingState !== savedNonce) { // Anti-CSRF
            setError("Security validation failure. Please log-in again.");
            return;
        }

        // ----
        const exchangeCodeForTokens = async () => {
            const KEYCLOAK_URL = process.env.NEXT_PUBLIC_KEYCLOAK_URL || "https://localhost:8443";
            const REALM = "doorlist";
            const CLIENT_ID = "doorlist-frontend";
            const FRONTEND_URL = process.env.NEXT_PUBLIC_FRONTEND_URL || "";
            const REDIRECT_URI = `${FRONTEND_URL}/auth/callback`;

            // ====
            const bodyParams = new URLSearchParams();
            bodyParams.set("grant_type", "authorization_code");
            bodyParams.set("client_id", CLIENT_ID);
            bodyParams.set("code", code);
            bodyParams.set("redirect_uri", REDIRECT_URI);

            bodyParams.set("code_verifier", savedVerifier);

            try {

                const response = await fetch(
                    `${KEYCLOAK_URL}/realms/${REALM}/protocol/openid-connect/token`,
                    {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/x-www-form-urlencoded",
                        },
                        body: bodyParams.toString(),
                    }
                );

                if (!response.ok) {
                    throw new Error(`Token endpoint rejected handshake (${response.status}): ${response.statusText}`);
                }

                const tokens = await response.json();
                window.sessionStorage.setItem("doorlist_auth_token", tokens.access_token);

                // Clear the ephemeral handshake store
                window.sessionStorage.removeItem("doorlist_pkce_verifier");
                window.sessionStorage.removeItem("doorlist_csrf_nonce");

                router.push("/dashboard");
            } catch (err) {
                console.log(`Cryptographic token exchange failed: ${err}`);
                return;
            }
        };
        exchangeCodeForTokens(); // Call, redirect, or don't
    }, [searchParams, router]);

    // Render before redirect
    if (error) {
        return (
            <div>
                Error: {error}
            </div>
        );
    } else {
        return (
            <div>
                Verifying Doorlist identity security credentials...
            </div>
        );
    }
}
