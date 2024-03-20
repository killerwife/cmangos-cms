'use client'

import { useSearchParams } from 'next/navigation'
import { useState, useEffect } from 'react';

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
    const [gameObjects, setGameObjects] = useState<gameobjectList>({} as gameobjectList)
    const [isLoading, setIsLoading] = useState<boolean>(true)

    useEffect(() => {
        const loadGos = async () => {
            let gameobjects = await fetch('https://localhost:7191/world/gameobject/' + zone + '/' + entry, {
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
        <div style={{ position: 'relative', top: 0, left: 0, margin: 0 }}>
            <img src={"/" + zone + ".jpg"} alt="pin" style={{ position: 'relative', top: 0, left: 0, margin: 0, padding: 0, objectFit: 'contain', height: '100%', width: '100%' }}></img>
            {
                gameObjects.items.map((gameobject) => {
                    return (
                        <img src={"/pin-yellow.png"} alt="pin" title={'' + gameobject.guid} style={{ margin: 0, padding: 0, transform: 'translate(-50%, -50%)', position: 'absolute', top: (Math.abs((gameobject.x - gameObjects.top) / (gameObjects.bottom - gameObjects.top) * 100)) + '%', left: (100 - Math.abs((gameobject.y - gameObjects.left) / (gameObjects.right - gameObjects.left) * 100)) + '%' }} />
                    );
                })
            }
        </div>
    );
}