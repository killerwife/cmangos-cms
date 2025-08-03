export const pickImageFilename = (mapId: number, zoneId: number): number => {
    if (zoneId == -1) {
        switch (Number(mapId)) {
            case 0: return -3;
            case 1: return -6;
        }
    }
    else {
        return zoneId;
    }

    return 0;
}