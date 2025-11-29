'use client'

import { useSearchParams } from 'next/navigation'
import { useState, useEffect, MouseEventHandler } from 'react';
import { env } from 'next-runtime-env';
import Link from 'next/link'
import { pickImageFilename } from '../../../components/PicPicker'
import { useRouter } from 'next/navigation'
import Select, { SelectChangeEvent } from '@mui/material/Select';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import FormControl from '@mui/material/FormControl';

export interface gameobject {
    x: number,
    y: number,
    z: number,
    guid: number,
    spawnGroupId: number,
    hasDuplicate: boolean,
    duplicates: string
}

export interface entityZone {
    mapId: number,
    zoneId: number,
    name: string
}

export interface gameobjectList {
    count: number
    items: gameobject[]
    left: number
    right: number
    bottom: number
    top: number,
    name: string,
    zones: entityZone[],
    mapIndices: number[]
}

export default function ZoneDisplay() {
    const searchParams = useSearchParams();
    const zone = searchParams.get('zone');
    const entry = searchParams.get('entry');
    const map = searchParams.get('map');
    const index = searchParams.get('index');
    const [picId, setPicId] = useState<string>("0");
    const [gameObjects, setGameObjects] = useState<gameobjectList>({} as gameobjectList);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [isMapView, setIsMapView] = useState<boolean>(false);
    const NEXT_PUBLIC_API = env('NEXT_PUBLIC_API');
    const [selectedGroupId, setSelectedGroupId] = useState<number>(-1);
    const offset = 0;
    const [value, setValue] = useState<number>(0);
    const router = useRouter();

    const loadGos = async () => {
        let tempZone = zone;
        if (isMapView)
            tempZone = "-1";

        setPicId(pickImageFilename(Number(map), Number(tempZone), Number(index)));

        let gameobjects = await fetch(NEXT_PUBLIC_API + '/world/gameobjects/' + map + '/' + tempZone + '/' + entry + '/' + index, {
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

        document.title = gameobjects.name + "(" + entry + ") - GO";
        setGameObjects(gameobjects);
        setIsLoading(false);
    }

    useEffect(() => {
        loadGos();
    }, [])

    useEffect(() => {
        if (isLoading == true)
            return;

        loadGos();
    }, [zone, isMapView, index])

    const onPointHover = (event: React.MouseEvent<HTMLImageElement, MouseEvent>, spawnGroupId: number, apply: boolean) => {
        if (apply && spawnGroupId !== 0)
            setSelectedGroupId(spawnGroupId);
        else
            setSelectedGroupId(-1);
    }

    const onMapViewClick = async () => {
        setIsMapView(!isMapView);
    }

    const onGameObjectClick = async (guid: number, duplicates: string, event: React.MouseEvent<HTMLImageElement, MouseEvent>) => {
        if (event.shiftKey) {
            if (event.ctrlKey) {
                navigator.clipboard.writeText((await navigator.clipboard.readText()) + ',' + (duplicates).toString())
            } else {
                navigator.clipboard.writeText((duplicates).toString())
            }
        } else {
            if (event.ctrlKey) {
                navigator.clipboard.writeText((await navigator.clipboard.readText()) + ',' + (offset > 0 ? "@GGUID+" : "") + (guid - offset).toString())
            } else {
                navigator.clipboard.writeText((offset > 0 ? "@GGUID+" : "") + (guid - offset).toString())
            }
        }
    }

    if (isLoading) {
        return (
            <div style={{ backgroundImage: "url(/" + picId.toString() + ".jpg)", backgroundSize: 'auto', backgroundRepeat: "no-repeat" }}>
            </div>
        )
    }    

    return (
        <div>
            <h1>Map: {map} Zone: {zone} Entry: {entry} Object: &apos;{gameObjects.name}&apos; Count: {gameObjects.count} <Link href={"/world/search/gameobjects"} style={{ marginRight: 10, color: 'white', textDecoration: 'underline' }}>Back</Link><button onClick={onMapViewClick} style={{ outlineStyle: 'solid' }} >{isMapView ? "Go to zone view" : "Go to map view"}</button> </h1>
            <div style={{ textDecorationLine: 'underline' }}>
                <FormControl fullWidth>
                    <InputLabel id="demo-simple-select-label">Map Indices</InputLabel>
                    <Select
                        labelId="demo-simple-select-label"
                        id="demo-simple-select"
                        value={value}
                        label="Map Index"
                        onChange={(event: SelectChangeEvent<Number>, child?) => {
                            if (event.target.value !== null) {
                                router.push("gameobjects?map=" + map + "&zone=" + zone + "&entry=" + entry + '&index=' + event.target.value);

                                setValue(Number(event.target.value));
                            }
                        }}
                    >
                        {
                            gameObjects.mapIndices.map(mapIndex => {
                                return (
                                    <MenuItem value={mapIndex}>{mapIndex.toString()}</MenuItem>
                                );
                            })
                        }
                    </Select>
                </FormControl>
            </div>
            <div>
                {
                    gameObjects.zones.map((otherZone, index) => {
                        return (
                            <span key={otherZone.zoneId} style={{ textDecorationLine: 'underline' }}>
                                <Link href={"gameobjects?map=" + otherZone.mapId + "&zone=" + otherZone.zoneId + "&entry=" + entry} style={{ marginRight: 10, color: (otherZone.zoneId.toString() == zone ? 'white' : 'grey') }}>{otherZone.name}</Link>
                                {(index > 0 && index % 10 == 0) ? <br></br> : ""}
                            </span>
                        );
                    })
                }
            </div>
            <div style={{ position: 'relative', top: 0, left: 0, margin: 0, display: 'inline-block' }}>
                <img src={"/" + picId.toString() + ".jpg"} alt="pin" style={{ display:'block', position: 'relative', top: 0, left: 0, margin: 0, padding: 0, objectFit: 'contain', height: '100%', width: '100%', maxHeight:"100vh" }}></img>
                {
                    gameObjects.items.map(gameobject => {
                        return (
                            <img src={selectedGroupId === gameobject.spawnGroupId ? "/pin-blue.png" : (gameobject.hasDuplicate === true ? "/pin-red.png" : "/pin-yellow.png")} key={gameobject.guid} onClick={(event) => { onGameObjectClick(gameobject.guid, gameobject.duplicates, event) }} className={'map-point-img, ' + gameobject.spawnGroupId} onMouseOver={(e) => { onPointHover(e, gameobject.spawnGroupId, true); }} onMouseOut={(e) => { onPointHover(e, gameobject.spawnGroupId, false); }} alt="pin" title={'' + gameobject.guid + (gameobject.hasDuplicate ? ' - ' : '') + gameobject.duplicates} style={{ width: '1%', minWidth: '11px', margin: 0, padding: 0, transform: 'translate(-50%, -50%)', position: 'absolute', top: (Math.abs((gameobject.x - gameObjects.top) / (gameObjects.bottom - gameObjects.top) * 100)) + '%', left: (100 - Math.abs((gameobject.y - gameObjects.left) / (gameObjects.right - gameObjects.left) * 100)) + '%' }} />
                        );
                    })
                }
            </div>
        </div>
    );
}