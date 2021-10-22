using System;
using System.Collections;
using Modding;
using UnityEngine;
using UnityEngine.UI;

namespace BingoUI.Counters
{
    public abstract partial class AbstractCounter
    {
        public float x;
        public float y;
        public bool enabled = true;
        public string spriteName;

        public abstract string GetText();
        public abstract void Hook();

        public AbstractCounter(float x, float y, string spriteName)
        {
            this.spriteName = spriteName;
            if (BingoUI.globalSettings.CounterPositions.TryGetValue(spriteName, out Layout layout))
            {
                this.x = layout.x;
                this.y = layout.y;
                enabled = layout.enabled;
            }
            else
            {
                this.x = x;
                this.y = y;
                BingoUI.globalSettings.CounterPositions.Add(spriteName, new Layout(x, y));
            }
        }

        public void DebugLog(string s)
        {
            Debug.Log($"[BingoUI]:[{spriteName}] - {s}");
        }
        public void Log(string s)
        {
            Modding.Logger.Log($"[BingoUI]:[{spriteName}] - {s}");
        }

        /// <summary>
        /// Set the text displayed by the counter
        /// </summary>
        /// <param name="newText">The new text</param>
        /// <param name="canShow">If this is false, don't show the counter unless it's already active</param>
        /// <param name="forceShow">If this is true, show the counter even if the text didn't change</param>
        public void UpdateText(string newText = null, bool canShow = true, bool forceShow = false)
        {
            if (newText == null) newText = GetText();
            if (textPanel.text == newText && !forceShow) return;
            if (textPanel.text != newText) DebugLog($"Updating: {textPanel.text} => {newText}");
            textPanel.text = newText;
            if (!canShow) return;
            
            if (DateTime.Now >= NextCanvasFade && enabled)
            {
                _coroutineStarter.StartCoroutine(FadeCanvas());
                NextCanvasFade = DateTime.Now.AddSeconds(0.5f);
            }
        }

        #region Canvas
        private static GameObject _canvas;
        protected static NonBouncer _coroutineStarter;

        public static void SetupCanvas()
        {
            _canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceCamera, new Vector2(1920, 1080));
            UnityEngine.Object.DontDestroyOnLoad(_canvas);

            GameObject go = new GameObject();
            _coroutineStarter = go.AddComponent<NonBouncer>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        private CanvasGroup canvasGroup;
        private Text textPanel;
        public DateTime NextCanvasFade = DateTime.MinValue;

        public void SetupCanvasIcon()
        {
            Vector2 anchor = new Vector2(x, y);
            GameObject canvasSprite = CanvasUtil.CreateImagePanel
            (
                _canvas,
                SpriteLoader.GetSprite(spriteName),
                new CanvasUtil.RectData(Vector2.zero, Vector2.zero, anchor, anchor + new Vector2(1f / 15f, 0.1f))
            );

            // Add a canvas group so we can fade it in and out
            canvasGroup = canvasSprite.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            if (!BingoUI.globalSettings.alwaysDisplay) canvasGroup.gameObject.SetActive(false);

            GameObject text = CanvasUtil.CreateTextPanel
            (
                canvasSprite,
                "0",
                23,
                TextAnchor.LowerCenter,
                new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one)
            );
            textPanel = text.GetComponent<Text>();
            textPanel.color = Color.black;
        }

        private IEnumerator FadeCanvas()
        {
            if (BingoUI.globalSettings.alwaysDisplay || BingoUI.globalSettings.neverDisplay) yield break;

            if (!canvasGroup.gameObject.activeSelf) FadeIn();
            yield return new WaitForSeconds(4f);
            FadeOut();
        }
        public void FadeIn()
        {
            _coroutineStarter.StartCoroutine(CanvasUtil.FadeInCanvasGroup(canvasGroup));
        }
        public void FadeOut()
        {
            _coroutineStarter.StartCoroutine(CanvasUtil.FadeOutCanvasGroup(canvasGroup));
        }
        #endregion
    }
}
