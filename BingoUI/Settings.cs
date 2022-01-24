﻿using System;
using System.Collections.Generic;
using GlobalEnums;

namespace BingoUI
{
    [Serializable]
    public class SaveSettings
    {
        public int spentGeo;

        public Dictionary<string, int> spentTrackedItems = new Dictionary<string, int>();

        public Dictionary<MapZone, int> AreaGrubs = new GrubMap();
        public HashSet<string> Cornifers = new HashSet<string>();

        public HashSet<(string, string)> Enemies = new HashSet<(string, string)>();
    }

    public struct Layout
    {
        public Layout(float x, float y)
        {
            enabled = true;
            this.x = x;
            this.y = y;
        }
        public bool enabled;
        public float x;
        public float y;

        public void Deconstruct(out float x, out float y, out bool enabled)
        {
            x = this.x;
            y = this.y;
            enabled = this.enabled;
        }
    }
    
    [Serializable]
    public class GlobalSettings
    {
        public bool alwaysDisplay = false;
        public bool neverDisplay = false;

        public bool showSpentGeo = true;
        public Dictionary<string, Layout> CounterPositions { get; set; } = new Dictionary<string, Layout>();
        
        public string boardUrl = "";
        public string boardToggle = "Tab";
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