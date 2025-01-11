import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import Navbar from "../components/Navbar";
import { PublicEnvScript } from 'next-runtime-env';
import { StyledEngineProvider } from '@mui/material/styles';
import { ReCaptchaProvider } from "next-recaptcha-v3";

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
              <ReCaptchaProvider>
                <StyledEngineProvider injectFirst>
                    {children}
                  </StyledEngineProvider>
              </ReCaptchaProvider>
          </body>
      </html>
  );
}
