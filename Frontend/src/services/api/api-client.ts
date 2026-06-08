// API Domains
import { User } from "@/services/api/User";
// Import { OtherControllerDomain } from "@/app/api/OtherDomain";

/**
 * This allows for API functionality to be called in a self-describing way with the
 * Object Literal Namespace pattern.
 */
export const api = {
    User: User,
    // OtherDomain: OtherDomain
}