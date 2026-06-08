"use client";

import React from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import navStyles from "@/components/Navigation/NavComponents/NavComponents.module.css";

/**
 * NavLink is for direct links to pages, including externally.
 */
interface NavLinkProps {
    href: string;
    children: React.ReactNode;
}

export const NavLink = ({ href, children }: NavLinkProps) => {
    const pathname = usePathname();
    const isActive = pathname === href;

    return (
        <Link
            href={href}
            className={`${navStyles.navInteractable} ${isActive ? navStyles.navInteractableDisabled : ""}`}
            aria-current={isActive ? "page" : undefined}
        >
            { children }
        </Link>
    );
};