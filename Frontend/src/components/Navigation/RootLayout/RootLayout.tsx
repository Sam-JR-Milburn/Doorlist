import React from "react";
import styles from "./RootLayout.module.css";

import { DynamicHeader } from "@/components/Navigation/DynamicHeader/DynamicHeader";

interface LayoutProps {
    children: React.ReactNode;
    headerActions?: React.ReactNode;
}

export const RootLayout = ({ children }: LayoutProps) => {

    return (
        <div className={styles.rootLayoutContainer}>
            <DynamicHeader />

            <main className={styles.mainContainer}>
                {children}
            </main>

            <footer></footer>
        </div>
    );
}