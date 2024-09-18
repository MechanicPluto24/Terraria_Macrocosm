using Macrocosm.Common.DataStructures;
using Terraria.ID;

namespace Macrocosm.Common.Sets
{
    /// <summary>
    /// Item Sets for special behavior of some Items, useful for crossmod.
    /// Note: Only initalize sets with vanilla content here, add modded content to sets in SetStaticDefaults.
    /// </summary>
    public class ItemSets
    {
        // TODO: complete this
        /// <summary> Set of items of their <see cref="DataStructures.FuelData"/>, for use in burner machines.  </summary>
        public static FuelData[] FuelData { get; } = ItemID.Sets.Factory.CreateCustomSet(defaultState: new FuelData(),

            ItemID.Book, new FuelData(FuelPotency.VeryLow, 60),
            ItemID.Wood, new FuelData(FuelPotency.Low, 80),
            ItemID.Coal, new FuelData(FuelPotency.High, 240)
        );

        public static LiquidExtractData[] LiquidExtractData { get; } = ItemID.Sets.Factory.CreateCustomSet(defaultState: new LiquidExtractData());

        public static int[] PotionDelay { get; } = ItemID.Sets.Factory.CreateIntSet(defaultState: 0);

        public static bool[] DeveloperItem { get; } = ItemID.Sets.Factory.CreateBoolSet();

        /// <summary> 
        /// Chests.
        /// Used for the assembly recipe of the Service Module. 
        /// </summary>
        public static bool[] Chest { get; } = ItemID.Sets.Factory.CreateBoolSet
        (
            ItemID.Chest,
            ItemID.GoldChest,
            ItemID.FrozenChest,
            ItemID.IvyChest,
            ItemID.LihzahrdChest,
            ItemID.LivingWoodChest,
            ItemID.MushroomChest,
            ItemID.RichMahoganyChest,
            ItemID.DesertChest,
            ItemID.SkywareChest,
            ItemID.WaterChest,
            ItemID.WebCoveredChest,
            ItemID.GraniteChest,
            ItemID.MarbleChest,
            ItemID.ShadowChest,
            ItemID.GoldenChest,
            ItemID.CorruptionChest,
            ItemID.CrimsonChest,
            ItemID.HallowedChest,
            ItemID.IceChest,
            ItemID.JungleChest,
            ItemID.DungeonDesertChest,
            ItemID.GolfChest,
            ItemID.NebulaChest,
            ItemID.SolarChest,
            ItemID.StardustChest,
            ItemID.VortexChest,
            ItemID.BoneChest,
            ItemID.LesionChest,
            ItemID.FleshChest,
            ItemID.GlassChest,
            ItemID.HoneyChest,
            ItemID.SlimeChest,
            ItemID.SteampunkChest,
            ItemID.AshWoodChest,
            ItemID.BalloonChest,
            ItemID.BambooChest,
            ItemID.BlueDungeonChest,
            ItemID.BorealWoodChest,
            ItemID.CactusChest,
            ItemID.CrystalChest,
            ItemID.DynastyChest,
            ItemID.EbonwoodChest,
            ItemID.GreenDungeonChest,
            ItemID.MartianChest,
            ItemID.MeteoriteChest,
            ItemID.ObsidianChest,
            ItemID.PalmWoodChest,
            ItemID.PinkDungeonChest,
            ItemID.PumpkinChest,
            ItemID.CoralChest,
            ItemID.ShadewoodChest,
            ItemID.SpiderChest,
            ItemID.SpookyChest
        );
    }
}
