using System;
using GlobalEnums;
using System.Collections.Generic;
using Modding;
using UnityEngine.SceneManagement;

namespace BingoUI.Counters
{
    public class GrubCounter : AbstractCounter
    {
        public GrubCounter(float x, float y, string spriteName) : base(x, y, spriteName) { }

        public override string GetText()
        {
            MapZone mapZone = GetSanitizedMapzone();
            return $"{PlayerData.instance.GetInt(nameof(PlayerData.grubsCollected))}({BingoUI.LS.AreaGrubs[mapZone]})";
        }
        public override void Hook()
        {
            ModHooks.SetPlayerIntHook += OnSetInt;
            ItemChangerCompatibility.OnObtainItem += CancelGrubObtain; // If they got a grub from IC then it wasn't obtained from its location, so un-add it
            ItemChangerCompatibility.OnObtainLocation += OnVisitGrubLocation; // If they checked a grub location, mark it
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChange; // On scene change, the grubs from current area might change
        }

        private void OnSceneChange(Scene arg0, Scene arg1)
        {
            if (BingoUI.GS.alwaysDisplay) UpdateText(canShow: false);
        }

        private void OnVisitGrubLocation(string location)
        {
            if (GrubLocations.TryGetValue(location, out MapZone mapZone))
            {
                BingoUI.LS.AreaGrubs[mapZone]++;
                UpdateText(forceShow: true);
            }
        }

        private void CancelGrubObtain(string item)
        {
            if (item == ItemChanger.ItemNames.Grub)
            {
                MapZone mapZone = GetSanitizedMapzone();
                BingoUI.LS.AreaGrubs[mapZone]--;
                UpdateText(forceShow: true);
            }
        }

        private int OnSetInt(string name, int orig)
        {
            if (name == nameof(PlayerData.grubsCollected))
            {
                MapZone mapZone = GetSanitizedMapzone();
                BingoUI.LS.AreaGrubs[mapZone]++;
                UpdateText($"{orig}({BingoUI.LS.AreaGrubs[mapZone]})");
            }
            return orig;
        }

        /// <summary>
        /// Return the current mapzone, sanitized according to grub areas.
        /// </summary>
        public static MapZone GetSanitizedMapzone()
        {
            return GameManager.instance.sm.mapZone switch
            {
                MapZone.CITY or MapZone.LURIENS_TOWER or MapZone.SOUL_SOCIETY or MapZone.KINGS_STATION => GameManager.instance.sceneName.StartsWith("Ruins2_11")
                                        // This is Tower of Love, which is separate from city and KE for rando goal purposes
                                        ? MapZone.LOVE_TOWER
                                        : MapZone.CITY,
                MapZone.CROSSROADS or MapZone.SHAMAN_TEMPLE => MapZone.CROSSROADS,
                MapZone.BEASTS_DEN or MapZone.DEEPNEST => MapZone.DEEPNEST,
                MapZone.FOG_CANYON or MapZone.MONOMON_ARCHIVE => MapZone.FOG_CANYON,
                MapZone.WASTES or MapZone.QUEENS_STATION => MapZone.WASTES,
                MapZone.OUTSKIRTS or MapZone.HIVE or MapZone.COLOSSEUM => MapZone.OUTSKIRTS,
                MapZone.TOWN or MapZone.KINGS_PASS => MapZone.TOWN,
                MapZone.WATERWAYS or MapZone.GODSEEKER_WASTE => MapZone.WATERWAYS,
                _ => GameManager.instance.sm.mapZone,
            };
        }

