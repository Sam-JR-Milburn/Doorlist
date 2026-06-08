"use client";

import React from "react";
import navStyles from "@/components/Navigation/NavComponents/NavComponents.module.css";
import actionLinkStyles from "@/components/Navigation/NavComponents/ActionLink/ActionLink.module.css";

/**
 * ActionLink is for nav elements suitable for onClick.
 */
interface ActionLinkProps {
    onClick: (event: React.MouseEvent<HTMLButtonElement>) => void;
    disabled?: boolean;
    children?: React.ReactNode;
    className?: string;
}

export const ActionLink = ({ onClick, disabled = false, children, className = "" }: ActionLinkProps) => {
    return (
        <button
            type={"button"}
            onClick={onClick}
            disabled={disabled}
            className={`${navStyles.navInteractable} ${actionLinkStyles.actionButton} ${className}`}>
            {children}
        </button>
    );
}