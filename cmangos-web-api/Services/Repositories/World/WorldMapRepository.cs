using Data.Dto.World;
using Data.Enum;
using Data.Model.Db2;
using Data.Model.DBC;

namespace Services.Repositories.World
{
    public class WorldMapRepository : IWorldMapRepository
    {
        private DBCRepository _dbcRepository;
        private Dictionary<int, Dictionary<int, UiMapAssignment>> _uiMapAssignments;

        public WorldMapRepository(DBCRepository dbcRepository)
        {
            _dbcRepository = dbcRepository;
            _uiMapAssignments = new();
            _uiMapAssignments[2366] = new()
            {
                { 0, new UiMapAssignment(269, 2366, 0, -2225, 6562.4995117188, -1000000, -1500, 7649.9995117188, 1000000, 0)} // Black Morass
            };
            _uiMapAssignments[2367] = new()
            {
                { 0, new UiMapAssignment(560, 2367, 0, 1572.9166259766,-477.08331298828,-1000000,3127.0832519531,1854.1666259766,1000000, 0)} // OHF
            };
            _uiMapAssignments[3457] = new()
            {
                { 1, new UiMapAssignment(532, 3457, 1, -11189.599609375, -2225.0244140625, -10000, -10822.900390625, -1674.9755859375, 1000000, [19619,19622,19648,19651,19686,22149])}, // Kara
                { 2, new UiMapAssignment(532, 3457, 2, -11189.002929688, -2081.419921875, 65, -11017.096679688, -1823.5600585938, 1000000, [19621,19686,19698])},
                { 3, new UiMapAssignment(532, 3457, 3, -11066.299804688, -2132.5747070312, -10000, -10836.200195312, -1787.4252929688, 1000000, [19649,19650,19652])},
                { 4, new UiMapAssignment(532, 3457, 4, -11119.599609375, -2190.0244140625, -10000, -10772.900390625, -1669.9755859375, 1000000, [19625,19626,19627,19628,19629,19633,19634,19635,19636,19637,19640,19650,19652,19747,19748,22148])},
                { 5, new UiMapAssignment(532, 3457, 5, -10969.299804688, -1932.5799560547, 105, -10813.200195312, -1698.4300537109, 1000000, [19634])},
                { 6, new UiMapAssignment(532, 3457, 6, -11190.599609375, -2205.7744140625, -10000, -10802.900390625, -1624.2255859375, 1000000, [19620,19634,19638,19639,19641,19643,19646,19647,20096])},
                { 7, new UiMapAssignment(532, 3457, 7, -11115.599609375, -2066.7744140625, 100, -10987.900390625, -1875.2255859375, 1000000, [19620,19644,19645,19647])},
                { 8, new UiMapAssignment(532, 3457, 8, -11105.200195312, -2037.6802978516, 140, -11012.299804688, -1898.3297119141, 1000000, [19644,19645])},
                { 9, new UiMapAssignment(532, 3457, 9, -11459.599609375, -2270.0244140625, 155, -10952.900390625, -1509.9755859375, 1000000, [19644,19656,19657,19685,20099,20100])},
                { 10, new UiMapAssignment(532, 3457, 10, -11386.333007812, -2040.1300048828, -10000, -11086.166992188, -1589.8800048828, 1000000, [19675,19677,19678,19681,19685])},
                { 11, new UiMapAssignment(532, 3457, 11, -11285.099609375, -1825.5300292969, -10000, -11104.400390625, -1554.4799804688, 1000000, [19679,19680])},
                { 12, new UiMapAssignment(532, 3457, 12, -11444.599609375, -2182.5244140625, -10000, -11047.900390625, -1587.4755859375, 1000000, [19676,19685])},
                { 13, new UiMapAssignment(532, 3457, 13, -11339.099609375, -1963.0244140625, -10000, -10986.400390625, -1433.9755859375, 1000000, [19682,19683,19685])},
                { 14, new UiMapAssignment(532, 3457, 14, -11143, -2032.6300048828, 225, -10979.5, -1787.3800048828, 1000000, [19620,19658,19661,19664,20085])},
                { 15, new UiMapAssignment(532, 3457, 15, -11113.6328125, -2025.0799560547, -10000, -10972.8671875, -1813.9300537109, 1000000, [19659,19662,19663,19664,19665])},
                { 16, new UiMapAssignment(532, 3457, 16, -11097, -2020.1300048828, 260, -11029.5, -1918.8800048828, 1000000, [19620,19664,19674])},
                { 17, new UiMapAssignment(532, 3457, 17, -11102, -2155.1298828125, 270, -10874.5, -1813.8798828125, 1000000, [19620,19660,19664,19666,19667,19668,19669,19670,19671,19672,19684])},
            };
            _uiMapAssignments[3562] = new() // HFR
            {
                { 0, new UiMapAssignment(543, 3562, 0, -1492.4799804688, 1294.7199707031, -10000, -1029.4399414062, 1989.2800292969, 1000000, [0])},
            };
            _uiMapAssignments[3606] = new() // Hyjal
            {
                { 0, new UiMapAssignment(534, 3606, 0, 4479.1665039062, -4024.9997558594, -1000000, 6145.8330078125, -1525, 1000000, [0])},
            };
            _uiMapAssignments[3607] = new() // SSC
            {
                { 0, new UiMapAssignment(548, 3607, 0, -400, -1362.5004882812, -10000, 650.00201416016, 212.50248718262, 1000000, [0])}, 
            };
            _uiMapAssignments[3713] = new() // BF
            {
                { 0, new UiMapAssignment(542, 3713, 0, -65.87670135498, -504.5299987793, -10000, 603.13598632813, 498.98901367188, 1000000, [0])},
            };
            _uiMapAssignments[3714] = new() // SHH
            {
                { 0, new UiMapAssignment(540, 3714, 0, -91.74800109863, -432.63925170898, -10000, 617.4169921875, 631.10821533203, 1000000, [0])},
            };
            _uiMapAssignments[3715] = new() // SV
            {
                { 1, new UiMapAssignment(545, 3715, 1, -425.94219970703, -716.74200439453, -28, 158.56721496582, 160.02200317383, 1000000, [20472,20473,22230,22231,22232])},
                { 2, new UiMapAssignment(545, 3715, 2, -425.94219970703, -716.74200439453, -10000, 158.56721496582, 160.02200317383, 1000000, [20472,20473,22230,22231,22232])},
            };
            _uiMapAssignments[3716] = new() // UB
            {
                { 0, new UiMapAssignment(546, 3716, 0, -172.87818908691, -653.94299316406, -10000, 423.73516845703, 240.97700500488, 1000000, [0])},
            };
            _uiMapAssignments[3717] = new() // SP
            {
                { 0, new UiMapAssignment(547, 3717, 0, -391.65203857422, -836.1240234375, -10000, 201.72003173828, 53.93410110474, 1000000, [0])}, 
            };
            _uiMapAssignments[3789] = new() // SL
            {
                { 0, new UiMapAssignment(555, 3789, 0, -498.21798706055, -656.44317626953, -10000, 62.79690170288, 185.07917785645, 1000000, [0])}, 
            };
            _uiMapAssignments[3790] = new() // Auchenai Crypts
            {
                { 0, new UiMapAssignment(558, 3790, 0, -300, -665, -1000000, 285, 254, 1000000, 0)},
                { 1, new UiMapAssignment(558, 3790, 1, -140.07600402832, -414.98321533203, -10000, 354.95098876953, 327.55722045898, 1000000, [20957,20958,21480])},
                { 2, new UiMapAssignment(558, 3790, 2, -210.07600402832, -602.48321533203, -10000, 334.95098876953, 215.05725097656, 1000000, [20953,20958,20960,20976,20977,20978,21104,21497,22202,22203,22223])},
            };
            _uiMapAssignments[3791] = new() // SH
            {
                { 1, new UiMapAssignment(556, 3791, 1, -295.34298706055, -187.6982421875, -10000, 173.65400695801, 515.79724121094, 1000000, [21009,21281,21553,21555,21556,21557,21559,21560,22059,22064])},
                { 2, new UiMapAssignment(556, 3791, 2, -295.34298706055, -187.6982421875, 13, 173.65400695801, 515.79724121094, 1000000, [21009,21024,21287,21552,21561,22060,22061,22062,22063])},
            };
            _uiMapAssignments[3792] = new() // MT
            {
                { 0, new UiMapAssignment(557, 3792, 0, -458.78601074219, -546.35107421875, -10000, 90.07080078125, 276.93408203125, 1000000, [0])}, 
            };
            _uiMapAssignments[3805] = new() // Zul'Aman
            {
                { 0, new UiMapAssignment(568, 3805, 0, -277.08331298828,583.33331298828,-1000000,568.75,1852.0832519531,1000000, 0)}
            };
            _uiMapAssignments[3836] = new() // Magtheridon
            {
                { 0, new UiMapAssignment(544, 3836, 0, -115.33335113525, -170.5, -10000, 255.33334350586, 385.5, 1000000, [0])}, 
            }; 
            _uiMapAssignments[3845] = new() // Tempest Keep
            {
                { 0, new UiMapAssignment(550, 3845, 0, -100, -787.5, -10000, 950, 787.5, 1000000, 0)}
            };
            _uiMapAssignments[3847] = new() // Botanica
            {
                { 0, new UiMapAssignment(553, 3847, 0, -256.91000366211, -107.64924621582, -10000, 248.02499389648, 649.75323486328, 1000000, [0])},
            };
            _uiMapAssignments[3848] = new() // Arcatraz
            {
                { 1, new UiMapAssignment(552, 3848, 1, -75.42433166504, -405.10501098633, -10000, 384.36502075195, 284.57901000977, 1000000, [21432,21708,21709,21715,21716,21784])},
                { 2, new UiMapAssignment(552, 3848, 2, 44, -245.28703308106, -10000, 408.03201293945, 300.7610168457, 1000000, [21713,21716,21786,21787,21789,21790,21791,21792,21793])},
                { 3, new UiMapAssignment(552, 3848, 3, 150.83297729492, -422.60501098633, -10000, 575.28900146484, 214.07899475098, 1000000, [21712,21783,21785,21787,21788,21789])},
            };
            _uiMapAssignments[3849] = new() // Mechanar
            {
                { 1, new UiMapAssignment(554, 3849, 1, -100.83920288086, -341.7799987793, -10000, 349.98620605469, 334.4580078125, 1000000, [21113,21249,21251,21252,21258,21259,21260,21913])},
                { 2, new UiMapAssignment(554, 3849, 2, -37.83934402466, -341.7799987793, 24, 412.98602294922, 334.4580078125, 1000000, [21113,21249,21250,21255,21256,21257,21912])},
            };
            _uiMapAssignments[3923] = new() // Gruul
            {
                { 0, new UiMapAssignment(565, 3923, 0, -12.5, -50, -10000, 337.5, 475, 1000000, [0])},
            };   
            _uiMapAssignments[3959] = new() // Black Temple
            {
                { 0, new UiMapAssignment(564, 3959, 0, 427.08331298828, 366.66665649414, -1000000, 949.99993896484, 1150, 1000000, 0) },
                { 1, new UiMapAssignment(564, 3959, 1, -240, 23.87053871155, -10000, 594.8330078125, 1276.1201171875, 1000000, [22363,22364,22365,22366,22367,22368,22369,22500])},
                { 2, new UiMapAssignment(564, 3959, 2, 380, -176, -10000, 1030, 799, 1000000, [21697, 21700, 21701, 22071, 22072, 22340, 22355, 22360, 22427, 22428, 22442, 22496, 22499])},
                { 3, new UiMapAssignment(564, 3959, 3, 400, -191, -10000, 1070, 814, 1000000, [22070, 22073, 22074, 22075, 22429])},
                { 4, new UiMapAssignment(564, 3959, 4, 343.3330078125, 134.99951171875, 185, 636.6669921875, 575.00048828125, 1000000, [21697, 22344, 22348, 22426])},
                { 5, new UiMapAssignment(564, 3959, 5, 664.16363525391, -70, -10000, 1110.8303222656, 600, 1000000, [22341, 22342, 22346, 22353, 22478])},
                { 6, new UiMapAssignment(564, 3959, 6, 450, -67.5, -10000, 920, 637.5, 1000000, [22343, 22345, 22347, 22352])},
                { 7, new UiMapAssignment(564, 3959, 7, 606.66668701172, 137.5, -10000, 843.33331298828, 492.5, 1000000, 22351)},
            };
            _uiMapAssignments[4075] = new() // Sunwell
            {
                { 0, new UiMapAssignment(580, 4075, 0, 1408.3299560547, 300, -1000000, 2012.5, 1206.25, 1000000, [0])},
                { 1, new UiMapAssignment(580, 4075, 1, 1580, 377.5, -10000, 1890, 842.5, 1000000, [23468,23469])},
            };
            _uiMapAssignments[4131] = new() // Mgt
            {
                { 1, new UiMapAssignment(585, 4131, 1, -27.79583740234, -304.22601318359, -10000, 325.76013183594, 226.10800170898, 1000000, [23119,23120,23121,23125,23162,23171,23731,23732])},
                { 2, new UiMapAssignment(585, 4131, 2, -27.79589080811, -304.22601318359, -3, 325.76010131836, 226.10800170898, 1000000, [23121,23122,23123,23167,23168,23169,23170,23171,23733,23734,26192])},
            };
            foreach (var dungMap in _dbcRepository.DungeonMaps)
            {
                int zoneId;
                var worldMapAreaEntry = _dbcRepository.WorldMapArea.Where(p => p.Value.Map == dungMap.Value.Map && p.Value.Area == 0).FirstOrDefault();
                if (worldMapAreaEntry.Equals(default(KeyValuePair<int, WorldMapArea>)))
                {
                    zoneId = (int)_dbcRepository.Areas.Where(p => p.Value.mapid == dungMap.Value.Map).Select(s => s.Value.zone == 0 ? (int)s.Value.ID : (int)s.Value.zone).First();
                }
                else
                {
                    zoneId = (int)worldMapAreaEntry.Value.Area;
                }

                if (!_uiMapAssignments.ContainsKey(zoneId))
                {
                    _uiMapAssignments.Add(zoneId, new());
                }

                var wmoGroupIds = _dbcRepository.DungeonMapChunks.Where(p => p.Value.DungeonMap == dungMap.Key).Select(p => (int)p.Value.WmoGroupId).ToList();

                if (_uiMapAssignments[zoneId].ContainsKey((int)dungMap.Value.Index))
                    continue;

                _uiMapAssignments[zoneId].Add((int)dungMap.Value.Index, new UiMapAssignment((int)dungMap.Value.Map, zoneId, dungMap.Value.Index, dungMap.Value.Top, dungMap.Value.Right, -1000000, dungMap.Value.Bottom, dungMap.Value.Left, 1000000, wmoGroupIds));
            }
            foreach (var zoneMap in _uiMapAssignments)
            {
                foreach (var uiMap in zoneMap.Value)
                {
                    if (uiMap.Value.WmoGroupId.Count() == 0)
                        continue;

                    foreach (var wmoGroupId in uiMap.Value.WmoGroupId)
                    {
                        if (wmoGroupId != 0)
                        {
                            var result = _dbcRepository.WmoAreas.Where(p => p.Value.WmoGroupId == wmoGroupId).FirstOrDefault();
                            if (result.Equals(default(KeyValuePair<int, WorldMapArea>)))
                                continue;

                            uiMap.Value.WmoAreaOverride = result.Value.WmoAreaOverride;
                            break;
                        }
                    }
                }
            }
        }

