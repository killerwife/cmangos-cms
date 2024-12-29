'use client'

import { useState, useEffect, MouseEventHandler } from 'react';
import { useRouter } from 'next/navigation'
import { env } from 'next-runtime-env';
import Autocomplete from '@mui/material/Autocomplete';
import TextField from '@mui/material/TextField';
import Link from 'next/link'

export interface searchResult {
    name: string,
    map: number,
    zone: number,
    entry: number
}

export default function CreatureSearch() {
    const [value, setValue] = useState<searchResult | null>();
    const router = useRouter();
    const [inputValue, setInputValue] = useState('');
    const [creatures, setCreatures] = useState<searchResult[]>([] as searchResult[]);
    const NEXT_PUBLIC_API = env('NEXT_PUBLIC_API');

    const loadPredictions = async () => {
        if (inputValue === '' || inputValue.length < 3) {
            setCreatures(value ? [value] : []);
            return undefined;
        }

        let creatures = await fetch(NEXT_PUBLIC_API + '/world/creatures/predict', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ name: inputValue }),
        })
            .then(async (response) => {
                if (response.status == 400) {
                    throw new Error('Failed to get creatures data');
                }
                return await response.json()
            })
            .then((r: searchResult[]) => {
                return r
            });

        setCreatures(creatures);
    }

    useEffect(() => {
        loadPredictions();
    }, [value, inputValue, fetch]);

    return (
        <div>
            <Link href={"/world"} style={{ marginRight: 10, color: 'white', textDecoration: 'underline' }}>Back</Link>
            <Autocomplete<searchResult>
                disablePortal
                value={value}
                getOptionLabel={(option) => option.name}
                filterOptions={(x) => x}
                onChange={(event, newValue) => {
                    if (newValue !== null)
                        router.push("/world/creatures?map=" + newValue.map + "&zone=" + newValue.zone + "&entry=" + newValue.entry);

                    setValue(newValue);
                }}
                onInputChange={(event, newInputValue) => {
                    setInputValue(newInputValue);
                }}
                style={{ color: "blue", outlineColor: "lightblue" }}
                options={creatures}
                sx={{ width: 300 }}
                renderInput={(params) => <TextField {...params} label="Zones" />}
            />
        </div>
    );
}