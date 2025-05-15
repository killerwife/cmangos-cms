'use client'

import { useSearchParams } from 'next/navigation'
import { useState, useEffect, MouseEventHandler } from 'react';
import { env } from 'next-runtime-env';
import Link from 'next/link'

export interface creatureWithMovement {
    x: number,
    y: number,
    z: number,
    guid: number,
    entry: number,
    name: string,
    map: number,
    movement: creatureMovement[],

    left: number,
    right: number,
    bottom: number,
    top: number
}

export interface creatureMovement {
    x: number,
    y: number,
    z: number
}

export default function ZoneDisplay() {
    const searchParams = useSearchParams();
    const zone = searchParams.get('zone');
    const guid = searchParams.get('guid');
    const [creature, setCreature] = useState<creatureWithMovement>({} as creatureWithMovement);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const NEXT_PUBLIC_API = env('NEXT_PUBLIC_API');
    const [selectedGroupId, setSelectedGroupId] = useState<number>(-1);

    useEffect(() => {
        const loadGos = async () => {
            let creatureWithMovement = await fetch(NEXT_PUBLIC_API + '/world/creature/' + zone + '/' + guid, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            })
                .then(async (response) => {
                    if (response.status == 400) {
                        throw new Error('Failed to get creature data');
                    }
                    return await response.json()
                })
                .then((r: creatureWithMovement) => {
                    return r
                });

            document.title = creatureWithMovement.name + "(" + guid + ") - Creature";
            setCreature(creatureWithMovement);
            setIsLoading(false);
        }

        loadGos();
    }, [])

    const onPointHover = (event: React.MouseEvent<HTMLImageElement, MouseEvent>, spawnGroupId: number, apply: boolean) => {
        if (apply && spawnGroupId !== 0)
            setSelectedGroupId(spawnGroupId);
        else
            setSelectedGroupId(-1);
    }

    if (isLoading) {
        return (
            <div style={{ backgroundImage: "url(/" + zone + ".jpg)", backgroundSize: 'auto', backgroundRepeat: "no-repeat" }}>
            </div>
        )
    }    

    return (
        <div>
            <h1>Zone: {zone} Entry: {creature.entry} Guid: &apos;{guid}&apos; Object: &apos;{creature.name}&apos; <Link href={"creatures?map=" + creature.map + "&zone=" + zone + "&entry=" + creature.entry} style={{ marginRight: 10, color: 'white', textDecoration: 'underline' }}>Back</Link> </h1>
            <div style={{ position: 'relative', top: 0, left: 0, margin: 0, display: 'inline-block' }}>
                <img src={"/" + zone + ".jpg"} alt="pin" style={{ display:'block', position: 'relative', top: 0, left: 0, margin: 0, padding: 0, objectFit: 'contain', height: '100%', width: '100%', maxHeight:"100vh" }}></img>

                <img src="/pin-blue.png" key={creature.guid} className={'map-point-img, ' + creature.guid} onMouseOver={(e) => { onPointHover(e, creature.guid, true); }} onMouseOut={(e) => { onPointHover(e, creature.guid, false); }} alt="pin" title={'' + creature.guid + ' X:' + creature.x + ' Y: ' + creature.y + ' Z:' + creature.z} style={{ width: '1%', minWidth: '11px', margin: 0, padding: 0, transform: 'translate(-50%, -50%)', position: 'absolute', top: (Math.abs((creature.x - creature.top) / (creature.bottom - creature.top) * 100)) + '%', left: (100 - Math.abs((creature.y - creature.left) / (creature.right - creature.left) * 100)) + '%' }} />
                {
                    creature.movement.map(creatureMovement => {
                        return (
                            <img src="/pin-yellow.png" key={creature.guid} className={'map-point-img, ' + creature.guid} onMouseOver={(e) => { onPointHover(e, creature.guid, true); }} onMouseOut={(e) => { onPointHover(e, creature.guid, false); }} alt="pin" title={'' + creature.guid + ' X:' + creatureMovement.x + ' Y: ' + creatureMovement.y + ' Z:' + creatureMovement.z} style={{ width: '1%', minWidth: '11px', margin: 0, padding: 0, transform: 'translate(-50%, -50%)', position: 'absolute', top: (Math.abs((creatureMovement.x - creature.top) / (creature.bottom - creature.top) * 100)) + '%', left: (100 - Math.abs((creatureMovement.y - creature.left) / (creature.right - creature.left) * 100)) + '%' }} />
                        );
                    })
                }
            </div>
        </div>
    );
}