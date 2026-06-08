import React from "react";
import { DashboardNav } from "@/components/Navigation/DashboardNav/DashboardNav";

export default function AuthenticatedLayout({ children, }: { children: React.ReactNode }) {
    return (
        <div>
            <DashboardNav />
            {children}
        </div>
    );
}