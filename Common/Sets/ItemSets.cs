using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Players;
using Macrocosm.Content.Liquids;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Sets
{
    /// <summary> Item Sets for special behavior of some Items, useful for crossmod. </summary>
    [ReinitializeDuringResizeArrays]
    public partial class ItemSets
    {
        /// <summary> Items in this set have a "Developer" tooltip and rarity. Used for developer items or other testing items. </summary>
        public static bool[] DeveloperItem { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(DeveloperItem)).Description("Items in this set have a \"Developer\" tooltip and rarity. Used for developer items or other testing items.").RegisterBoolSet();

        /// <summary> Items in this set have an "Unobtainable" tooltip. Used for items that can't yet be obtained legitimately. </summary>
        public static bool[] UnobtainableItem { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(UnobtainableItem)).Description(" Items in this set have an \"Unobtainable\" tooltip. Used for items that can't yet be obtained legitimately.").RegisterBoolSet();

        /// <summary> Items that can't be used on Macrocosm subworlds. Items in this set also have a "This item can not be used on {SubworldName}" tooltip. </summary>
        public static bool[] UnusableItem { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(UnusableItem)).Description("Items that can't be used on Macrocosm subworlds. Items in this set also have a \"This item can not be used on <SubworldName>\" tooltip.").RegisterBoolSet(
            ItemID.BloodMoonStarter,
            ItemID.GoblinBattleStandard,
            ItemID.SnowGlobe,
            ItemID.SolarTablet,
            ItemID.PirateMap,
            ItemID.PumpkinMoonMedallion,
            ItemID.NaughtyPresent,
            ItemID.CelestialSigil
        );

        /// <summary> Torch item types that work in a Macrocosm subworld. </summary>
        public static bool[] AllowedTorches { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(AllowedTorches)).Description("Torch item types that work in a Macrocosm subworld.").RegisterBoolSet();

        /// <summary> The Potion Sickness duration (in ticks) applied when consuming this item. </summary>
        public static int[] PotionDelay { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(PotionDelay)).Description("The Potion Sickness duration (in ticks) applied when consuming this item.").RegisterIntSet(defaultState: 0);

        /// <summary> 
        /// Explosives shot by this item deal damage to the owner. Defaults to true, letting the vanilla player hurt code run. 
        /// <br/> Set to false to disable self damage for explosives shot by this weapon in regular gameplay (not FTW).
        /// </summary>
        public static bool[] ExplosivesShotDealDamageToOwner { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(ExplosivesShotDealDamageToOwner)).Description("Explosives shot by this item deal damage to the owner. Defaults to true, letting the vanilla player hurt code run. Set to false to disable self damage for explosives shot by this weapon in regular gameplay (not FTW).").RegisterBoolSet(defaultState: true);

        /// <summary> 
        /// Explosives shot by this item deal damage to the owner. Defaults to true, letting the vanilla player hurt code run. 
        /// <br/> Set to false to disable self damage for explosives shot by this weapon in For The Worthy worlds.
        /// </summary>
        public static bool[] ExplosivesShotDealDamageToOwner_GetGoodWorld { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(ExplosivesShotDealDamageToOwner_GetGoodWorld)).Description("Explosives shot by this item deal damage to the owner. Defaults to true, letting the vanilla player hurt code run. Set to false to disable self damage for explosives shot by this weapon in For The Worthy worlds.").RegisterBoolSet(defaultState: true);

        /// <summary> Weapon item types that ignore the <see cref="MacrocosmPlayer.ShootSpreadReduction"> mechanics. </summary>
        public static bool[] IgnoresShootSpreadReduction { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(IgnoresShootSpreadReduction)).Description("Weapon item types that ignore the mod's ShootSpreadReduction mechanics.").RegisterBoolSet(defaultState: false);

        /// <summary> Damage adjustment for items while in a Macrocosm subworld. Applies a tooltip indicating if the item is stronger or weaker. </summary>
        public static float[] DamageAdjustment { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(DamageAdjustment)).Description("Damage adjustment for items while in a Macrocosm subworld. Applies a tooltip indicating if the item is stronger or weaker.").RegisterFloatSet(defaultState: 1f,
            ItemID.Zenith, 0.28f,
            ItemID.LastPrism, 0.36f,
            ItemID.StarWrath, 0.63f,
            ItemID.StardustDragonStaff, 0.25f,
            ItemID.Terrarian, 0.45f,
            ItemID.NebulaBlaze, 0.6f,
            ItemID.Meowmere, 0.5f,
            ItemID.Celeb2, 0.28f,
            ItemID.LunarFlareBook, 1.05f,
            ItemID.EmpressBlade, 0.5f,
            ItemID.MoonlordArrow, 0.88f,
            ItemID.RainbowCrystalStaff, 0.3f,
            ItemID.MoonlordTurretStaff, 0.2f,
            ItemID.SDMG, 0.8f,
            ItemID.MiniNukeI, 0.4f,
            ItemID.MiniNukeII, 0.4f,
            ItemID.SolarEruption, 0.78f,
            ItemID.DayBreak, 0.67f,
            ItemID.VortexBeater, 0.9f,
            ItemID.Phantasm, 0.9f,
            ItemID.StardustCellStaff, 0.9f
        );

        /// <summary> Whether this Wing accessory's flight time is affected by the Macrocosm subworld Atmospheric Density. </summary>
        public static bool[] WingTimeDependsOnAtmosphericDensity { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(WingTimeDependsOnAtmosphericDensity)).Description("Whether this Wing accessory's flight time is affected by the Macrocosm subworld Atmospheric Density.").RegisterBoolSet(
            ItemID.CreativeWings,
            ItemID.AngelWings,
            ItemID.DemonWings,
            ItemID.FairyWings,
            ItemID.FinWings,
            ItemID.FrozenWings,
            ItemID.HarpyWings,
            ItemID.RedsWings,
            ItemID.DTownsWings,
            ItemID.WillsWings,
            ItemID.CrownosWings,
            ItemID.CenxsWings,
            ItemID.BejeweledValkyrieWing,
            ItemID.Yoraiz0rWings,
            ItemID.JimsWings,
            ItemID.SkiphsWings,
            ItemID.LokisWings,
            ItemID.ArkhalisWings,
            ItemID.LeinforsWings,
            ItemID.GhostarsWings,
            ItemID.SafemanWings,
            ItemID.FoodBarbarianWings,
            ItemID.GroxTheGreatWings,
            ItemID.LeafWings,
            ItemID.BatWings,
            ItemID.BeeWings,
            ItemID.ButterflyWings,
            ItemID.FlameWings,
            ItemID.GhostWings,
            ItemID.BoneWings,
            ItemID.MothronWings,
            ItemID.GhostWings,
            ItemID.BeetleWings,
            ItemID.FestiveWings,
            ItemID.TatteredFairyWings,
            ItemID.SteampunkWings,
            ItemID.BetsyWings,
            ItemID.RainbowWings,
            ItemID.FishronWings,
            ItemID.WingsNebula,
            ItemID.WingsVortex,
            ItemID.WingsSolar,
            ItemID.WingsStardust
        );

        /// <summary> The flight time multiplier of this Wing accesory on Macrocosm subworlds. </summary>
        public static float[] WingTimeMultiplier { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(WingTimeMultiplier)).Description("The flight time multiplier of this Wing accesory on Macrocosm subworlds.").RegisterFloatSet(defaultState: 1f,
            ItemID.LongRainbowTrailWings, 0.5f,
            ItemID.Hoverboard, 0.3f,
            ItemID.Jetpack, 0.3f
        );

        /// <summary> Items that are seeds that place alchemy or similar plants. </summary>
        public static bool[] PlantSeed { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(PlantSeed)).Description("Items that are seeds that place alchemy or similar plants.").RegisterBoolSet(
           ItemID.DaybloomSeeds,
           ItemID.BlinkrootSeeds,
           ItemID.MoonglowSeeds,
           ItemID.WaterleafSeeds,
           ItemID.ShiverthornSeeds,
           ItemID.DeathweedSeeds,
           ItemID.FireblossomSeeds,
           ItemID.PumpkinSeed
        );

        /// <summary> Items that place saplings (acorns, vanity tree saplings). </summary>
        public static bool[] TreeSeed { get; } = ItemID.Sets.Factory.CreateNamedSet(nameof(TreeSeed)).Description("Items that place saplings (acorns, vanity tree saplings).").RegisterBoolSet(
            ItemID.Acorn,
            ItemID.VanityTreeSakuraSeed,
            ItemID.VanityTreeYellowWillowSeed
        );

        public static TrashData[] TrashData { get; } = ItemID.Sets.Factory.CreateCustomSet(defaultState: new TrashData(),
            ItemID.Wood, new TrashData(ItemID.Wood, 7),
            ItemID.Gel, new TrashData(ItemID.Gel, DustID.TintableDust, color: new Color(58, 114, 255)),
            ItemID.CopperCoin, new TrashData(ItemID.CopperCoin, DustID.Copper),
            ItemID.IronHelmet, new TrashData(ItemID.IronHelmet, DustID.Iron),
            ItemID.AshBlock, new TrashData(ItemID.AshBlock, DustID.Ash)
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
    }
}
