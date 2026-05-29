import { NavLink } from "@/components/Navigation/NavComponents/NavLink/NavLink";

export const UnauthenticatedNav = ({}) => {
    return (
        <div className="unauthenticatedNav">
            <p>Unauthenticated</p>
            <NavLink href={"/login"}>Login</NavLink>
            <br />
            <NavLink href={"/register"}>Register</NavLink>
        </div>
    );
}