using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;
using Vasi;

namespace BingoUI.Counters {
    public class TollCounter: AbstractCounter {

        public TollCounter(float x, float y, string spriteName) : base(x, y, spriteName) { }

        public override string GetText() => $"{BingoUI.LS.Tolls.Count()}";

        public override void Hook()
        {
            Events.OnFsmEnable += PatchTollFsm;
        }

        private static readonly HashSet<string> FsmNames = new()
        {
            "Toll Machine",
            "Stag Bell",
            "Toll Machine Bench",
        };

        private void PatchTollFsm(PlayMakerFSM fsm)
        {
            if (!FsmNames.Contains(fsm.FsmName))
            {
                return;
            }
            if (fsm.gameObject.name.Contains("Lever"))
            {
                // RG lever has the same FSM, with a YES state
                return;
            }

            // If it doesn't send text to a YN box then it's probably not a paid toll
            FsmState sendText = fsm.GetState("Send Text");
            if (sendText == null) return;

            FsmState yes = fsm.GetState("Yes");
            if (yes == null) return;

            yes.InsertMethod(0, () => OnTollPaid(fsm.gameObject));
        }


        private void OnTollPaid(GameObject go)
        {
            string sceneName = go.scene.name;

            BingoUI.LS.Tolls.Add(sceneName);
            UpdateText();
        }
    }
}
