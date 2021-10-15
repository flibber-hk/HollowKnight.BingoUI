using Modding;

namespace BingoUI.Counters
{
    public class TrackedGotSoldCounter : AbstractCounter
    {
        public string fieldName;

        public TrackedGotSoldCounter(float x, float y, string spriteName, string fieldName) : base(x, y, spriteName)
        {
            this.fieldName = fieldName;
        }

        private int AddToSold(int increment)
        {
            if (!BingoUI.localSettings.spentTrackedItems.TryGetValue(fieldName, out int sold))
            {
                BingoUI.localSettings.spentTrackedItems.Add(fieldName, 0);
                return 0;
            }
            BingoUI.localSettings.spentTrackedItems[fieldName] = sold + increment;
            return sold + increment;
        }
        private int GetSold() => AddToSold(0);

        public override string GetText()
        {
            int got = PlayerData.instance.GetInt(fieldName);
            int sold = GetSold();
            return $"{got}({got + sold})";
        }

        public override void Hook()
        {
            ModHooks.SetPlayerIntHook += OnSetInt;
        }

        private int OnSetInt(string name, int set)
        {
            if (name == fieldName)
            {
                int orig = PlayerData.instance.GetInt(fieldName);
                int sold;
                if (orig > set)
                {
                    sold = AddToSold(orig - set);
                }
                else
                {
                    sold = GetSold();
                }
                UpdateText($"{set}({set + sold})");
            }

            return set;
        }
    }
}
