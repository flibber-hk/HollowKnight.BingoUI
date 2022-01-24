using System.Collections.Generic;
using Modding;

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
            entries.Add(new IMenuMod.MenuEntry
            {
                Name = "Show Geo Tracker:",
                Description = string.Empty,
                Values = new string[] { "True", "False" },
                Saver = (i) => GS.showSpentGeo = i == 0,
                Loader = () => GS.showSpentGeo ? 0 : 1
            });

            return entries;
        }

        public void SetDisplayState(int i)
        {
            GS.alwaysDisplay = i == 1;
            GS.neverDisplay = i == 2;
        }

        public int GetDisplayState()
        {
            if (GS.alwaysDisplay) return 1;
            else if (GS.neverDisplay) return 2;
            return 0;
        }
    }
}