        public int PickIndexForXyz(float x, float y, float z, int zoneId)
        {
            if (_uiMapAssignments.ContainsKey(zoneId))
            {
                foreach (var pair in _uiMapAssignments[zoneId])
                {
                    if (x >= pair.Value.MinX && x <= pair.Value.MaxX &&
                        y >= pair.Value.MinY && y <= pair.Value.MaxY &&
                        z >= pair.Value.MinZ && z <= pair.Value.MaxZ)
                        return pair.Key;
                }

                return 0;
            }

            return 0;
        }

        public UiMapAssignment? GetWorldMapArea(int mapId, int zoneId, int index)
        {
            KeyValuePair<int, WorldMapArea> areaEntry;
            if (zoneId == -1)
            {
                int worldMapId = 0;
                switch (mapId)
                {
                    case 0: worldMapId = 14; break;
                    case 1: worldMapId = 13; break;
                    case 530: worldMapId = 466; break;
                    case 571: worldMapId = 485; break;
                }
                areaEntry = _dbcRepository.WorldMapArea.Where(p => p.Key == worldMapId && mapId == p.Value.Map).SingleOrDefault();

                if (areaEntry.Equals(default(KeyValuePair<int, WorldMapArea>)))
                    return null;

                return new UiMapAssignment(mapId, zoneId, (uint)index, areaEntry.Value);
            }
            else
            {
                if (_uiMapAssignments.ContainsKey(zoneId))
                {
                    if (_uiMapAssignments[zoneId].ContainsKey(index))
                    {
                        return _uiMapAssignments[zoneId][index];
                    }
                }

                if (index != 0) // worldmaparea is fallback for index 0
                    return null;

                areaEntry = _dbcRepository.WorldMapArea.Where(p => p.Value.Area == zoneId && mapId == p.Value.Map).SingleOrDefault();

                if (areaEntry.Equals(default(KeyValuePair<int, WorldMapArea>)))
                    return null;

                return new UiMapAssignment(mapId, zoneId, (uint)index, areaEntry.Value);
            }
        }

        public int PickIndexForWmoGroupId(int wmoGroupId, int zoneId)
        {
            if (_uiMapAssignments.ContainsKey(zoneId))
            {
                foreach (var pair in _uiMapAssignments[zoneId])
                {
                    if (pair.Value.WmoGroupId.Contains(wmoGroupId))
                        return pair.Key;
                }

                return 0;
            }

            return 0;
        }

        public List<MapIndices> GetIndicesForZone(int zoneId)
        {
            if (_uiMapAssignments.ContainsKey(zoneId))
                return _uiMapAssignments[zoneId].Select(p => new MapIndices
                {
                    Index = (int)p.Value.Index,
                    Name = p.Value.WmoAreaOverride
                }).ToList();
            return new List<MapIndices>();
        }
    }
}
