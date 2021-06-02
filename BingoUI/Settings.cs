using System;
using System.Collections.Generic;
using GlobalEnums;
using Modding;

namespace BingoUI
{
    [Serializable]
    public class SaveSettings : ModSettings
    {
        public int DreamTreesCompleted;

        public int spentGeo;

        public Dictionary<MapZone, int> AreaGrubs = new GrubMap();

        public HashSet<string> Cornifers = new HashSet<string>();
        
        public HashSet<(string,string)> Devouts = new HashSet<(string, string)>();
        public HashSet<(string,string)> GreatHuskSentries = new HashSet<(string, string)>();
    }

    public struct Layout
    {
        public Layout(bool e, float x, float y) {
            enabled = e;
            X = x;
            Y = y;
        }

        public bool enabled;
        public float X;
        public float Y;
    }
    
    [Serializable]
    public class GlobalSettings : ModSettings
    {
        public bool alwaysDisplay=false;
        public bool customLayout=false;

        public bool grub_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float grub_x {
            get => GetFloat(0f);
            set => SetFloat(value);
        }
        public float grub_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool devout_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float devout_x {
            get => GetFloat(1f/15f);
            set => SetFloat(value);
        }
        public float devout_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool trinket1_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float trinket1_x {
            get => GetFloat(2f/15f);
            set => SetFloat(value);
        }
        public float trinket1_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool trinket2_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float trinket2_x {
            get => GetFloat(3f/15f);
            set => SetFloat(value);
        }
        public float trinket2_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool trinket3_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float trinket3_x {
            get => GetFloat(4f/15f);
            set => SetFloat(value);
        }
        public float trinket3_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool trinket4_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float trinket4_x {
            get => GetFloat(5f/15f);
            set => SetFloat(value);
        }
        public float trinket4_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool ore_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float ore_x {
            get => GetFloat(6f/15f);
            set => SetFloat(value);
        }
        public float ore_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool maps_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float maps_x {
            get => GetFloat(7f/15f);
            set => SetFloat(value);
        }
        public float maps_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool cornifer_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float cornifer_x {
            get => GetFloat(8f/15f);
            set => SetFloat(value);
        }
        public float cornifer_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool regg_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float regg_x {
            get => GetFloat(9f/15f);
            set => SetFloat(value);
        }
        public float regg_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool DreamPlant_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float DreamPlant_x {
            get => GetFloat(10f/15f);
            set => SetFloat(value);
        }
        public float DreamPlant_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool lifeblood_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float lifeblood_x {
            get => GetFloat(11f/15f);
            set => SetFloat(value);
        }
        public float lifeblood_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool charms_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float charms_x {
            get => GetFloat(12f/15f);
            set => SetFloat(value);
        }
        public float charms_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool pins_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float pins_x {
            get => GetFloat(13f/15f);
            set => SetFloat(value);
        }
        public float pins_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool notches_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float notches_x {
            get => GetFloat(14f/15f);
            set => SetFloat(value);
        }
        public float notches_y {
            get => GetFloat(0.01f);
            set => SetFloat(value);
        }
        public bool ghs_enabled {
            get => GetBool(true);
            set => SetBool(value);
        }
        public float ghs_x {
            get => GetFloat(14f/15f);
            set => SetFloat(value);
        }
        public float ghs_y {
            get => GetFloat(0.12f);
            set => SetFloat(value);
        }
    }

    public class GrubMap : Dictionary<MapZone, int>
    {
        public GrubMap()
        {
            foreach (MapZone mz in Enum.GetValues(typeof(MapZone)))
                this[mz] = 0;
        }
    }
}