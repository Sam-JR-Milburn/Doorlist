"use client";

import React, { useState } from "react";

import { api } from "@/api/api-client";
import { FullUserRegistrationDto } from "@/api/User/types";

import styles from "./RegisterUserPage.module.css";

export const RegisterUserPage = () => {
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

    // Cleanly grab the file
    const handleFileChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const fileList = event.target.files;
        if (fileList && fileList.length > 0) {
            setProfilePicture(fileList[0]);
        } else {
            setProfilePicture(null);
        }
    }

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
        <div className={styles.registerLayout}>
            <form onSubmit={handleSubmit} className={styles.registerUserForm}>
                <h3 className={styles.introText}>Register for Doorlist</h3>

                <div className={styles.registerUserRow}>
                    <label htmlFor="firstName">First Name</label>
                    <input id="firstName" value={firstName} onChange={(e) => setFirstName(e.target.value)} required />
                </div>
                <div className={styles.registerUserRow}>
                    <label htmlFor="lastName">Last Name</label>
                    <input id="lastName" value={lastName} onChange={(e) => setLastName(e.target.value)} required />
                </div>
                <div className={styles.registerUserRow}>
                    <label htmlFor="dateOfBirth">Date of Birth</label>
                    <input id="dateOfBirth" type="date" value={dateOfBirth} onChange={(e) => setDateOfBirth(e.target.value)} required />
                </div>
                <div className={styles.registerUserRow}>
                    <label htmlFor="profilePicture">Profile Picture</label>
                    <input id="profilePicture" type="file" accept="image/png, image/jpeg, image/webp" onChange={handleFileChange} />
                </div>
                <div className={styles.registerUserRow}>
                    <label htmlFor="email">Email</label>
                    <input id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
                </div>
                <div className={styles.registerUserRow}>
                    <label htmlFor="password">Password</label>
                    <input id="password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
                </div>
                <button type="submit" className={styles.registerButton}>
                    Register
                </button>
                {loading ?
                    <p> LOADING... </p>
                    :
                    <></>
                }
            </form>
        </div>
    );
}