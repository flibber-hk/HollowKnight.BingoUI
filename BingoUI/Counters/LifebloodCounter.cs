using Modding;

namespace BingoUI.Counters
{
    public class LifebloodCounter : AbstractCounter
    {
        public LifebloodCounter(float x, float y, string spriteName) : base(x, y, spriteName) { }
        public override string GetText()
        {
            int raw = PlayerData.instance.GetInt(nameof(PlayerData.healthBlue));
            int lifeblood = PlayerData.instance.GetInt(nameof(PlayerData.joniHealthBlue)) != 0 ? (raw + 1) : raw;
            return $"{lifeblood}";
        }
        public override void Hook()
        {
            ModHooks.SetPlayerIntHook += OnSetInt;
        }

        private int OnSetInt(string name, int orig)
        {
            if (name == nameof(PlayerData.healthBlue))
            {
                int lifeblood = PlayerData.instance.GetInt(nameof(PlayerData.joniHealthBlue)) != 0 ? (orig + 1) : orig;

                UpdateText($"{lifeblood}", canShow: lifeblood > 6);
            }
            return orig;
        }
    }
}
