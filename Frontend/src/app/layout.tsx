import React from "react";
import type { Metadata } from "next";

import "./normalize.css";
import "./globals.css";

import { RootLayout } from "@/components/Navigation/RootLayout/RootLayout";

export const metadata: Metadata = {
  title: "Doorlist",
  description: "Ticket sales and distribution... but fake!",
};

import { neueHaas } from "@/app/fonts";

export default function RootLayoutContainer({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className={neueHaas.className}>
    <body>
      <RootLayout>{children}</RootLayout>
    </body>
    </html>
  );
}
