'use client'

import Link from "next/link";
import { useCookies } from 'react-cookie';

export default function Navbar() {
    const [cookies, setCookie, removeCookie] = useCookies(['access-token', 'refresh-token'])

    const LogoutRequest = () => {
        fetch('https://localhost:7191/revoke/token', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ refreshToken: cookies['refresh-token'] }),
        })
            .then(async (response) => {
                removeCookie('access-token', { secure: true, sameSite: 'none' })
                removeCookie('refresh-token', { secure: true, sameSite: 'none' })
            })
    }

    if (cookies['access-token'] != null) {
        return (
            <span className="flex justify-between">
                <h1 className="text-xl">Navigation Bar</h1>
                <ul className="flex gap-4">
                    <li>
                        <Link href="/">Home</Link>
                    </li>
                    <li>
                        <Link href="/account">Account</Link>
                    </li>
                    <li>
                        <button onClick={() => { LogoutRequest(); }} >Log out</button>
                    </li>
                </ul>
            </span>
        );
    }

    return (
        <span className="flex justify-between">
            <h1 className="text-xl">Navigation Bar</h1>
            <ul className="flex gap-4">
                <li>
                    <Link href="/">Home</Link>
                </li>
                <li>
                    <Link href="/login">Login</Link>
                </li>
                <li>
                    <Link href="/register">Sign Up</Link>
                </li>
            </ul>
        </span>
    );
}