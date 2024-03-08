'use client'

import { useRouter } from 'next/navigation'

export default function Page() {
    const router = useRouter()

    return (
        <main className="flex min-h-screen flex-col items-center justify-between p-24">
            <div className="z-10 max-w-5xl w-full items-center justify-between font-mono text-sm lg:flex">
                <button onClick={() => { router.push('/login') }}>Go to login</button>
            </div>
        </main>
    );
}
