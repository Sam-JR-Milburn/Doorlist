import { NavLink } from "@/components/Navigation/NavComponents/NavLink/NavLink";

export const AuthenticatedNav = () => {
    return (
        <div className="authenticatedNav">
            <p>Authenticated</p>
            <NavLink href={"/dashboard"}>Dashboard</NavLink>
        </div>
    );
}