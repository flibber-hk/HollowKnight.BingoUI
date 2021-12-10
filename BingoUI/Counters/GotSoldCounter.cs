using Modding;

namespace BingoUI.Counters
{
    public class GotSoldCounter : AbstractCounter
    {
        public string obtainString;
        public string sellString;

        public GotSoldCounter(float x, float y, string spriteName, string obtainString, string sellString) : base(x, y, spriteName)
        {
            this.obtainString = obtainString;
            this.sellString = sellString;
        }
        public override string GetText() => GetText(PlayerData.instance.GetInt(obtainString), PlayerData.instance.GetInt(sellString));
        public string GetText(int got, int sold) 
        {
            return $"{got}({got + sold})";
        }
        public override void Hook()
        {
            ModHooks.SetPlayerIntHook += OnSetInt;
        }

        private int OnSetInt(string name, int orig)
        {
            if (name == obtainString || name == sellString)
            {
                int got = name == obtainString ? orig : PlayerData.instance.GetInt(obtainString);
                int sold = name == obtainString ? orig : PlayerData.instance.GetInt(sellString);

                UpdateText(GetText(got, sold));
            }
            return orig;
        }
    }
}
