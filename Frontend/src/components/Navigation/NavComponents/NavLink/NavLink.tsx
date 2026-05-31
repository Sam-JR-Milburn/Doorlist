"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import styles from "./NavLink.module.css";

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
            className={`${styles.navLink} ${isActive ? styles.active : ""}`}
            aria-current={isActive ? "page" : undefined}
        >
            { children }
        </Link>
    );
};