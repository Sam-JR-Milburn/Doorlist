import React from "react";
import styles from "./RootLayout.module.css";

import { DynamicHeader } from "@/components/Navigation/DynamicHeader/DynamicHeader";

export const RootLayout = ({ children }: React.PropsWithChildren) => {

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