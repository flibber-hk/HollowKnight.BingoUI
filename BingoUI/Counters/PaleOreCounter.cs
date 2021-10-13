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
        public override string GetText()
        {
            int ore = PlayerData.instance.GetInt(nameof(PlayerData.ore));
            int upgrades = PlayerData.instance.GetInt(nameof(PlayerData.nailSmithUpgrades));
            int oreFromUpgrades = upgrades * (upgrades - 1) / 2; // This equation is stolen from Yusuf
            return $"{ore}({ore + oreFromUpgrades})";
        }
        private int OnSetInt(string name, int orig)
        {
            if (name == nameof(PlayerData.ore) || name == nameof(PlayerData.nailSmithUpgrades))
            {
                int ore;
                if (name == nameof(PlayerData.ore)) ore = orig;
                else ore = PlayerData.instance.GetInt(nameof(PlayerData.ore));
                int upgrades;
                if (name == nameof(PlayerData.nailSmithUpgrades)) upgrades = orig;
                else upgrades = PlayerData.instance.GetInt(nameof(PlayerData.nailSmithUpgrades));
                int oreFromUpgrades = upgrades * (upgrades - 1) / 2; // This equation is stolen from Yusuf
                UpdateText($"{ore}({ore + oreFromUpgrades})");
            }

            return orig;
        }
    }
}
