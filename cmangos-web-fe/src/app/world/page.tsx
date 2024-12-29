'use client'

import Link from 'next/link'
export default function World() {
    return (
        <div style={{ textDecorationLine: 'underline' }}>
            <Link href={"/world/search/creatures"}>Creature search</Link>
            <br/>
            <Link href={"/world/search/gameobjects"}>GameObject search</Link>
        </div>
    );
}