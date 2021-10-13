using System.Collections.Generic;
using System.Linq;

namespace BingoUI.Counters
{
    public class DreamTreeCounter : AbstractCounter
    {
        public DreamTreeCounter(float x, float y, string spriteName) : base(x, y, spriteName) { }

        public override string GetText() => $"{PlayerData.instance.GetVariable<List<string>>(nameof(PlayerData.scenesEncounteredDreamPlantC)).Count()}";
        public override void Hook()
        {
            On.GameManager.AddToDreamPlantCList += UpdateDreamTrees;
        }

        private void UpdateDreamTrees(On.GameManager.orig_AddToDreamPlantCList orig, GameManager self)
        {
            orig(self);

            UpdateText($"{PlayerData.instance.GetVariable<List<string>>(nameof(PlayerData.scenesEncounteredDreamPlantC)).Count()}");
        }
    }
}
