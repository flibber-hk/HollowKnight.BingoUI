using System;

namespace BingoUI.Counters
{
    public class CharmCounter : AbstractCounter
    {
        public CharmCounter(float x, float y, string spriteName) : base(x, y, spriteName) { }

        public override void Hook()
        {
            On.PlayerData.SetBool += OnSetBool;
            Modding.ModHooks.SetPlayerIntHook += OnGetWhiteFragment;
        }

        private int OnGetWhiteFragment(string name, int orig)
        {
            if (name == nameof(PlayerData.royalCharmState))
            {
                PlayerData.instance.CountCharms();
                int charms = PlayerData.instance.GetInt(nameof(PlayerData.charmsOwned));
                if (orig > 2 && PlayerData.instance.GetInt(nameof(PlayerData.royalCharmState)) <= 2)
                {
                    charms += 1;
                }
                else if (orig <= 2 && PlayerData.instance.GetInt(nameof(PlayerData.royalCharmState)) > 2)
                {
                    charms -= 1;
                }
                UpdateText(charms.ToString());
            }
            return orig;
        }

        private void OnSetBool(On.PlayerData.orig_SetBool orig, PlayerData self, string boolName, bool value)
        {
            orig(self, boolName, value);
            if (boolName.StartsWith("gotCharm_"))
            {
                UpdateText();
            }
        }

        public override string GetText()
        {
            PlayerData.instance.CountCharms();
            return PlayerData.instance.GetInt(nameof(PlayerData.charmsOwned)).ToString();
        }
    }
}
