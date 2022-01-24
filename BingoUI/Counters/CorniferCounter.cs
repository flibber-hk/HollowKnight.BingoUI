using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vasi;

namespace BingoUI.Counters
{
    public class CorniferCounter : AbstractCounter
    {
        public CorniferCounter(float x, float y, string spriteName) : base(x, y, spriteName) { }

        public override string GetText() => $"{BingoUI.LS.Cornifers.Count()}";

        public override void Hook()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += PatchCornifer;
            ItemChangerCompatibility.OnVisitLocation += OnVisitCornifer;
        }

        private void PatchCornifer(Scene arg0, LoadSceneMode arg1)
        {
            _coroutineStarter.StartCoroutine(PatchCorniferDelay());
        }

        private IEnumerator PatchCorniferDelay()
        {
            yield return null;
            // Finds all kinds of cornifer locations in a possible scene and patches the FSM to increase the cornifer locations counter
            GameObject[] cornifers = new GameObject[3];
            cornifers[0] = GameObject.Find("Cornifer");
            cornifers[1] = GameObject.Find("Cornifer Deepnest"); // Why is this a separete object TC
            cornifers[2] = GameObject.Find("Cornifer Card");

            foreach (GameObject cornifer in cornifers)
            {
                if (cornifer == null) continue;
                PlayMakerFSM fsm = cornifer.LocateMyFSM("Conversation Control");
                fsm.GetState("Box Up").AddMethod(() =>
                {
                    string sceneName = GameManager.instance.sceneName;
                    BingoUI.LS.Cornifers.Add(sceneName);
                    UpdateText();
                });
            }
        }

        private void OnVisitCornifer(string location)
        {
            if (CorniferLocations.TryGetValue(location, out string sceneName))
            {
                BingoUI.LS.Cornifers.Add(sceneName);
                UpdateText();
            }
        }

        private static readonly Dictionary<string, string> CorniferLocations = new Dictionary<string, string>()
        {
            [ItemChanger.LocationNames.Crossroads_Map] = ItemChanger.SceneNames.Crossroads_33,
            [ItemChanger.LocationNames.Greenpath_Map] = ItemChanger.SceneNames.Fungus1_06,
            [ItemChanger.LocationNames.Fog_Canyon_Map] = ItemChanger.SceneNames.Fungus3_25,
            [ItemChanger.LocationNames.Fungal_Wastes_Map] = ItemChanger.SceneNames.Fungus2_18,
            [ItemChanger.LocationNames.Deepnest_Map_Right] = ItemChanger.SceneNames.Fungus2_25,
            [ItemChanger.LocationNames.Deepnest_Map_Upper] = ItemChanger.SceneNames.Deepnest_01b,
            [ItemChanger.LocationNames.Ancient_Basin_Map] = ItemChanger.SceneNames.Abyss_04,
            [ItemChanger.LocationNames.Kingdoms_Edge_Map] = ItemChanger.SceneNames.Deepnest_East_03,
            [ItemChanger.LocationNames.City_of_Tears_Map] = ItemChanger.SceneNames.Ruins1_31,
            [ItemChanger.LocationNames.Royal_Waterways_Map] = ItemChanger.SceneNames.Waterways_09,
            [ItemChanger.LocationNames.Howling_Cliffs_Map] = ItemChanger.SceneNames.Cliffs_01,
            [ItemChanger.LocationNames.Crystal_Peak_Map] = ItemChanger.SceneNames.Mines_30,
            [ItemChanger.LocationNames.Queens_Gardens_Map] = ItemChanger.SceneNames.Fungus1_24,
            [ItemChanger.LocationNames.Resting_Grounds_Map] = ItemChanger.SceneNames.RestingGrounds_09
        };
    }
}
