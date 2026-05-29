import React from "react";
import type { Metadata } from "next";

import "./normalize.css";
import "./globals.css";
import localFont from "next/font/local";

const neueHaas = localFont({
  src: [
    {
      path: "../../public/fonts/NeueHaasGroteskDisplay/NeueHaasDisplayXXThin.ttf",
      weight: "200",
      style: "normal"
    },
    {
      path: "../../public/fonts/NeueHaasGroteskDisplay/NeueHaasDisplayThin.ttf",
      weight: "500",
      style: "normal"
    },
    {
      path: "../../public/fonts/NeueHaasGroteskDisplay/NeueHaasDisplayMedium.ttf",
      weight: "900",
      style: "normal"
    }
  ],
  variable: "--font-neue-haas-grotesk"
});

import { RootLayout } from "@/components/Navigation/RootLayout/RootLayout";

export const metadata: Metadata = {
  title: "Doorlist",
  description: "Ticket sales and distribution... but fake!",
};

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
