import React from "react";
import styles from "./SvgIconWrapper.module.css";

interface SvgIconProps {
    icon: React.ComponentType<React.SVGProps<SVGSVGElement>>;
    size?: string;
    className?: string;

    colour?: string;
    hoverColour?: string;
    activeColour?: string;

    ariaLabel?: string;
}

export const SvgIconWrapper = ({
    icon: IconComponent,
    size = "32px",
    className = "",
    colour = "currentColor",
    hoverColour,
    activeColour,
    ariaLabel = "",
}: SvgIconProps) => {
    // Inject CSS vars from the arguments
    const styleHook = {
        "--icon-size": size,
        "--icon-colour": colour,
        "--icon-hover-colour": hoverColour || colour,
        "--icon-active-colour": activeColour || hoverColour || colour,
    } as React.CSSProperties;

    return (
        <span
            className={`${styles.wrapper} ${className}`}
            style={styleHook} aria-label={ariaLabel}>
            <IconComponent width={"100%"} height={"100%"} focusable={"false"} aria-hidden={"true"} />
        </span>
    );
}