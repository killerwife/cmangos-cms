import { redirect, usePathname } from "next/navigation";

export default function Home() {
    const pathname = usePathname()

    return (
        <main className="flex min-h-screen flex-col items-center justify-between p-24">
            <div className="z-10 max-w-5xl w-full items-center justify-between font-mono text-sm lg:flex">
                <button onClick={redirect("/login")}>Go to login</button>
            </div>
        </main>
    );
}
