using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using Modding;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Logger = Modding.Logger;

namespace BingoUI.Board
{
    public class BingoSync : MonoBehaviour
    {
        public static void Setup()
        {
            GameObject go = new GameObject("Bingo Board");
            _instance = go.AddComponent<BingoSync>();
            DontDestroyOnLoad(go);
        }

        public static void Unload()
        {
            Destroy(_instance._canvas);
            Destroy(_instance.gameObject);
        }

        private static BingoSync _instance;
        private NonBouncer _nb;
        private GameObject _canvas;

        private GameObject menu;
        private GameObject roomIDText;
        private string _roomID;

        private GameObject toggleKeyButton;
        private bool setToggle;
        private KeyCode toggle;

        private GameObject board;

        private GameObject[] goals;
        private GameObject[] goalNames;
        private GameObject[,] goalColors;
        private string[] colors;
        private GameObject[] star;

        private const int GOAL_COLOR_DIVISIONS = 20;
        private readonly string[] COLOR_ORDER = { "pink", "red", "orange", "brown", "yellow", "green", "teal", "blue", "navy", "purple" };
        private readonly Color[] HIGHLIGHT_CYCLE = { Color.white, Color.red, Color.green, Color.blue };

        public void Initialize()
        {
            _canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceCamera, new Vector2(1920, 1080));
            UnityEngine.Object.DontDestroyOnLoad(_canvas);

            GameObject go = new GameObject();
            _nb = go.AddComponent<NonBouncer>();
            UnityEngine.Object.DontDestroyOnLoad(go);

            try {
                toggle = (KeyCode) Enum.Parse(typeof(KeyCode), BingoUI.globalSettings.boardToggle);
            } catch {
                toggle = KeyCode.Tab;
            }

            _roomID = sanitizeID(BingoUI.globalSettings.boardUrl);
            BingoUI.globalSettings.boardUrl = _roomID;

            BuildMenu();
            BuildBoard();
        }

        public void BuildMenu()
        {
            menu = CanvasUtil.CreateBasePanel(_canvas, new CanvasUtil.RectData(Vector2.zero, Vector2.zero, new Vector2(0.6f, 0.6f), new Vector2(0.95f, 0.65f)));

            GameObject roomIDBase = CanvasUtil.CreateBasePanel(menu, new CanvasUtil.RectData(Vector2.zero, Vector2.zero, new Vector2(0.005f, 0.005f), new Vector2(0.495f, 0.995f)));
            roomIDText = CanvasUtil.CreateTextPanel(
                roomIDBase,
                $"Room ID: {_roomID}",
                20,
                TextAnchor.MiddleCenter,
                new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one)
            );
            Image bg = roomIDBase.AddComponent<Image>();
            bg.sprite = SpriteLoader.GetSprite("blank_cell");
            bg.preserveAspect = false;

