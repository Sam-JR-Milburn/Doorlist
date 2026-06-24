import React from "react";
import styles from "./RootLayout.module.css";

import { DynamicHeader } from "@/components/Navigation/DynamicHeader/DynamicHeader";
import { IdentitySessionProvider } from "@/services/auth/IdentityServiceProvider";

export const RootLayout = ({ children }: React.PropsWithChildren) => {

    return (
        <div className={styles.rootLayoutContainer}>
            <IdentitySessionProvider>
                <DynamicHeader />

                <main className={styles.mainContainer}>
                    {children}
                </main>

                <footer></footer>
            </IdentitySessionProvider>
        </div>
    );
}