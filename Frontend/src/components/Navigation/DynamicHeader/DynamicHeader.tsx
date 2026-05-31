import React from "react";

import styles from "./DynamicHeader.module.css";
import { NavLink } from "@/components/Navigation/NavComponents/NavLink/NavLink";

export const DynamicHeader = () => {
    // Placeholder for auth libs
    const isAuthenticated = false;

    return (
        <header className={styles.doorlistHeader}>
            <div className={styles.doorlistLogo}>
                <NavLink href={"/"}><h2>Doorlist</h2></NavLink>
            </div>

            <div className={styles.centralNav}></div>

            {!isAuthenticated ?
                <div className={styles.authNavLinks}>
                    <NavLink href={"/login"}><h4>Login</h4></NavLink>
                    <NavLink href={"/register"}><h4>Register</h4></NavLink>
                </div> :
                <div className={styles.authNavLinks}>
                    <p>PLACEHOLDER</p>
                </div>
            }
        </header>
    );
}