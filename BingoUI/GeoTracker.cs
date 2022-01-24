using System;
using System.Reflection;
using Vasi;

namespace BingoUI
{
    public static class GeoTracker
    {
        private static readonly Func<GeoCounter, int> GetGeoCounterCurrent;
        static GeoTracker()
        {
            FieldInfo fi = Mirror.GetFieldInfo(typeof(GeoCounter), "counterCurrent", true);
            GetGeoCounterCurrent = (Func<GeoCounter, int>)Mirror.GetGetter<GeoCounter, int>(fi);
        }
        
        internal static void Hook()
        {
            On.GeoCounter.Update += GeoTracker.UpdateGeoText;
            On.GeoCounter.TakeGeo += GeoTracker.CheckGeoSpent;
        }

        private static void CheckGeoSpent(On.GeoCounter.orig_TakeGeo orig, GeoCounter self, int geo)
        {
            orig(self, geo);

            if (GameManager.instance.GetSceneNameString() == ItemChanger.SceneNames.Fungus3_35 && PlayerData.instance.GetBool(nameof(PlayerData.bankerAccountPurchased)))
            {
                return;
            }

            BingoUI.LS.spentGeo += geo;
        }

        private static void UpdateGeoText(On.GeoCounter.orig_Update orig, GeoCounter self)
        {
            orig(self);
            if (BingoUI.GS.showSpentGeo)
            {
                self.geoTextMesh.text = $"{GetGeoCounterCurrent(self)} ({BingoUI.LS.spentGeo} spent)";
            }
        }
    }
}
