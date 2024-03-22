'use client'

import Link from "next/link";
import { useCookies } from 'react-cookie';
import { useRouter } from 'next/navigation'
import { env } from 'next-runtime-env';
import { useEffect, useState } from "react";

export default function Navbar() {
    const [cookies, setCookie, removeCookie] = useCookies(['access-token', 'refresh-token'])
    const [initial, setInitial] = useState(true)
    const [loggedIn, setLoggedIn] = useState(false)
    const router = useRouter()
    const NEXT_PUBLIC_API = env('NEXT_PUBLIC_API');

    useEffect(() => {
        setInitial(false)
        setLoggedIn(cookies['access-token'] != null)
    }, [])

    const LogoutRequest = () => {
        fetch(NEXT_PUBLIC_API + '/revoke/token', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ refreshToken: cookies['refresh-token'] }),
        })
            .then(async (response) => {
                if (response.status == 401) {
                    router.push('/login')
                    throw new Error('Unauthorized');
                }
                else if (response.status == 400) {
                    throw new Error('Token could not be revoked');
                }
                removeCookie('access-token', { secure: true, sameSite: 'none' })
                removeCookie('refresh-token', { secure: true, sameSite: 'none' })
                router.push('/')
            })
    }

    if (initial) {
        return (
            <span className="flex justify-between">
                <h1 className="text-xl">Navigation Bar</h1>
            </span>
        );
    }

    if (loggedIn) {
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