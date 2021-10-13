﻿using System;
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
            MapZone mapZone = SanitizeMapzone(GameManager.instance.sm.mapZone);
            return $"{PlayerData.instance.GetInt(nameof(PlayerData.grubsCollected))}({BingoUI.localSettings.AreaGrubs[mapZone]})";
        }
        public override void Hook()
        {
            ModHooks.SetPlayerIntHook += OnSetInt;
            ItemChangerCompatibility.OnObtainItem += CancelGrubObtain; // If they got a grub from IC then cancel the area add
            ItemChangerCompatibility.OnObtainLocation += OnVisitGrubLocation; // If they checked a grub location, mark it
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChange;
        }

        private void OnSceneChange(Scene arg0, Scene arg1)
        {
            if (BingoUI.globalSettings.alwaysDisplay) UpdateText();
        }

        private void OnVisitGrubLocation(string location)
        {
            if (GrubLocations.TryGetValue(location, out MapZone mapZone))
            {
                BingoUI.localSettings.AreaGrubs[mapZone]++;
                UpdateText(forceShow: true);
            }
        }

        private void CancelGrubObtain(string item)
        {
            if (item == ItemChanger.ItemNames.Grub)
            {
                MapZone mapZone = SanitizeMapzone(GameManager.instance.sm.mapZone);
                BingoUI.localSettings.AreaGrubs[mapZone]--;
                UpdateText(forceShow: true);
            }
        }

        private int OnSetInt(string name, int orig)
        {
            if (name == nameof(PlayerData.grubsCollected))
            {
                MapZone mapZone = SanitizeMapzone(GameManager.instance.sm.mapZone);
                BingoUI.localSettings.AreaGrubs[mapZone]++;
                UpdateText($"{orig}({BingoUI.localSettings.AreaGrubs[mapZone]})");
            }
            return orig;
        }

        
        public static MapZone GetMapZoneFromScene(string sceneName)
        {
            // An placement might not be obtained while the player is in its scene (e.g. itemsync)
            throw new NotImplementedException();
        }

        public static MapZone SanitizeMapzone(MapZone mapZone)
        {
            switch (mapZone)
            {
                case MapZone.CITY:
                case MapZone.LURIENS_TOWER:
                case MapZone.SOUL_SOCIETY:
                case MapZone.KINGS_STATION:
                    return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Ruins2_11"
                        ? MapZone.LOVE_TOWER
                        // This is Tower of Love, which is separate from city and KE for rando goal purposes
                        : MapZone.CITY;
                case MapZone.CROSSROADS:
                case MapZone.SHAMAN_TEMPLE:
                    return MapZone.CROSSROADS;
                case MapZone.BEASTS_DEN:
                case MapZone.DEEPNEST:
                    return MapZone.DEEPNEST;
                case MapZone.FOG_CANYON:
                case MapZone.MONOMON_ARCHIVE:
                    return MapZone.FOG_CANYON;
                case MapZone.WASTES:
                case MapZone.QUEENS_STATION:
                    return MapZone.WASTES;
                case MapZone.OUTSKIRTS:
                case MapZone.HIVE:
                case MapZone.COLOSSEUM:
                    return MapZone.OUTSKIRTS;
                case MapZone.TOWN:
                case MapZone.KINGS_PASS:
                    return MapZone.TOWN;
                case MapZone.WATERWAYS:
                case MapZone.GODSEEKER_WASTE:
                    return MapZone.WATERWAYS;
                default:
                    return mapZone;
            }
        }

        private static readonly Dictionary<string, MapZone> GrubLocations = new Dictionary<string, MapZone>()
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
            [ItemChanger.LocationNames.Grub_Hive_External] = MapZone.HIVE,
            [ItemChanger.LocationNames.Grub_Hive_Internal] = MapZone.HIVE,
            [ItemChanger.LocationNames.Grub_Basin_Requires_Wings] = MapZone.ABYSS,
            [ItemChanger.LocationNames.Grub_Basin_Requires_Dive] = MapZone.ABYSS,
            [ItemChanger.LocationNames.Grub_Waterways_Main] = MapZone.WATERWAYS,
            [ItemChanger.LocationNames.Grub_Waterways_East] = MapZone.WATERWAYS,
            [ItemChanger.LocationNames.Grub_Waterways_Requires_Tram] = MapZone.WATERWAYS,
            [ItemChanger.LocationNames.Grub_City_of_Tears_Left] = MapZone.CITY,
            [ItemChanger.LocationNames.Grub_Soul_Sanctum] = MapZone.SOUL_SOCIETY,
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
            [ItemChanger.LocationNames.Grub_Collector_1] = MapZone.CITY,
            [ItemChanger.LocationNames.Grub_Collector_2] = MapZone.CITY,
            [ItemChanger.LocationNames.Grub_Collector_3] = MapZone.CITY
        };

    }
}
