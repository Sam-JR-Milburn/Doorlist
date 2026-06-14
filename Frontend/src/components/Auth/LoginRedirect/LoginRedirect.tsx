"use client";

import React, { useState } from "react";
import { ActionLink } from "@/components/Navigation/NavComponents/ActionLink/ActionLink";

import {useIdentitySession} from "@/services/auth/IdentityServiceProvider";

export const LoginRedirect = () => {
    const [isRedirecting, setIsRedirecting] = useState<boolean>(false);
    const identitySession = useIdentitySession();

    const handleLoginAction = async () => {
        if (isRedirecting) { return; }
        setIsRedirecting(true);

        try {
            // Build URL, redirect.
            const authUrl = await identitySession.buildAuthorisationUrl();
            window.location.href = authUrl;
        } catch (err) {
            console.error(`Failed to assemble PKCE redirect URL for login: ${err}`);
            setIsRedirecting(false);
        }
    }

    return (
        <ActionLink onClick={handleLoginAction} disabled={isRedirecting}>
            <h4>Login</h4>
        </ActionLink>
    );
}

