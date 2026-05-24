import type { Metadata } from "next";
import "./normalize.css";

export const metadata: Metadata = {
  title: "Doorlist",
  description: "Ticket sales and distribution - but fake!",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
