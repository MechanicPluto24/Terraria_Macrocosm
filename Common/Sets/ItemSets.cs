using Macrocosm.Common.DataStructures;
using Macrocosm.Content.Liquids;
using Terraria;
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
        /// <summary> Set of items that can be burned in Burners. See <see cref="DataStructures.FuelData"/> </summary>
        public static FuelData[] FuelData { get; } = ItemID.Sets.Factory.CreateCustomSet(defaultState: new FuelData(),

            ItemID.Book, new FuelData(FuelPotency.VeryLow, 60),
            ItemID.Wood, new FuelData(FuelPotency.Low, 80),
            ItemID.Coal, new FuelData(FuelPotency.High, 240)
        );

        /// <summary> Set of items from which liquid can be extracted (e.g. Oil Shale). See <see cref="DataStructures.LiquidExtractData"/></summary>
        public static LiquidExtractData[] LiquidExtractData { get; } = ItemID.Sets.Factory.CreateCustomSet(defaultState: new LiquidExtractData());

        public static LiquidContainerData[] LiquidContainerData { get; } = ItemID.Sets.Factory.CreateCustomSet(defaultState: new LiquidContainerData(), 
            ItemID.EmptyBucket, DataStructures.LiquidContainerData.CreateEmpty(10),
            ItemID.WaterBucket, new LiquidContainerData(LiquidType.Water, 10, ItemID.EmptyBucket),
            ItemID.LavaBucket, new LiquidContainerData(LiquidType.Lava, 10, ItemID.EmptyBucket),
            ItemID.HoneyBucket, new LiquidContainerData(LiquidType.Honey, 10, ItemID.EmptyBucket),
            ItemID.BottomlessBucket, DataStructures.LiquidContainerData.CreateInfinite(LiquidType.Water),
            ItemID.BottomlessLavaBucket, DataStructures.LiquidContainerData.CreateInfinite(LiquidType.Lava),
            ItemID.BottomlessHoneyBucket, DataStructures.LiquidContainerData.CreateInfinite(LiquidType.Honey),
            ItemID.BottomlessShimmerBucket, DataStructures.LiquidContainerData.CreateInfinite(LiquidType.Shimmer)
        );

        /// <summary> Set of items wiith custom potion sickness duration </summary>
        public static int[] PotionDelay { get; } = ItemID.Sets.Factory.CreateIntSet(defaultState: 0);

        /// <summary> 
        /// If true, explosives shot by this item deal damage to the owner. Defaults to true, letting the vanilla player hurt code run. 
        /// <br/> Set to false to disable self damage for explosives shot by this weapon in regular gameplay (not FTW).
        /// </summary>
        public static bool[] ExplosivesShotDealDamageToOwner { get; } = ItemID.Sets.Factory.CreateBoolSet(true);

        /// <summary> 
        /// If true, explosives shot by this item deal damage to the owner. Defaults to true, letting the vanilla player hurt code run. 
        /// <br/> Set to false to disable self damage for explosives shot by this weapon in For The Worthy worlds.
        /// </summary>
        public static bool[] ExplosivesShotDealDamageToOwner_GetGoodWorld { get; } = ItemID.Sets.Factory.CreateBoolSet(true);

        /// <summary> Unobtainable developer/debug items. Items in this set have a "Developer Item" tooltip  </summary>
        public static bool[] DeveloperItem { get; } = ItemID.Sets.Factory.CreateBoolSet();

        /// <summary> Set of seeds for alchemy or similar plants </summary>
        public static bool[] PlantSeed { get; } = ItemID.Sets.Factory.CreateBoolSet
        (
           ItemID.DaybloomSeeds, 
           ItemID.BlinkrootSeeds,
           ItemID.MoonglowSeeds, 
           ItemID.WaterleafSeeds,
           ItemID.ShiverthornSeeds, 
           ItemID.DeathweedSeeds,
           ItemID.FireblossomSeeds,
           ItemID.PumpkinSeed
        );

        /// <summary> Set of tree "seeds" (acorns and vanity saplings) </summary>
        public static bool[] TreeSeed { get; } = ItemID.Sets.Factory.CreateBoolSet
        (
            ItemID.Acorn,
            ItemID.VanityTreeSakuraSeed,
            ItemID.VanityTreeYellowWillowSeed
        );

        /// <summary> Chests. Used for the assembly recipe of the Service Module. </summary>
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
