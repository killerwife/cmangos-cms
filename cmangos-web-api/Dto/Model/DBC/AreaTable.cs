namespace Data.Model.DBC
{
    [DBFile("AreaTable")]
    public class AreaTable
    {
        public uint ID;                                             // 0        m_ID
        public uint mapid;                                          // 1        m_ContinentID
        public uint zone;                                           // 2        m_ParentAreaID
        public uint exploreFlag;                                    // 3        m_AreaBit
        public uint flags;                                          // 4        m_flags
        public uint soundProvider;                                  // 5        m_SoundProviderPref
        public uint soundProviderUnderwater;                        // 6        m_SoundProviderPrefUnderwater
        public uint ambienceId;                                     // 7        m_AmbienceID
        public uint zoneMusic;                                      // 8        m_ZoneMusic
        public uint introSound;                                     // 9        m_IntroSound
        public int area_level;                                      // 10       m_ExplorationLevel
        public string area_name;                                    // 11-26    m_AreaName_lang
    }
}
