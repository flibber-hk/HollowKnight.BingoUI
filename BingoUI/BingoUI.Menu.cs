using System.Collections.Generic;
using Modding;
using BingoUI.Counters;

namespace BingoUI
{
    public partial class BingoUI : IMenuMod
    {
        public bool ToggleButtonInsideMenu => false;


        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> entries = new List<IMenuMod.MenuEntry>();

            entries.Add(new IMenuMod.MenuEntry
            {
                Name = "Display Counters:",
                Description = "Toggle when to display the counters",
                Values = new string[] {"On Change", "Always", "Never" },
                Saver = SetDisplayState,
                Loader = GetDisplayState
            });

            // Button for each counter?

            return entries;
        }

        public void SetDisplayState(int i)
        {
            globalSettings.alwaysDisplay = i == 1;
            globalSettings.neverDisplay = i == 2;
        }

        public int GetDisplayState()
        {
            if (globalSettings.alwaysDisplay) return 1;
            else if (globalSettings.neverDisplay) return 2;
            return 0;
        }
    }
}
