using Modding;

namespace BingoUI.Counters
{
    public class PaleOreCounter : AbstractCounter
    {
        public PaleOreCounter(float x, float y, string spriteName) : base(x, y, spriteName) { }

        public override void Hook()
        {
            ModHooks.SetPlayerIntHook += OnSetInt;
        }
        public override string GetText() => GetText(PlayerData.instance.GetInt(nameof(PlayerData.ore)), PlayerData.instance.GetInt(nameof(PlayerData.nailSmithUpgrades)));
        public string GetText(int ore, int upgrades)
        {
            int oreFromUpgrades = upgrades * (upgrades - 1) / 2; // This equation is stolen from Yusuf
            return $"{ore}({ore + oreFromUpgrades})";
        }
        private int OnSetInt(string name, int orig)
        {
            if (name == nameof(PlayerData.ore) || name == nameof(PlayerData.nailSmithUpgrades))
            {
                int ore = name == nameof(PlayerData.ore) ? orig : PlayerData.instance.GetInt(nameof(PlayerData.ore));
                int upgrades = name == nameof(PlayerData.nailSmithUpgrades) ? orig : PlayerData.instance.GetInt(nameof(PlayerData.nailSmithUpgrades));
                UpdateText(GetText(ore, upgrades));
            }

            return orig;
        }
    }
}
