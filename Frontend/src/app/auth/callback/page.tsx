"use client";

import { useEffect, useRef, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";

import {useIdentitySession} from "@/services/auth/IdentityServiceProvider";

export default function AuthCallbackPage() {
    const searchParams = useSearchParams();
    const router = useRouter();
    const identitySession = useIdentitySession();

    const exchangeAttempted = useRef(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (exchangeAttempted.current) { return; }
        exchangeAttempted.current = true;

        const code = searchParams.get("code");
        const incomingState = searchParams.get("state");

        const executeVerification = async () => {
            try {
                await identitySession.handleCallbackExchange(code, incomingState); // Handles logic, validation and token persistence

                router.push("/dashboard");
            } catch (err) {
                console.error(`Cryptographic token validation failed: ${err}`);
                setError((err as Error).message || "Cryptographic token validation failed");
            }
        }

        executeVerification();
    }, [searchParams, router, identitySession]);

    // Conditional render?
    if (error) {
        return (<div style={{color: "red"}}>Security Error</div>);
    } else {
        return (<div>Verifying Identity Credentials...</div>);
    }
}
