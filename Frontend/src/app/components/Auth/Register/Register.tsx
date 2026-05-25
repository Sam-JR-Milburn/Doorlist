"use client";

import React, { useState } from "react";

import { api } from "@/app/api/api-client";
import { FullUserRegistrationDto } from "@/app/api/User/types";


export const Register = () => {
    // Capture form
    const [email, setEmail] = useState("");
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [password, setPassword] = useState("");
    const [dateOfBirth, setDateOfBirth] = useState("");
    const [profilePicture, setProfilePicture] = useState<File | null>(null);

    // Reset the state before invoking.
    const [loading, setLoading] = useState(false);
    const [errorMessage, setErrorMessage] = useState("");
    const [successMessage, setSuccessMessage] = useState("");

    // ----
    const handleFileChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const fileList = event.target.files;
        if (fileList && fileList.length > 0) {
            setProfilePicture(fileList[0]);
        } else {
            setProfilePicture(null);
        }
    }

    // ----
    const handleSubmit = async (event: React.SubmitEvent<HTMLFormElement>) => {
        event.preventDefault();

        setLoading(true);
        setErrorMessage("");
        setSuccessMessage("");

        const payload: FullUserRegistrationDto = {
            email,
            firstName,
            lastName,
            password,
            dateOfBirth,
            profilePicture: profilePicture,
        }

        const response = await api.User.registerUserInternal(payload);
        setLoading(false);

        if (response.success) {
            setSuccessMessage("Successfully registered!");
            setEmail("");
            setFirstName("");
            setLastName("");
            setPassword("");
            setDateOfBirth("");
        } else {
            setErrorMessage(response.error?.message || "Failure to register user");
        }
    }

    return (
        <div>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="firstName">First name</label>
                    <input id="firstName" value={firstName} onChange={(e) => setFirstName(e.target.value)} required />
                </div>
                <div>
                    <label htmlFor="lastName">Last name</label>
                    <input id="lastName" value={lastName} onChange={(e) => setLastName(e.target.value)} required />
                </div>
                <div>
                    <label htmlFor="dateOfBirth">Date of birth</label>
                    <input id="dateOfBirth" type="date" value={dateOfBirth} onChange={(e) => setDateOfBirth(e.target.value)} required />
                </div>
                <div>
                    <label htmlFor="profilePicture">Upload a profile picture</label>
                    <input id="profilePicture" type="file" accept="image/png, image/jpeg, image/webp" onChange={handleFileChange} />
                </div>
                <div>
                    <label htmlFor="email">Email</label>
                    <input id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
                </div>
                <div>
                    <label htmlFor="password">Password</label>
                    <input id="password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
                </div>
                <button type="submit">
                    Register
                </button>
            </form>
        </div>
    );
}