            GameObject pasteIDButtonBase = CanvasUtil.CreateBasePanel(
                menu,
                new CanvasUtil.RectData(Vector2.zero, Vector2.zero, new Vector2(0.505f, 0.005f), new Vector2(0.745f, 0.995f))
            );
            CanvasUtil.CreateButton(
                pasteIDButtonBase,
                i => ChangeBoard(),
                0,
                SpriteLoader.GetSprite("blank_cell"),
                "Paste From Clipboard",
                20,
                TextAnchor.MiddleCenter,
                new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one)
            );

            GameObject toggleKeyButtonBase = CanvasUtil.CreateBasePanel(
                menu,
                new CanvasUtil.RectData(Vector2.zero, Vector2.zero, new Vector2(0.755f, 0.005f), new Vector2(0.995f, 0.995f))
            );
            toggleKeyButton = CanvasUtil.CreateTextPanel(
                toggleKeyButtonBase,
                $"Toggle: {toggle.ToString()}",
                20,
                TextAnchor.MiddleCenter,
                new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one)
            );

            Image img = toggleKeyButtonBase.AddComponent<Image>();
            img.sprite = SpriteLoader.GetSprite("blank_cell");

            Button button = toggleKeyButtonBase.AddComponent<Button>();
            button.targetGraphic = img;
            button.onClick.AddListener(SetToggleKey);
            button.transition = Selectable.Transition.None;
            
            menu.SetActive(false);
        }

        public void BuildBoard()
        {
            board = CanvasUtil.CreateBasePanel(_canvas, new CanvasUtil.RectData(Vector2.zero, Vector2.zero, new Vector2(0.6f, 0.2f), new Vector2(0.95f, 0.6f)));
            
            goals = new GameObject[25];
            goalNames = new GameObject[25];
            goalColors = new GameObject[25,GOAL_COLOR_DIVISIONS];
            colors = new string[25];
            star = new GameObject[25];
            for (int i = 0; i < 25; i++) {
                int r = 4 - (i / 5);
                int c = i % 5;

                CanvasUtil.RectData rect = new CanvasUtil.RectData(Vector2.zero, Vector2.zero, new Vector2(0.2f*c+0.005f, 0.2f*r+0.005f), new Vector2(0.195f + 0.2f*c, 0.195f + 0.2f*r));
                goals[i] = CanvasUtil.CreateBasePanel(board, rect);

                for (int j = 0; j < GOAL_COLOR_DIVISIONS; j++) {
                    goalColors[i,j] = CanvasUtil.CreateImagePanel(
                        goals[i],
                        SpriteLoader.GetSprite("blank_cell"),
                        new CanvasUtil.RectData(Vector2.zero, Vector2.zero, new Vector2(1.0f*j/GOAL_COLOR_DIVISIONS, 0.0f), new Vector2(1.0f*(j+1)/GOAL_COLOR_DIVISIONS, 1.0f))
                    );
                    goalColors[i,j].GetComponent<Image>().preserveAspect = false;
                    colors[i] = "blank";
                }

                goalNames[i] = CanvasUtil.CreateTextPanel(goals[i], "Goal", 16, TextAnchor.MiddleCenter, new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one));
                goalNames[i].AddComponent<Outline>().effectColor = Color.black;
                goalNames[i].GetComponent<Text>().color = Color.white;

                Button highlightButton = goals[i].AddComponent<Button>();
                int temp = i;
                highlightButton.onClick.AddListener(() => CycleTextHighlight(temp));
            }

            board.SetActive(false);
            Logger.Log("[BingoUI] - Bingo board creation done");

            _nb.StartCoroutine(ConstantRefresh(5f));
        }

        public void Update()
        {
            try {
                if (setToggle) {
                    foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode))) {
                        if (Input.GetKeyDown(kc)) {
                            setToggle = false;
                            toggle = kc;
                            BingoUI.globalSettings.boardToggle = kc.ToString();
                            toggleKeyButton.GetComponent<Text>().text = $"Toggle: {BingoUI.globalSettings.boardToggle}";
                            break;
                        }
                    }
                } else {
                    if (Input.GetKeyDown(toggle)) {
                        if (!board.activeSelf || !menu.activeSelf) {
                            _nb.StartCoroutine(RefreshBoard(_roomID));
                            board.SetActive(true);
                            menu.SetActive(true);
                        }
                        else {
                            board.SetActive(false);
                            menu.SetActive(false);
                        }
                    }
                }
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }

        public static BingoSync Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<BingoSync>();

                if (_instance != null) return _instance;

                Logger.LogWarn("Couldn't find BingoSync");

                GameObject go = new GameObject();
                _instance = go.AddComponent<BingoSync>();
                DontDestroyOnLoad(go);

                return _instance;
            }
        }

        public void SetToggleKey()
        {
            setToggle = true;
            toggleKeyButton.GetComponent<Text>().text = "Toggle: PRESS A KEY";
        }

        private IEnumerator ConstantRefresh(float delay)
        {
            while (true) {
                if (board.activeSelf) yield return _nb.StartCoroutine(RefreshBoard(_roomID));
                yield return new WaitForSeconds(delay);
            }
        }

        private void ChangeBoard()
        {
            _roomID = sanitizeID(GUIUtility.systemCopyBuffer);
            BingoUI.globalSettings.boardUrl = _roomID;
            roomIDText.GetComponent<Text>().text = $"Room ID: {_roomID}";
            _nb.StartCoroutine(RefreshBoard(_roomID));
        }

        private IEnumerator RefreshBoard(string roomID)
        {
            using (UnityWebRequest req = UnityWebRequest.Get($"https://bingosync.com/room/{roomID}/board")) {
                yield return req.SendWebRequest();

                if (req.isNetworkError || req.isHttpError) yield break;
                
                string[] req_board = req.downloadHandler.text.Split('}');
                for (int i = 0; i < 25; i++) {
                    string goal_name = substring_between(req_board[i], "\"name\": \"", "\"");

                    int slot = i;
                    if (Int32.TryParse(substring_between(req_board[i], "\"slot\": \"slot", "\""), out int s)) {
                        slot = s-1;
                    }

                    // blue brown green navy orange pink purple red teal yellow
                    string color = substring_between(req_board[i], "\"colors\": \"", "\"");

                    if (color != colors[slot]) {
                        colors[slot] = color;
                        if (color == "blank") {
                            for (int j = 0; j < GOAL_COLOR_DIVISIONS; j++) {
                                goalColors[slot,j].GetComponent<Image>().sprite = SpriteLoader.GetSprite("blank_cell");
                            }
                        } else {
                            string[] c = sortColors(color);

                            // Distribute the colors as evenly as possible
                            int[] n = new int[c.Length];
                            for (int j = 0; j < GOAL_COLOR_DIVISIONS; j++) n[j % c.Length]++;

                            // Repaint the cell with the color split
                            int k = 0;
                            for (int j = 0; j < c.Length; j++) {
                                for (int l = 0; l < n[j]; l++) {
                                    goalColors[slot,k++].GetComponent<Image>().sprite = SpriteLoader.GetSprite($"{c[j]}_cell");
                                }
                            }
                        }
                    }

                    goalNames[slot].GetComponent<Text>().text = GoalShortener.shorten(goal_name);
                }
            }
        }

        private void CycleTextHighlight(int i)
        {
            Text t = goalNames[i].GetComponent<Text>();
            for (int c = 0; c < HIGHLIGHT_CYCLE.Length; c++) {
                if (HIGHLIGHT_CYCLE[c] == t.color) {
                    t.color = HIGHLIGHT_CYCLE[(c + 1) % HIGHLIGHT_CYCLE.Length];
                    return;
                }
            }
            t.color = Color.white;
        }

        private string substring_between(string a, string b, string c)
        {
            string t = a.Substring(a.IndexOf(b) + b.Length);
            return t.Substring(0, t.IndexOf(c));
        }

        private string[] sortColors(string color)
        {
            string[] unsorted = color.Split(' ');

            string[] sorted = new string[unsorted.Length];
            int idx = 0;

            foreach (string ordered_color in COLOR_ORDER) {
                foreach (string c in unsorted) {
                    if (ordered_color == c) {
                        sorted[idx++] = ordered_color;
                        break;
                    }
                }
            }

            return sorted;
        }

        private string sanitizeID(string id)
        {
            if (id.IndexOf("/room/") != -1) {
                id = id.Substring(id.IndexOf("/room/")+6);
            }

            if (id.IndexOf("/") != -1) {
                return id.Substring(0, id.IndexOf("/"));
            }

            return id;
        }

        //private string csrf = "";

        /*private IEnumerator GetCSRF() {
            using (UnityWebRequest req = UnityWebRequest.Get("https://bingosync.com")) {
                yield return req.SendWebRequest();
                csrf = substring_between(req.GetResponseHeader("set-cookie"), "csrftoken=", ";");
                Logger.Log($"Got CSRF token: {csrf}");
            }
        }

        private IEnumerator EnterRoom(string roomID, string pass)
        {
            if (roomID.StartsWith("https://bingosync.com/room/")) {
                if (csrf == "") yield return _nb.StartCoroutine(GetCSRF());

                UnityWebRequest get = UnityWebRequest.Get(roomID);
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

                Logger.Log($"Attempting to enter {roomID} with password {pass}");

                UnityWebRequest req = UnityWebRequest.Post(roomID, form);
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