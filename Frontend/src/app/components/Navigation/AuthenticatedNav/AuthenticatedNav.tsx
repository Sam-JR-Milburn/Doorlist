
interface UserProfile {
    firstName: string,
    lastName: string,
}
interface UserObjectState {
    user: UserProfile
}

export const AuthenticatedNav = ({ user }: UserObjectState) => {
    return (
        <div className="authenticatedNav">
            <p>Authenticated</p>
            <p>Welcome {user.firstName} {user.lastName}</p>
        </div>
    );
}