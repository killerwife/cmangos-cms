'use client'

import { useState, useEffect, MouseEventHandler } from 'react';
import { useRouter } from 'next/navigation'
import Autocomplete from '@mui/material/Autocomplete';
import TextField from '@mui/material/TextField';

export interface searchResult {
    name: string,
    zone: number,
    entry: number
}

export default function CreatureSearch() {
    const [value, setValue] = useState<searchResult | null>();
    const router = useRouter();

    return (
        <div>
            <Autocomplete<searchResult>
                disablePortal
                value={value}
                getOptionLabel={(option) => option.name}
                onChange={(event, newValuel) => {
                    if (newValuel !== null)
                        router.push("creatures?map=" + newValuel.mapId + "&zone=" + newValuel.zoneId + "&entry=" + entry);

                    setValue(newValuel);
                }}
                style={{ color: "blue", outlineColor: "lightblue" }}
                options={creatures.zones}
                sx={{ width: 300 }}
                renderInput={(params) => <TextField {...params} label="Zones" />}
            />
        </div>
    );
}