
// Matching registerUserInternal
export interface FullUserRegistrationDto {
    email: string;
    firstName: string;
    lastName: string;
    password: string;
    dateOfBirth: string; // Enforced as "YYYY-MM-DD"
    profilePicture: File | null;
}

export interface UserRegistrationResponseDto {
    userId: string;
    firstName: string;
    lastName: string;
}