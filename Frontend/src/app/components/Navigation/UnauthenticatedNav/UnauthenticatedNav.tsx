export const UnauthenticatedNav = ({}) => {
    return (
        <div className="unauthenticatedNav">
            <p>Unauthenticated</p>
            <a href={"/login"}>Login</a>
        </div>
    );
}