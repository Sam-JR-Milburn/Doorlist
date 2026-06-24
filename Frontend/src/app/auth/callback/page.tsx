"use client";

import { useEffect, useRef, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";

import { useIdentitySession } from "@/services/auth/IdentityServiceProvider";

export default function AuthCallbackPage() {
    const searchParams = useSearchParams();
    const router = useRouter();
    const { manager, syncAuth } = useIdentitySession();

    const exchangeAttempted = useRef(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (exchangeAttempted.current) { return; }
        exchangeAttempted.current = true;

        const code = searchParams.get("code");
        const incomingState = searchParams.get("state");

        const executeVerification = async () => {
            try {
                await manager.handleCallbackExchange(code, incomingState); // Handles logic, validation and token persistence

                const session = await syncAuth();
                if (session.isAuthenticated) {
                    router.push("/dashboard");
                } else {
                    throw new Error("Handshake succeeded but the session couldn't be authenticated locally");
                }
            } catch (err) {
                console.error(`Cryptographic token validation failed: ${err}`);
                setError((err as Error).message || "Cryptographic token validation failed");
            }
        }

        executeVerification();
    }, [searchParams, router, manager, syncAuth]);

    // Conditional render?
    if (error) {
        return (<div style={{color: "red"}}>Security Error</div>);
    } else {
        return (<div>Verifying Identity Credentials...</div>);
    }
}
