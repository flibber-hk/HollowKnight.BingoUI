using System;
using ItemChanger;

namespace BingoUI
{
    public static class ItemChangerCompatibility
    {
        public static event Action<string> OnVisitLocation;
        public static event Action<string> OnObtainLocation;
        public static event Action<string> OnObtainItem;

        public static void Initialize()
        {
            AbstractItem.AfterGiveGlobal += ItemChanger_AfterGiveGlobal;
            AbstractPlacement.OnVisitStateChangedGlobal += ItemChanger_OnVisitStateChangedGlobal;
        }

        private static void ItemChanger_OnVisitStateChangedGlobal(VisitStateChangedEventArgs obj)
        {
            if (obj.NewFlags.HasFlag(VisitState.Previewed) && !obj.Orig.HasFlag(VisitState.Previewed))
            {
                OnVisitLocation?.Invoke(obj.Placement.Name);
            }
            if (obj.NewFlags.HasFlag(VisitState.ObtainedAnyItem) && !obj.Orig.HasFlag(VisitState.ObtainedAnyItem))
            {
                OnObtainLocation?.Invoke(obj.Placement.Name);
            }
        }

        private static void ItemChanger_AfterGiveGlobal(ReadOnlyGiveEventArgs obj)
        {
            OnObtainItem?.Invoke(obj.Item.name);
        }
    }
}