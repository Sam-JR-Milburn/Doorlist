import React from "react";
import Link from "next/link";

import styles from "./SvgIconLink.module.css";
import { SvgIconWrapper } from "@/components/Utils/Media/SvgIconWrapper/SvgIconWrapper";

interface SvgIconLinkProps {
    href: string;
    icon: React.ComponentType<React.SVGProps<SVGSVGElement>>;
    size?: string;
    className?: string;

    colour?: string;
    hoverColour?: string;
    activeColour?: string;

    ariaLabel?: string;
}

/* Expose a clickable icon link */
export const SvgIconLink = ({
    href,
    icon,
    size = "32px",
    className,
    colour = "var(--color-text-main)",
    hoverColour = "var(--color-site-primary-theme)",
    activeColour = "var(--color-site-primary-theme)",
    ariaLabel = "",
}: SvgIconLinkProps) => {
    return (
        <Link
            href={href}
            className={`${styles.iconLink} ${className}`}
            aria-label={ariaLabel}
        >
            <SvgIconWrapper
                icon={icon}
                size={size}
                colour={colour}
                hoverColour={hoverColour}
                activeColour={activeColour}
            />
        </Link>
    );
}