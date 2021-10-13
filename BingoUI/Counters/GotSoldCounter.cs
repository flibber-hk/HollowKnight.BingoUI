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
        public override string GetText() 
        {
            int got = PlayerData.instance.GetInt(obtainString);
            int sold = PlayerData.instance.GetInt(sellString);
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
                int got;
                if (name == obtainString) got = orig;
                else got = PlayerData.instance.GetInt(obtainString);
                int sold;
                if (name == sellString) sold = orig;
                else sold = PlayerData.instance.GetInt(sellString);

                UpdateText($"{got}({got + sold})");
            }
            return orig;
        }
    }
}
