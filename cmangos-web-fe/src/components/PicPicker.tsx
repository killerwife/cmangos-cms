export const pickImageFilename = (mapId: number, zoneId: number, index: number): string => {
    if (zoneId == -1) {
        switch (Number(mapId)) {
            case 0: return "-3";
            case 1: return "-6";
            case 530: return "-2";
            case 571: return "-5";
        }
    }
    else {
        if (index > 0)
            return zoneId.toString() + '-' + index.toString();

        return zoneId.toString();
    }

    return "0";
}