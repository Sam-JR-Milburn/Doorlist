import React from "react";
import styles from "./RootLayout.module.css";

import { AuthenticatedNav } from "@/app/components/Navigation/AuthenticatedNav/AuthenticatedNav";
import { UnauthenticatedNav } from "@/app/components/Navigation/UnauthenticatedNav/UnauthenticatedNav";

interface LayoutProps {
    children: React.ReactNode;
    headerActions?: React.ReactNode;
}

export const RootLayout = ({ children, headerActions }: LayoutProps) => {
    // Check for authenticated status here
    // const { isAuthenticated, user, logout, login } = UseAuth();
    const isAuthenticated: boolean = true;
    const user = { firstName: "Sam", lastName: "M" };

    return (
        <div className={styles.rootLayoutContainer}>
            <header className={styles.doorlistHeader}>
                <nav aria-label="all-page navigation">
                    {headerActions}

                    {isAuthenticated ?
                        <AuthenticatedNav user={ user } /> :
                        <UnauthenticatedNav />
                    }
                </nav>
            </header>
            <main className={styles.mainContainer}>
                {children}
            </main>
        </div>
    );
}