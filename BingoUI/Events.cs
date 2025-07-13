using System;

namespace BingoUI
{
    internal static class Events
    {
        private static event Action<PlayMakerFSM> _onFsmEnable;
        public static event Action<PlayMakerFSM> OnFsmEnable
        {
            add
            {
                if (_onFsmEnable == null) On.PlayMakerFSM.OnEnable += ModifyFsm;
                _onFsmEnable += value;
            }
            remove
            {
                _onFsmEnable -= value;
                if (_onFsmEnable == null) On.PlayMakerFSM.OnEnable -= ModifyFsm;
            }
        }

        private static void ModifyFsm(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);
            _onFsmEnable?.Invoke(self);
        }
    }
}
