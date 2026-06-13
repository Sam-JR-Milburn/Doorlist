"use client";

import { useEffect, useRef, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";

import { handleAuthCallbackExchange } from "@/services/auth/keycloakAuthService";

export default function AuthCallbackPage() {
    const searchParams = useSearchParams();
    const router = useRouter();
    const exchangeAttempted = useRef(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (exchangeAttempted.current) { return; }
        exchangeAttempted.current = true;

        const code = searchParams.get("code");
        const incomingState = searchParams.get("state");

        const executeVerification = async () => {
            try {
                const tokens = await handleAuthCallbackExchange(code, incomingState);

                // TODO: Replace with an encapsulated and auto-refreshing module
                window.sessionStorage.setItem("doorlist_access_token", tokens.accessToken);

                console.log("Access Token: "+tokens.accessToken);
                console.log("ID Token: "+tokens.idToken);
                console.log("Refresh Token: "+tokens.refreshToken);

                router.push("/dashboard");
            } catch (err) {
                console.error(`Cryptographic token validation failed: ${err}`);
                setError((err as Error).message || "Cryptographic token validation failed");
            }
        }

        executeVerification();
    }, [searchParams, router]);

    // Conditional render?
    if (error) {
        return (<div style={{color: "red"}}>Security Error</div>);
    } else {
        return (<div>Verifying Identity Credentials...</div>);
    }
}
