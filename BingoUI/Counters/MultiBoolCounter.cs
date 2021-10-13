using System.Collections.Generic;
using Modding;

namespace BingoUI.Counters
{
    public class MultiBoolCounter : AbstractCounter
    {
        private readonly HashSet<string> boolNames;

        public MultiBoolCounter(float x, float y, string spriteName, HashSet<string> boolNames) : base(x, y, spriteName)
        {
            this.boolNames = boolNames;
        }
        public override string GetText()
        {
            int count = 0;
            foreach (string boolName in boolNames)
            {
                if (PlayerData.instance.GetBool(boolName)) count++;
            }
            return $"{count}";
        }
        public override void Hook()
        {
            ModHooks.SetPlayerBoolHook += OnSetBool;
        }

        private bool OnSetBool(string name, bool orig)
        {
            if (!boolNames.Contains(name)) return orig;
            else
            {
                int count = orig ? 1 : 0;
                foreach (string boolName in boolNames)
                {
                    if (boolName != name && PlayerData.instance.GetBool(boolName)) count++;
                }

                UpdateText($"{count}");
            }
            return orig;
        }
    }
}
