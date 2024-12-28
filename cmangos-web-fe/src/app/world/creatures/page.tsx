'use client'

import { useSearchParams } from 'next/navigation'
import { useState, useEffect, MouseEventHandler } from 'react';
import { env } from 'next-runtime-env';
import Link from 'next/link'
import { text } from 'node:stream/consumers';
import { useRouter } from 'next/navigation'

export interface creature {
    x: number,
    y: number,
    z: number,
    guid: number,
    spawnGroupId: number
}

export interface entityZone {
    mapId: number,
    zoneId: number,
    name: string
}

export interface creatureList {
    count: number
    items: creature[]
    left: number
    right: number
    bottom: number
    top: number,
    name: string,
    zones: entityZone[]
}

export default function ZoneDisplay() {
    const searchParams = useSearchParams();
    const zone = searchParams.get('zone');
    const entry = searchParams.get('entry');
    const map = searchParams.get('map');
    const [creatures, setCreatures] = useState<creatureList>({} as creatureList);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const NEXT_PUBLIC_API = env('NEXT_PUBLIC_API');
    const [selectedGroupId, setSelectedGroupId] = useState<number>(-1);
    const offset = 0;
    const router = useRouter();

    const loadGos = async () => {
        let creatures = await fetch(NEXT_PUBLIC_API + '/world/creatures/' + map + '/' + zone + '/' + entry, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        })
            .then(async (response) => {
                if (response.status == 400) {
                    throw new Error('Failed to get creatures data');
                }
                return await response.json()
            })
            .then((r: creatureList) => {
                return r
            });

        setCreatures(creatures);
        setIsLoading(false);
    }

    useEffect(() => {
        loadGos();
    }, [])

    useEffect(() => {
        if (isLoading == true)
            return;

        loadGos();
    }, [zone])

    const onClickCreature = (guid: number, event: React.MouseEvent<HTMLImageElement, MouseEvent>) =>
    {
        if (event.ctrlKey)
            router.push("creature?zone=" + zone + "&guid=" + guid);
        else
            navigator.clipboard.writeText((guid - offset).toString()); 
    };

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
            <h1>Map: {map} Zone: {zone} Entry: {entry} Object: '{creatures.name}' Count: {creatures.count} </h1>
            <div style={{ textDecorationLine: 'underline' }}>
                {
                    creatures.zones.map(otherZone => {
                        return <Link href={"creatures?map=" + otherZone.mapId + "&zone=" + otherZone.zoneId + "&entry=" + entry} style={{ marginRight: 10, color: (otherZone.zoneId.toString() == zone ? 'white' : 'grey') }}>{otherZone.name}</Link>
                    })
                }
            </div>
            <div style={{ position: 'relative', top: 0, left: 0, margin: 0, display: 'inline-block' }}>
                <img src={"/" + zone + ".jpg"} alt="pin" style={{ display:'block', position: 'relative', top: 0, left: 0, margin: 0, padding: 0, objectFit: 'contain', height: '100%', width: '100%', maxHeight:"100vh" }}></img>
                {
                    creatures.items.map(creature => {
                        return (
                            <img src={selectedGroupId === creature.spawnGroupId ? "/pin-blue.png" : "/pin-yellow.png"} key={creature.guid} onClick={(event) => { onClickCreature(creature.guid, event) } } className={'map-point-img, ' + creature.spawnGroupId} onMouseOver={(e) => { onPointHover(e, creature.spawnGroupId, true); }} onMouseOut={(e) => { onPointHover(e, creature.spawnGroupId, false); }} alt="pin" title={'' + creature.guid} style={{ width: '1%', minWidth: '11px', margin: 0, padding: 0, transform: 'translate(-50%, -50%)', position: 'absolute', top: (Math.abs((creature.x - creatures.top) / (creatures.bottom - creatures.top) * 100)) + '%', left: (100 - Math.abs((creature.y - creatures.left) / (creatures.right - creatures.left) * 100)) + '%' }} />
                        );
                    })
                }
            </div>
        </div>
    );
}