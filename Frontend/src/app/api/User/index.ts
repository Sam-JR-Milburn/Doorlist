import { ApiResponse, ClientHttpError } from "@/app/api/types";
import {
    FullUserRegistrationDto, UserRegistrationResponseDto,
} from "@/app/api/User/types";

const BASE_API_URL = process.env.NEXT_PUBLIC_API_URL || ""; // Read from .env.*

export const User = {

    // Register a user with the Keycloak service
    registerUserInternal: async (payload: FullUserRegistrationDto): Promise<ApiResponse<UserRegistrationResponseDto>> => {
        let response: Response | undefined;
        try {
            const formData = new FormData();
            formData.append("email", payload.email);
            formData.append("firstName", payload.firstName);
            formData.append("lastName", payload.lastName);
            formData.append("dateOfBirth", payload.dateOfBirth);
            if (payload.profilePicture) {
                formData.append("profilePicture", payload.profilePicture);
            }
            response = await fetch(`${BASE_API_URL}/api/User/registerUserInternal`,
                {
                    method: "POST",
                    body: formData,
                });

            const contentType = response.headers.get("content-type");
            const isJson = contentType && contentType.includes("application/json");

            // If it wasn't created
            if (response.status !== 201) {
                if (isJson) {
                    const errorResponse = await response.json() as ApiResponse<never>;
                    throw new ClientHttpError(response.status, errorResponse.error?.message || "Registration failed", errorResponse.requestId);
                }
                const fallbackText = await response.text();
                throw new ClientHttpError(response.status, fallbackText);
            }

            if (!isJson) {
                throw new ClientHttpError(response.status, `Received 201 from the API but received an invalid content-type`);
            }

            const respData = await response.json() as ApiResponse<UserRegistrationResponseDto>;
            return {
                success: true,
                statusCode: response.status,
                data: respData.data,
                error: null,
                timestamp: respData.timestamp,
                requestId: respData.requestId,
            };
        } catch (error) {
            if (error instanceof ClientHttpError) {
                const clientApiError = error as ClientHttpError;
                return {
                    success: false,
                    statusCode: clientApiError.statusCode,
                    data: null,
                    error: { message: clientApiError.message },
                    timestamp: new Date().toISOString(),
                    requestId: clientApiError.requestId,
                }
            } else {
                return {
                    success: false,
                    statusCode: response?.status ?? 500,
                    data: null,
                    error: { message: "Couldn't register user" },
                    timestamp: new Date().toISOString(),
                    requestId: undefined,
                }
            }
        }
    },

    // HERE BE FUNCTIONS...
} as const;