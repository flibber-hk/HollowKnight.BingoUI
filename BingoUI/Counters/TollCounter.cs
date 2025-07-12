using System.Collections.Generic;
using System.Linq;

namespace BingoUI.Counters {
    public class TollCounter: AbstractCounter {
        private static string cityBenchTollSceneName = ItemChanger.SceneNames.Ruins1_31;
        private static int cityBenchTollPrice = 150;

        public TollCounter(float x, float y, string spriteName) : base(x, y, spriteName) { }

        public override string GetText() => $"{BingoUI.LS.Tolls.Count()}";

        public override void Hook() {
            On.GeoCounter.TakeGeo += OnTollPaid;
        }

        private void OnTollPaid(On.GeoCounter.orig_TakeGeo orig, GeoCounter self, int geo) {
            orig(self, geo);
            string sceneName = GameManager.instance.GetSceneNameString();
            if(!TollScenes.Contains(sceneName))
                return;
            if(sceneName == cityBenchTollSceneName && geo != cityBenchTollPrice)
                return;
            BingoUI.LS.Tolls.Add(sceneName);
            UpdateText();
        }

        private static readonly List<string> TollScenes = new List<string>() {
            ItemChanger.SceneNames.Crossroads_47,
            ItemChanger.SceneNames.Crossroads_49b,
            ItemChanger.SceneNames.Mines_33,
            ItemChanger.SceneNames.Fungus1_31,
            ItemChanger.SceneNames.Fungus1_16_alt,
            ItemChanger.SceneNames.Fungus2_02,
            ItemChanger.SceneNames.Ruins1_29,
            ItemChanger.SceneNames.Ruins1_31,
            ItemChanger.SceneNames.Ruins2_08,
            ItemChanger.SceneNames.Abyss_18,
            ItemChanger.SceneNames.Abyss_22,
            ItemChanger.SceneNames.Fungus3_40,
            ItemChanger.SceneNames.Fungus3_50,
            ItemChanger.SceneNames.Deepnest_09
        };
    }
}
