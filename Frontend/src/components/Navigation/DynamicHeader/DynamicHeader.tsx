import React from "react";

import styles from "./DynamicHeader.module.css";

import { AuthenticatedNav } from "../AuthenticatedNav/AuthenticatedNav";
import { UnauthenticatedNav } from "../UnauthenticatedNav/UnauthenticatedNav";
import { NavLink } from "@/components/Navigation/NavComponents/NavLink/NavLink";

export const DynamicHeader = () => {
    // Placeholder for auth libs
    const isAuthenticated = false;

    return (
        <header className={styles.doorlistHeader}>
            <NavLink href={"/"}><h1>Doorlist</h1></NavLink>

            {isAuthenticated ?
            <AuthenticatedNav /> :
            <UnauthenticatedNav />}
        </header>
    );
}