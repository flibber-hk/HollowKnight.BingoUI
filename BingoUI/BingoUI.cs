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
        public static SaveSettings localSettings { get; set; } = new SaveSettings();
        public void OnLoadLocal(SaveSettings s) => localSettings = s;
        public SaveSettings OnSaveLocal() => localSettings;

        public static GlobalSettings globalSettings { get; set; } = new GlobalSettings();
        public void OnLoadGlobal(GlobalSettings s) => globalSettings = s;
        public GlobalSettings OnSaveGlobal() => globalSettings;
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

            // GeoTracker stuff. Taken straight up from Serena's code, with no changes, so some of it might be redundant idk
            On.GeoCounter.Update +=  GeoTracker.UpdateGeoText;
            On.GeoCounter.TakeGeo += GeoTracker.CheckGeoSpent;
        }


        #region Pause/Unpause
        private IEnumerator OnPause(On.UIManager.orig_GoToPauseMenu orig, UIManager uiManager)
        {
            yield return orig(uiManager);

            if (globalSettings.alwaysDisplay || globalSettings.neverDisplay)
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
            if (globalSettings.alwaysDisplay)
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
            if (globalSettings.alwaysDisplay)
                yield break;

            // Same thing as above, except apparently quitting to menu doesn't count as unpausing
            foreach (AbstractCounter counter in AbstractCounter.Counters)
            {
                counter.FadeOut();
            }
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (!globalSettings.alwaysDisplay)
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