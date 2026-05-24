
// Matching registerUserInternal
export interface FullUserRegistrationDto {
    email: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string; // Enforced as "YYYY-MM-DD"
    profilePicture: File | null;
}

export interface UserRegistrationResponseDto {
    id: string;
    firstName: string;
    lastName: string;
}