'use client'

import { useRouter } from 'next/navigation'
import { useCookies } from 'react-cookie';

const LogoutRequest = () => {

}

export default function Page() {
    const router = useRouter()
    const cookies = useCookies(['access-token'])

    if (cookies != null) {
        return (
            <main className="flex min-h-screen flex-col items-center justify-between p-24">
                <div className="z-10 max-w-5xl w-full items-center justify-between font-mono text-sm lg:flex">
                    <button onClick={() => { router.push('/account') }}>Go to account</button>
                </div>
            </main>
        );
    }
    document.cookie
    return (
        <main className="flex min-h-screen flex-col items-center justify-between p-24">
            <div className="z-10 max-w-5xl w-full items-center justify-between font-mono text-sm lg:flex">
                <button onClick={() => { router.push('/login') }}>Go to login</button>
            </div>
        </main>
    );
}
