using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Modding;
using UnityEngine.SceneManagement;
using BingoUI.Counters;

namespace BingoUI
{
    public partial class BingoUI : Mod, IGlobalSettings<GlobalSettings>, ILocalSettings<SaveSettings>
    {
        internal static BingoUI Instance;

        #region Settings
        public static SaveSettings LS { get; set; } = new SaveSettings();
        public void OnLoadLocal(SaveSettings s) => LS = s;
        public SaveSettings OnSaveLocal() => LS;

        public static GlobalSettings GS { get; set; } = new GlobalSettings();
        public void OnLoadGlobal(GlobalSettings s) => GS = s;
        public GlobalSettings OnSaveGlobal() => GS;
        #endregion

        public override void Initialize()
        {
            Instance = this;

            SpriteLoader.LoadSprites();
            AbstractCounter.SetupCanvas();

            if (ModHooks.GetMod(nameof(ItemChanger.ItemChangerMod)) is Mod)
            {
                Log("Hooking Itemchanger");
                ItemChangerCompatibility.Initialize();
            }

            AbstractCounter.InitializeCounters();

            // Pause Hooks
            On.UIManager.GoToPauseMenu += OnPause;
            On.UIManager.UIClosePauseMenu += OnUnpause;
            On.UIManager.ReturnToMainMenu += OnUnpauseQuitGame;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

            GeoTracker.Hook();
        }


        #region Pause/Unpause
        private IEnumerator OnPause(On.UIManager.orig_GoToPauseMenu orig, UIManager uiManager)
        {
            yield return orig(uiManager);

            if (GS.alwaysDisplay || GS.neverDisplay)
                yield break;

            // Update and display every image
            foreach (AbstractCounter counter in AbstractCounter.Counters)
            {
                counter.UpdateText();
                counter.FadeIn();
            }
        }

        private void OnUnpause(On.UIManager.orig_UIClosePauseMenu origUIClosePauseMenu, UIManager self)
        {
            origUIClosePauseMenu(self);
            if (GS.alwaysDisplay)
                return;

            // Fade all the canvases, which we were displaying due to pause, out
            foreach (AbstractCounter counter in AbstractCounter.Counters)
            {
                counter.FadeOut();
            }
        }

        private IEnumerator OnUnpauseQuitGame(On.UIManager.orig_ReturnToMainMenu origReturnToMainMenu, UIManager self)
        {
            yield return origReturnToMainMenu(self);
            if (GS.alwaysDisplay)
                yield break;

            // Same thing as above, except apparently quitting to menu doesn't count as unpausing
            foreach (AbstractCounter counter in AbstractCounter.Counters)
            {
                counter.FadeOut();
            }
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (!GS.alwaysDisplay)
            {
                // Fade all the canvases out in case any got stuck on from quick pause/unpausing
                foreach (AbstractCounter counter in AbstractCounter.Counters)
                {
                    counter.FadeOut();
                }
            }
        }
        #endregion

        public override string GetVersion() => Vasi.VersionUtil.GetVersion<BingoUI>();
    }
}