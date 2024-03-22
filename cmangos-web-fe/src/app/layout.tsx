import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import Navbar from "../components/Navbar";
import { PublicEnvScript } from 'next-runtime-env';

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "cMaNGOS CMS",
  description: "Account management",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
      <html lang="en">
          <head>
              <PublicEnvScript />
          </head>
          <body className={inter.className}>
              <Navbar></Navbar>
              {children}
          </body>
      </html>
  );
}
