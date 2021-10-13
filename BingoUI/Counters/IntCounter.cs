using Modding;

namespace BingoUI.Counters
{
    public class IntCounter : AbstractCounter
    {
        private readonly string intName;

        public IntCounter(float x, float y, string spriteName, string intName) : base(x, y, spriteName)
        {
            this.intName = intName;
        }
        public override string GetText() => $"{PlayerData.instance.GetInt(intName)}";
        public override void Hook()
        {
            ModHooks.SetPlayerIntHook += OnSetInt;
        }

        private int OnSetInt(string name, int orig)
        {
            if (name == intName)
            {
                UpdateText($"{orig}");
            }
            return orig;
        }
    }
}
