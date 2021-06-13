using System;
using System.Collections;
using System.Collections.Generic;

namespace BingoUI
{
    public class GoalShortener
    {
        public static Dictionary<string, string> replace = new Dictionary<string, string>() {
            {"Get Coiled Nail (Nail 3)", "Nail 3"},
            {"Use City Crest and ride both large elevators in CoT", "City Crest + CoT Elevators"},
            {"Use City Crest + Ride both CoT large elevators", "City Crest + CoT Elevators"},
            {"Enter the Lifeblood Core room without wearing any Lifeblood charms", "Lifeblood Core room w/o charms"},
            {"Read the mind of the Shade Beast (Shade Cloak holder, req. Awoken DN.)", "(Awoken) Dream Nail Shade Beast"},
            {"Obtain Wayward Compass or Gathering Swarm", "Compass or Swarm"},
            {"Obtain Dream Wielder or Dreamshield", "Dream Wielder or Dreamshield"},
            {"Obtain Flukenest or Fury of the Fallen", "Flukenest or Fury"},
            {"Obtain Grubsong or Grubberfly's Elegy", "Grubsong or Grubberfly"},
            {"Obtain Glowing Womb or Weaversong", "Glowing Womb or Weaversong"},
            {"Obtain Heavy Blow or Steady Body", "Heavy Blow or Steady Body"},
            {"Obtain Hiveblood or Sharp Shadow", "Hiveblood or Sharp Shadow"},
            {"Obtain Longnail or Mark of Pride", "Longnail or MoP"},
            {"Obtain Quick Slash or Nailmaster's Glory", "Quick Slash or NMG"},
            {"Obtain Shaman Stone or Spell Twister", "Shaman or Twister"},
            {"Obtain Sprintmaster or Dashmaster", "Sprint or Dashmaster"},
            {"Obtain Soul Eater or Soul Catcher", "Soul Eater or Catcher"},
            {"Obtain Thorns of Agony or Stalwart Shell", "Thorns or Stalwart Shell"},
            {"Obtain Shape of Unn or Baldur Shell", "Shape of Unn or Baldur Shell"},
            {"Obtain Quick Focus or Deep Focus", "Quick or Deep Focus"},
            {"Talk to Lemm in his shop with Defender's Crest equipped", "Talk to Lemm with DCrest"},
            {"Talk to Lemm with Defender's Crest equipped", "Talk to Lemm with DCrest"},
            {"Check the journal below Stone Sanc. and the one above Mantis Village", "Check Stone Sanc. and MV journals"},
            {"Check the hidden hallownest seal in Deepnest by Mantis Lords", "Check Super Secret Seal"},
            {"Slash / Check the soul totem above the Pilgrim's Way bridge", "Check the Pilgrim's Way Totem"},
            {"Bow to Moss Prophet, dead or alive", "Bow to Moss Prophet"},
            {"Have 3 different maps not counting Dirtmouth or Hive", "Have 3 different maps"},
            {"Hit the Oro scarecrow up until the hoppers spawn", "Spawn the Oro scarecrow hoppers"},

            {"Check / Free all grubs", "Grubs"},
            {"Check/Free all grubs", "Grubs"},
            {"/ Check", ""},
            {"Complete the ", ""},
            {"Defeat ", ""},
            {" different", ""},

            {"Grey Prince Zote", "GPZ"},
            {"Nightmare King Grimm", "NKG"},
            {"Vengefly King", "VFK"},
            {"Massive Moss Charger", "MMC"},
            {"Trial of the Warrior", "Colo 1"},
            {"Trial of the Conqueror", "Colo 2"},

            {"Ancient Basin", "Basin"},
            {"City of Tears", "CoT"},
            {"Forgotten Crossroads", "Xroads"},
            {"Crossroads", "Xroads"},
            {"Crystal Peaks", "Peak"},
            {"Crystal Peak", "Peak"},
            {"Distant Village", "DV"},
            {"Fungal Wastes", "Fungal"},
            {"Howling Cliffs", "Cliffs"},
            {"Kingdom's Edge", "KE"},
            {"Queen's Gardens", "QG"},
            {"Soul Sanctum", "Sanctum"},
            {"Stone Sanctuary", "Stone Sanc."},
            {"Teacher's Archives", "Archives"},
            {"White Palace", "WP"},

            {"six", "6"},
            {"three", "3"},
            {"two", "2"}
        };

        public static string shorten(string goal) {
            foreach (KeyValuePair<string, string> pair in replace) {
                goal = goal.Replace(pair.Key, pair.Value);
            }

            while (goal.IndexOf(" (") != -1) {
                goal = goal.Substring(0, goal.IndexOf(" (")) + goal.Substring(goal.IndexOf(")")+1);
            }
            return goal;
        }
    }
}