        // These represent the Sanitized MapZones of the scenes containing the grubs
        private static readonly Dictionary<string, MapZone> GrubLocations = new()
        {
            [ItemChanger.LocationNames.Grub_Crossroads_Acid] = MapZone.CROSSROADS,
            [ItemChanger.LocationNames.Grub_Crossroads_Center] = MapZone.CROSSROADS,
            [ItemChanger.LocationNames.Grub_Crossroads_Stag] = MapZone.CROSSROADS,
            [ItemChanger.LocationNames.Grub_Crossroads_Spike] = MapZone.CROSSROADS,
            [ItemChanger.LocationNames.Grub_Crossroads_Guarded] = MapZone.CROSSROADS,
            [ItemChanger.LocationNames.Grub_Greenpath_Cornifer] = MapZone.GREEN_PATH,
            [ItemChanger.LocationNames.Grub_Greenpath_Journal] = MapZone.GREEN_PATH,
            [ItemChanger.LocationNames.Grub_Greenpath_MMC] = MapZone.GREEN_PATH,
            [ItemChanger.LocationNames.Grub_Greenpath_Stag] = MapZone.GREEN_PATH,
            [ItemChanger.LocationNames.Grub_Fog_Canyon] = MapZone.FOG_CANYON,
            [ItemChanger.LocationNames.Grub_Fungal_Bouncy] = MapZone.WASTES,
            [ItemChanger.LocationNames.Grub_Fungal_Spore_Shroom] = MapZone.WASTES,
            [ItemChanger.LocationNames.Grub_Deepnest_Mimic] = MapZone.DEEPNEST,
            [ItemChanger.LocationNames.Grub_Deepnest_Nosk] = MapZone.DEEPNEST,
            [ItemChanger.LocationNames.Grub_Deepnest_Spike] = MapZone.DEEPNEST,
            [ItemChanger.LocationNames.Grub_Dark_Deepnest] = MapZone.DEEPNEST,
            [ItemChanger.LocationNames.Grub_Beasts_Den] = MapZone.DEEPNEST,
            [ItemChanger.LocationNames.Grub_Kingdoms_Edge_Oro] = MapZone.OUTSKIRTS,
            [ItemChanger.LocationNames.Grub_Kingdoms_Edge_Camp] = MapZone.OUTSKIRTS,
            [ItemChanger.LocationNames.Grub_Hive_External] = MapZone.OUTSKIRTS, // MapZone.HIVE,
            [ItemChanger.LocationNames.Grub_Hive_Internal] = MapZone.OUTSKIRTS, // MapZone.HIVE,
            [ItemChanger.LocationNames.Grub_Basin_Requires_Wings] = MapZone.ABYSS,
            [ItemChanger.LocationNames.Grub_Basin_Requires_Dive] = MapZone.ABYSS,
            [ItemChanger.LocationNames.Grub_Waterways_Main] = MapZone.WATERWAYS,
            [ItemChanger.LocationNames.Grub_Ismas_Grove] = MapZone.WATERWAYS,
            [ItemChanger.LocationNames.Grub_Waterways_Requires_Tram] = MapZone.WATERWAYS,
            [ItemChanger.LocationNames.Grub_City_of_Tears_Left] = MapZone.CITY,
            [ItemChanger.LocationNames.Grub_Soul_Sanctum] = MapZone.CITY, // MapZone.SOUL_SOCIETY,
            [ItemChanger.LocationNames.Grub_Watchers_Spire] = MapZone.CITY,
            [ItemChanger.LocationNames.Grub_City_of_Tears_Guarded] = MapZone.CITY,
            [ItemChanger.LocationNames.Grub_Kings_Station] = MapZone.CITY,
            [ItemChanger.LocationNames.Grub_Resting_Grounds] = MapZone.RESTING_GROUNDS,
            [ItemChanger.LocationNames.Grub_Crystal_Peak_Below_Chest] = MapZone.MINES,
            [ItemChanger.LocationNames.Grub_Crystallized_Mound] = MapZone.MINES,
            [ItemChanger.LocationNames.Grub_Crystal_Peak_Spike] = MapZone.MINES,
            [ItemChanger.LocationNames.Grub_Crystal_Peak_Mimic] = MapZone.MINES,
            [ItemChanger.LocationNames.Grub_Crystal_Peak_Crushers] = MapZone.MINES,
            [ItemChanger.LocationNames.Grub_Crystal_Heart] = MapZone.MINES,
            [ItemChanger.LocationNames.Grub_Hallownest_Crown] = MapZone.MINES,
            [ItemChanger.LocationNames.Grub_Howling_Cliffs] = MapZone.CLIFFS,
            [ItemChanger.LocationNames.Grub_Queens_Gardens_Stag] = MapZone.ROYAL_GARDENS,
            [ItemChanger.LocationNames.Grub_Queens_Gardens_Marmu] = MapZone.ROYAL_GARDENS,
            [ItemChanger.LocationNames.Grub_Queens_Gardens_Top] = MapZone.ROYAL_GARDENS,
            [ItemChanger.LocationNames.Grub_Collector_1] = MapZone.LOVE_TOWER, // MapZone.CITY,
            [ItemChanger.LocationNames.Grub_Collector_2] = MapZone.LOVE_TOWER, // MapZone.CITY,
            [ItemChanger.LocationNames.Grub_Collector_3] = MapZone.LOVE_TOWER, // MapZone.CITY
        };

    }
}
