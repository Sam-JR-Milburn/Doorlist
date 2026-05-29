export interface ApiErrorDetail {
    field?: string;
    message: string;
}

export interface ApiResponse<T = void> {
    success: boolean;
    statusCode: number;
    data: T | null;
    error: {
        message: string;
        details?: ApiErrorDetail[];
    } | null;
    timestamp: string;
    requestId?: string; // Tracking IDs, if-and-when they're implemented
}

export class ClientHttpError extends Error {
    public readonly requestId: string | undefined;
    constructor(public statusCode: number, message: string, requestId?: string) {
        super(message);
        this.name = "ClientHttpError";
        this.requestId = requestId;
    }
}