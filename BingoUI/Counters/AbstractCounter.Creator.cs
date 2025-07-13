using System.Collections.Generic;
using System.Linq;

namespace BingoUI.Counters
{
    public abstract partial class AbstractCounter
    {
        public static List<AbstractCounter> Counters;

        public static void InitializeCounters()
        {
            Counters = new List<AbstractCounter>()
            {
                // new CounterType(default x position, default y position, sprite name, any other parameters)
                new GrubCounter(0f, 0.01f, "grub"),
                new EnemyCounter(1f / 15f, 0.01f, "devout", "Slash Spider"),
                new GotSoldCounter(2f / 15f, 0.01f, "trinket1", nameof(PlayerData.trinket1), nameof(PlayerData.soldTrinket1)),
                new GotSoldCounter(3f / 15f, 0.01f, "trinket2", nameof(PlayerData.trinket2), nameof(PlayerData.soldTrinket2)),
                new GotSoldCounter(4f / 15f, 0.01f, "trinket3", nameof(PlayerData.trinket3), nameof(PlayerData.soldTrinket3)),
                new GotSoldCounter(5f / 15f, 0.01f, "trinket4", nameof(PlayerData.trinket4), nameof(PlayerData.soldTrinket4)),
                new TrackedGotSoldCounter(6f / 15f, 0.01f, "ore", nameof(PlayerData.ore)),
                new MultiBoolCounter(7f / 15f, 0.01f, "maps", MapNames),
                new CorniferCounter(8f / 15f, 0.01f, "cornifer"),
                new TrackedGotSoldCounter(9f / 15f, 0.01f, "rancidegg", nameof(PlayerData.rancidEggs)),
                new DreamTreeCounter(10f / 15f, 0.01f, "dreamplant"),
                new LifebloodCounter(11f / 15f, 0.01f, "lifeblood"),
                new CharmCounter(12f / 15f, 0.01f, "charms"),
                new MultiBoolCounter(13f / 15f, 0.01f, "pins", PinNames),
                new IntCounter(14f / 15f, 0.01f, "notches", nameof(PlayerData.charmSlots)),
                new EnemyCounter(14f / 15f, 0.12f, "greathusksentry", "Great Shield Zombie"),
                new TollCounter(13f / 15f, 0.12f, "tolls"),
            };

            foreach (AbstractCounter counter in Counters)
            {
                counter.Hook();
                counter.SetupCanvasIcon();
                counter.UpdateText(null, canShow: false);
            }
        }

        public static HashSet<string> MapNames = new()
        {
            nameof(PlayerData.mapAbyss),
            nameof(PlayerData.mapCity),
            nameof(PlayerData.mapCliffs),
            nameof(PlayerData.mapCrossroads),
            nameof(PlayerData.mapDeepnest),
            nameof(PlayerData.mapFogCanyon),
            nameof(PlayerData.mapFungalWastes),
            nameof(PlayerData.mapGreenpath),
            nameof(PlayerData.mapMines),
            nameof(PlayerData.mapOutskirts),
            nameof(PlayerData.mapRestingGrounds),
            nameof(PlayerData.mapRoyalGardens),
            nameof(PlayerData.mapWaterways)
        };

        public static HashSet<string> PinNames = new()
        {
            nameof(PlayerData.hasPinBench),
            nameof(PlayerData.hasPinCocoon),
            nameof(PlayerData.hasPinGhost),
            nameof(PlayerData.hasPinShop),
            nameof(PlayerData.hasPinSpa),
            nameof(PlayerData.hasPinStag),
            nameof(PlayerData.hasPinTram),
            nameof(PlayerData.hasPinDreamPlant)
        };
    }
}
