"use client";
import React, { useEffect, useState } from "react";

import styles from "./DynamicHeader.module.css";
import { NavLink } from "@/components/Navigation/NavComponents/NavLink/NavLink";
import { LoginRedirect } from "@/components/Auth/LoginRedirect/LoginRedirect";
import { useIdentitySession } from "@/services/auth/IdentityServiceProvider";
import {ActionLink} from "@/components/Navigation/NavComponents/ActionLink/ActionLink";

export const DynamicHeader = () => {
    const identitySession = useIdentitySession();

    const [isMounted, setIsMounted] = useState<boolean>(false);
    const [authState, setAuthState] = useState<{ loggedIn: boolean; preview: string | null }>({
        loggedIn: false,
        preview: null,
    });

    // Grab auth state on component load
    useEffect(() => {
        // eslint-disable-next-line react-hooks/set-state-in-effect
        setIsMounted(true);

        const checkAuth = async () => {
            try {
                const authenticated = await identitySession.isAuthenticated();
                let tokenPreview = "";

                if (authenticated) {
                    const token = identitySession.getRawAccessToken();
                    if (token) {
                        tokenPreview = token.substring(0,10).concat("...");
                    }
                }

                setAuthState({
                    loggedIn: authenticated,
                    preview: tokenPreview,
                });
            } catch (err) {
                console.error("", err);
                setAuthState({
                    loggedIn: false,
                    preview: null,
                });
            }
        }

        checkAuth();
    }, [identitySession]);

    if (!isMounted) {
        return (
            <header className={styles.doorlistHeader}>
                <div className={styles.doorlistLogo}>
                    <NavLink href={"/"}><h2>Doorlist</h2></NavLink>
                </div>
                <div className={styles.centralNav}></div>
                <div className={styles.authNavLinks}>
                    <NavLink href={"/register"}><h4>Register</h4></NavLink>
                </div>
            </header>
        );
    }

    return (
        <header className={styles.doorlistHeader}>
            <div className={styles.doorlistLogo}>
                <NavLink href={"/"}><h2>Doorlist</h2></NavLink>
            </div>

            <div className={styles.centralNav}></div>

            {!authState.loggedIn ?
                <div className={styles.authNavLinks}>
                    <LoginRedirect />
                    <NavLink href={"/register"}><h4>Register</h4></NavLink>
                </div> :
                <div className={styles.authNavLinks}>
                    <span>Token: {authState.preview}</span>
                    <ActionLink onClick={() => identitySession.logout()}>Logout</ActionLink>
                </div>
            }
        </header>
    );
}