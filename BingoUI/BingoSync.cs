using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Modding;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Logger = Modding.Logger;

namespace BingoUI
{
    public class BingoSync : MonoBehaviour
    {
        private GameObject board;
        private CanvasGroup boardGroup;
        private GameObject[] goals;
        private GameObject[] goalNames;
        private string[] colors;
        private Dictionary<string, Sprite> sprites;
        private static string SPRITE_PATH = "BingoUI.Images.Colors.";

        private NonBouncer _nb;
        private GameObject _canvas;
        private string csrf = "";
        private string _url; // TEMPORARY

        public void Initialize(NonBouncer nb, GameObject canvas, string url) {
            _nb = nb;
            _canvas = canvas;
            _url = url;
            sprites = SereCore.ResourceHelper.GetSprites();
            BuildBoard();
        }

        public void BuildBoard()
        {
            board = CanvasUtil.CreateBasePanel(_canvas, new CanvasUtil.RectData(Vector2.zero, Vector2.zero, new Vector2(0.6f, 0.2f), new Vector2(0.9f, 0.6f)));
            board.AddComponent<CanvasGroup>();
            boardGroup = board.GetComponent<CanvasGroup>();

            goals = new GameObject[25];
            goalNames = new GameObject[25];
            colors = new string[25];
            for (int i = 0; i < 25; i++) {
                int r = 4 - (i / 5);
                int c = i % 5;

                CanvasUtil.RectData rect = new CanvasUtil.RectData(Vector2.zero, Vector2.zero, new Vector2(0.2f*c, 0.2f*r), new Vector2(0.2f + 0.2f*c, 0.2f + 0.2f*r));
                goals[i] = CanvasUtil.CreateBasePanel(board, rect);

                Image img = goals[i].AddComponent<Image>();
                img.sprite = sprites[$"{SPRITE_PATH}blank"];
                img.preserveAspect = false;
                colors[i] = "blank";

                goalNames[i] = CanvasUtil.CreateTextPanel(goals[i], "Goal", 18, TextAnchor.MiddleCenter, new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one));
            }

            board.SetActive(false);
            Logger.Log("Bingo board creation done");

            _nb.StartCoroutine(ConstantRefresh(5f));
        }

        public void Update()
        {
            try
            {
                DetectToggle();
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        private void DetectToggle()
        { 
            if (Input.GetKeyDown(KeyCode.Tab)) {
                if (!board.activeSelf) {
                    _nb.StartCoroutine(RefreshBoard(_url));
                    _nb.StartCoroutine(CanvasUtil.FadeInCanvasGroup(boardGroup));
                }
                else _nb.StartCoroutine(CanvasUtil.FadeOutCanvasGroup(boardGroup));
            }
        }

        private string substring_between(string a, string b, string c) {
            string t = a.Substring(a.IndexOf(b) + b.Length);
            return t.Substring(0, t.IndexOf(c));
        }

        private IEnumerator ConstantRefresh(float delay) {
            while (true) {
                if (board.activeSelf) yield return _nb.StartCoroutine(RefreshBoard(_url));
                yield return new WaitForSeconds(delay);
            }
        }

        private IEnumerator RefreshBoard(string url) {
            if (url.StartsWith("https://bingosync.com/room/")) {
                using (UnityWebRequest req = UnityWebRequest.Get($"{url}/board")) {
                    yield return req.SendWebRequest();
                    
                    string[] board = req.downloadHandler.text.Split('}');
                    for (int i = 0; i < 25; i++) {
                        string name = substring_between(board[i], "\"name\": \"", "\"");

                        int slot = i;
                        if (Int32.TryParse(substring_between(board[i], "\"slot\": \"slot", "\""), out int s)) {
                            slot = s-1;
                        }

                        // blue brown green navy orange pink purple red teal yellow
                        string color = substring_between(board[i], "\"colors\": \"", "\"");

                        if (color != colors[i]) {
                            string[] c = color.Split(' ');
                            goals[i].GetComponent<Image>().sprite = sprites[$"{SPRITE_PATH}{c[0]}"];
                            /*for (int j = 0; j < c.Length; j++) {
                                CanvasUtil.RectData rd = new CanvasUtil.RectData(Vector2.zero, Vector2.zero, new Vector2(j * 1.0f / c.Length, 0.0f), new Vector2((j+1) * 1.0f / c.Length, 1.0f));
                                CanvasUtil.CreateImagePanel(colorPanels[j], sprites[$"{SPRITE_PATH}{c[j]}"], rd);
                            }*/

                            colors[i] = color;
                        }

                        goalNames[i].GetComponent<Text>().text = name;
                    }
                }
            }
        }

        /*private IEnumerator GetCSRF() {
            using (UnityWebRequest req = UnityWebRequest.Get("https://bingosync.com")) {
                yield return req.SendWebRequest();
                csrf = substring_between(req.GetResponseHeader("set-cookie"), "csrftoken=", ";");
                Logger.Log($"Got CSRF token: {csrf}");
            }
        }

        private IEnumerator EnterRoom(string url, string pass)
        {
            if (url.StartsWith("https://bingosync.com/room/")) {
                if (csrf == "") yield return _nb.StartCoroutine(GetCSRF());

                UnityWebRequest get = UnityWebRequest.Get(url);
                yield return get.SendWebRequest();

                //Logger.Log(get.downloadHandler.text);

                WWWForm form = new WWWForm();
                form.AddField("encoded_room_uuid", substring_between(get.downloadHandler.text, "name=\"encoded_room_uuid\" value=\"", "\""));
                form.AddField("room_name", substring_between(get.downloadHandler.text, "name=\"room_name\" value=\"", "\""));
                form.AddField("creator_name", substring_between(get.downloadHandler.text, "name=\"creator_name\" value=\"", "\""));
                form.AddField("game_name", substring_between(get.downloadHandler.text, "name=\"game_name\" value=\"", "\""));
                form.AddField("player_name", "BingoUI");
                form.AddField("passphrase", pass);

                Logger.Log(substring_between(get.downloadHandler.text, "name=\"encoded_room_uuid\" value=\"", "\""));
                Logger.Log(substring_between(get.downloadHandler.text, "name=\"room_name\" value=\"", "\""));
                Logger.Log(substring_between(get.downloadHandler.text, "name=\"creator_name\" value=\"", "\""));
                Logger.Log(substring_between(get.downloadHandler.text, "name=\"game_name\" value=\"", "\""));

                Logger.Log($"Attempting to enter {url} with password {pass}");

                UnityWebRequest req = UnityWebRequest.Post(url, form);
                req.SetRequestHeader("cookie", "csrftoken=" + csrf);
                req.SetRequestHeader("X-CSRFToken", csrf);
                yield return req.SendWebRequest();
                
                if (req.isNetworkError || req.isHttpError) {
                    Logger.Log($"{req.error}: {req.downloadHandler.text}");
                } else {
                    Logger.Log(req.downloadHandler.text);
                }
            }
        }*/
    }
}