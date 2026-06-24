"use client";
import React, { useEffect, useState } from "react";

import styles from "./DynamicHeader.module.css";
import { NavLink } from "@/components/Navigation/NavComponents/NavLink/NavLink";
import { LoginRedirect } from "@/components/Auth/LoginRedirect/LoginRedirect";
import { useIdentitySession } from "@/services/auth/IdentityServiceProvider";
import { ActionLink } from "@/components/Navigation/NavComponents/ActionLink/ActionLink";

export const DynamicHeader = () => {
    const { isAuthenticated, isReady, manager } = useIdentitySession();

    const [isMounted, setIsMounted] = useState<boolean>(false);

    // Guard against SSR hydration mismatch
    useEffect(() => {
        // eslint-disable-next-line react-hooks/set-state-in-effect
        setIsMounted(true);
    }, []);

    if (!isMounted) {
        return (
            <header className={styles.doorlistHeader}>
                <div className={styles.doorlistLogo}>
                    <NavLink href={"/"}><h2>Doorlist</h2></NavLink>
                </div>
                <div className={styles.centralNav}></div>
                <div className={styles.authNavLinks}>
                    <h4>Login</h4>
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

            {!isAuthenticated ?
                <div className={styles.authNavLinks}>
                    <LoginRedirect disabled={!isReady} />
                    <NavLink href={"/register"}><h4>Register</h4></NavLink>
                </div> :
                <div className={styles.authNavLinks}>
                    {/*<span>Token: {authState.preview}</span> */}
                    <ActionLink
                        disabled={!isReady}
                        onClick={() => manager.logout()}>
                        <h4>Logout</h4>
                    </ActionLink>
                </div>
            }
        </header>
    );
}