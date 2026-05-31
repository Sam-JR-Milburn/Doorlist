import styles from "./DashboardNav.module.css";

import { SvgIconLink } from "@/components/Utils/Media/SvgIconLink/SvgIconLink";
import CartIcon from "@/res/media/svg/icon-cart.svg";
import UserAccountIcon from "@/res/media/svg/icon-user-circle.svg"

import Link from "next/link";


export const DashboardNav = () => {
    return (
        <nav className={styles.DashboardNav}>
            <p>
                Fun Fact Of The Day
            </p>
            <p>Stuff Here</p>

            <SvgIconLink
                href={"/account"}
                icon={UserAccountIcon} size={"32px"}
                colour={"var(--color-text-main)"} hoverColour={"var(--color-site-primary-theme)"} activeColour={"var(--color-site-primary-theme)"}
                ariaLabel={"View Account"}
            />

            <SvgIconLink
                href={"/cart"}
                icon={CartIcon} size={"32px"}
                colour={"var(--color-text-main)"} hoverColour={"var(--color-site-primary-theme)"} activeColour={"var(--color-site-primary-theme)"}
                ariaLabel={"View Purchase Cart"}
            />
        </nav>
    );
}