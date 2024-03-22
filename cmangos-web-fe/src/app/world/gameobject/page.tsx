'use client'

import { useSearchParams } from 'next/navigation'
import { useState, useEffect } from 'react';
import { env } from 'next-runtime-env';

export interface gameobject {
    x: number,
    y: number,
    z: number,
    guid: number
}

export interface gameobjectList {
    count: number
    items: gameobject[]
    left: number
    right: number
    bottom: number
    top: number
}

export default function ZoneDisplay() {
    const searchParams = useSearchParams()
    const zone = searchParams.get('zone')
    const entry = searchParams.get('entry')
    const map = searchParams.get('map')
    const [gameObjects, setGameObjects] = useState<gameobjectList>({} as gameobjectList)
    const [isLoading, setIsLoading] = useState<boolean>(true)
    const NEXT_PUBLIC_FOO = env('NEXT_PUBLIC_FOO');

    useEffect(() => {
        const loadGos = async () => {
            let gameobjects = await fetch(NEXT_PUBLIC_FOO + '/world/gameobject/' + map + '/' + zone + '/' + entry, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            })
                .then(async (response) => {
                    if (response.status == 400) {
                        throw new Error('Failed to get gameobject data');
                    }
                    return await response.json()
                })
                .then((r: gameobjectList) => {
                    return r
                });

            setGameObjects(gameobjects);
            setIsLoading(false);
        }

        loadGos();
    }, [])

    if (isLoading) {
        return (
            <div style={{ backgroundImage: "url(/" + zone + ".jpg)", backgroundSize: 'auto', backgroundRepeat: "no-repeat" }}>
            </div>
        )
    }    

    return (
        <div>
            <h1>Map: {map} Zone: {zone} Object: {entry} Count: {gameObjects.count} </h1>
            <div style={{ position: 'relative', top: 0, left: 0, margin: 0, display: 'inline-block' }}>
                <img src={"/" + zone + ".jpg"} alt="pin" style={{ display:'block', position: 'relative', top: 0, left: 0, margin: 0, padding: 0, objectFit: 'contain', height: '100%', width: '100%', maxHeight:"100vh" }}></img>
                {
                    gameObjects.items.map((gameobject) => {
                        return (
                            <img src={"/pin-yellow.png"} alt="pin" title={'' + gameobject.guid} style={{ width: '1%', minWidth: '11px', margin: 0, padding: 0, transform: 'translate(-50%, -50%)', position: 'absolute', top: (Math.abs((gameobject.x - gameObjects.top) / (gameObjects.bottom - gameObjects.top) * 100)) + '%', left: (100 - Math.abs((gameobject.y - gameObjects.left) / (gameObjects.right - gameObjects.left) * 100)) + '%' }} />
                        );
                    })
                }
            </div>
        </div>
    );
}