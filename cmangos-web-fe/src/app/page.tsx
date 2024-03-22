'use client'

import { useRouter } from 'next/navigation'
import { useEffect, useState } from 'react';
import { useCookies } from 'react-cookie';

export default function Page() {
    const router = useRouter()
    const [cookies] = useCookies(['access-token'])
    const [loggedIn, setLoggedIn] = useState(false)

    useEffect(() => {
        setLoggedIn(cookies['access-token'] != null)
    }, [])

    return (
        <main className="flex min-h-screen flex-col items-center justify-between p-24">
            <div className="z-10 max-w-5xl w-full items-center justify-between font-mono text-sm lg:flex">
                <h1>Welcome to cMaNGOS CMS</h1>
            </div>
        </main>
    );
}
