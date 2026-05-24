import type { Metadata } from "next";
import "./normalize.css";

import { RootLayout} from "./components/Navigation/RootLayout/RootLayout";

export const metadata: Metadata = {
  title: "Doorlist",
  description: "Ticket sales and distribution - but fake!",
};

export default function RootLayoutContainer({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
    <body>
      <RootLayout>{children}</RootLayout>
    </body>
    </html>
  );
}
