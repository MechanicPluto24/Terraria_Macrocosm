using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Liquids;
using Microsoft.Xna.Framework;
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
        /// <summary> Uned for unobtainable testing items and other developer items. Items in this set have a "Developer" tooltip and rarity </summary>
        public static bool[] DeveloperItem { get; } = ItemID.Sets.Factory.CreateBoolSet();

        /// <summary> Unobtainable items, other than Developer Items meant for testing. Items in this set have an "Unobtainable" tooltip </summary>
        public static bool[] UnobtainableItem { get; } = ItemID.Sets.Factory.CreateBoolSet();

        /// <summary> Items that can't be used on Macrocosm subworlds. Items in this set also have a tooltip to tell the player they can't the item </summary>
        public static bool[] UnusableItems { get; } = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.BloodMoonStarter,
            ItemID.GoblinBattleStandard,
            ItemID.SnowGlobe,
            ItemID.SolarTablet,
            ItemID.PirateMap,
            ItemID.PumpkinMoonMedallion,
            ItemID.NaughtyPresent,
            ItemID.CelestialSigil
        );

        /// <summary> Set of torches that work in a Macrocosm subworld </summary>
        public static bool[] AllowedTorches { get; } = ItemID.Sets.Factory.CreateBoolSet();

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

        /// <summary> Set of items that can be burned in Burners. See <see cref="DataStructures.FuelData"/> </summary>
        public static FuelData[] FuelData { get; } = ItemID.Sets.Factory.CreateCustomSet(defaultState: new FuelData(),

            // Special
            ItemID.GuideVoodooDoll, new FuelData(FuelPotency.Low, 600)
            {
                Burning = (item, pos) => Utility.BurnTownNPC(NPCID.Guide),
                OnConsumed = (item, pos) => Utility.SimulateGuideVoodooDollBurn(pos),
            },

            ItemID.ClothierVoodooDoll, new FuelData(FuelPotency.Low, 600)
            {
                Burning = (item, pos) => Utility.BurnTownNPC(NPCID.Clothier),
                OnConsumed = (item, pos) => NPC.SpawnSkeletron(Utility.GetClosestPlayer(pos, 1, 1).whoAmI),
            },

            ItemID.EmpressButterfly, new FuelData(FuelPotency.Low, 600, critter: true)
            {
                OnConsumed = (item, pos) => Utility.SpawnAndKillNPC(null, pos, NPCID.EmpressButterfly, noPlayerInteraction: false),
            },

            ItemID.LadyBug, new FuelData(FuelPotency.Low, 100, critter: true)
            {
                OnConsumed = (item, pos) => NPC.LadyBugKilled(pos),
            },

            ItemID.GoldLadyBug, new FuelData(FuelPotency.Low, 100, critter: true)
            {
                OnConsumed = (item, pos) => NPC.LadyBugKilled(pos, GoldLadyBug: true),
            },

            // Misc
            ItemID.Coal, new FuelData(FuelPotency.High, 240),
            ItemID.Cobweb, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.Book, new FuelData(FuelPotency.Low, 120),
            ItemID.Silk, new FuelData(FuelPotency.Low, 60),
            ItemID.Feather, new FuelData(FuelPotency.Low, 80),
            ItemID.TatteredCloth, new FuelData(FuelPotency.Low, 80),
            ItemID.Hay, new FuelData(FuelPotency.Medium, 40),
            ItemID.Seaweed, new FuelData(FuelPotency.VeryLow, 140),
            ItemID.Hive, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.Confetti, new FuelData(FuelPotency.Low, 40),
            ItemID.Present, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.Sail, new FuelData(FuelPotency.Low, 80),
            ItemID.SpiderBlock, new FuelData(FuelPotency.Low, 40),
            ItemID.FragmentSolar, new FuelData(FuelPotency.SolarFragmentHigh, 240),

            // Wood
            ItemID.Wood, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahogany, new FuelData(FuelPotency.Low, 80),
            ItemID.Ebonwood, new FuelData(FuelPotency.Low, 80),
            ItemID.Shadewood, new FuelData(FuelPotency.Low, 80),
            ItemID.Pearlwood, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWood, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWood, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastyWood, new FuelData(FuelPotency.Low, 80),
            ItemID.SpookyWood, new FuelData(FuelPotency.Low, 80),
            ItemID.AshWood, new FuelData(FuelPotency.VeryLow, 120),

            // Gel & slime
            ItemID.Gel, new FuelData(FuelPotency.Medium, 60),
            ItemID.PinkGel, new FuelData(FuelPotency.High, 60),
            ItemID.SlimeBlock, new FuelData(FuelPotency.Medium, 60),
            ItemID.FrozenSlimeBlock, new FuelData(FuelPotency.Medium, 120),
            ItemID.PinkSlimeBlock, new FuelData(FuelPotency.High, 60),

            // Shrooms
            ItemID.Mushroom, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.GlowingMushroom, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.VileMushroom, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.ViciousMushroom, new FuelData(FuelPotency.VeryLow, 80),

            // Alch plants
            ItemID.Daybloom, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.Blinkroot, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.Moonglow, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.Waterleaf, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.Deathweed, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.Shiverthorn, new FuelData(FuelPotency.VeryLow, 100),
            ItemID.Fireblossom, new FuelData(FuelPotency.Medium, 80),

            // Other plants & seeds
            ItemID.Sunflower, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.Cactus, new FuelData(FuelPotency.Low, 80),
            ItemID.Pumpkin, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.PumpkinSeed, new FuelData(FuelPotency.VeryLow, 40),

            // Grass seeds
            ItemID.GrassSeeds, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.JungleGrassSeeds, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.MushroomGrassSeeds, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.AshGrassSeeds, new FuelData(FuelPotency.VeryLow, 40),

            // Alch seeds
            ItemID.DaybloomSeeds, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.BlinkrootSeeds, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.MoonglowSeeds, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.WaterleafSeeds, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.ShiverthornSeeds, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.DeathweedSeeds, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.FireblossomSeeds, new FuelData(FuelPotency.VeryLow, 40),
            ItemID.PumpkinSeed, new FuelData(FuelPotency.VeryLow, 40),

            // Dye plants
            ItemID.GreenMushroom, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.TealMushroom, new FuelData(FuelPotency.VeryLow, 80),

            // Tree seeds
            ItemID.Acorn, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.VanityTreeSakuraSeed, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.VanityTreeYellowWillowSeed, new FuelData(FuelPotency.VeryLow, 80),

            // Threads
            ItemID.BlackThread, new FuelData(FuelPotency.Low, 60),
            ItemID.PinkThread, new FuelData(FuelPotency.Low, 60),
            ItemID.GreenThread, new FuelData(FuelPotency.Low, 60),

            // Torches
            ItemID.Torch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.BlueTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.RedTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.GreenTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.PurpleTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.WhiteTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.YellowTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.CursedTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.OrangeTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.IchorTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.UltrabrightTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.BoneTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.RainbowTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.PinkTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.DesertTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.CoralTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.CorruptTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.CrimsonTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.HallowedTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.JungleTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.MushroomTorch, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.ShimmerTorch, new FuelData(FuelPotency.VeryLow, 80),

            // Vines & Ropes
            ItemID.Rope, new FuelData(FuelPotency.VeryLow, 100),
            ItemID.VineRope, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.SilkRope, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.WebRope, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.RopeCoil, new FuelData(FuelPotency.VeryLow, 140),
            ItemID.VineRopeCoil, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.WebRopeCoil, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.SilkRopeCoil, new FuelData(FuelPotency.VeryLow, 120),

            // Fire & Lava themed 
            ItemID.Hellstone, new FuelData(FuelPotency.High, 120),
            ItemID.HellstoneBar, new FuelData(FuelPotency.High, 120),
            ItemID.LavaBucket, new FuelData(FuelPotency.High, 120),
            ItemID.LivingFireBlock, new FuelData(FuelPotency.Medium, 40),
            ItemID.LivingCursedFireBlock, new FuelData(FuelPotency.Medium, 40),
            ItemID.LivingDemonFireBlock, new FuelData(FuelPotency.Medium, 40),
            ItemID.LivingUltrabrightFireBlock, new FuelData(FuelPotency.Medium, 40),
            ItemID.MagicLavaDropper, new FuelData(FuelPotency.VeryLow, 120),

            // Evil
            ItemID.RottenChunk, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.CursedFlame, new FuelData(FuelPotency.High, 40),
            ItemID.TissueSample, new FuelData(FuelPotency.Medium, 80),
            ItemID.Vertebrae, new FuelData(FuelPotency.Low, 100),
            ItemID.FleshBlock, new FuelData(FuelPotency.Low, 120),
            ItemID.LesionBlock, new FuelData(FuelPotency.VeryLow, 120),

            // Wooden furniture
            ItemID.WoodPlatform, new FuelData(FuelPotency.Low, 80),
            ItemID.WoodenChair, new FuelData(FuelPotency.Low, 80),
            ItemID.WoodenDoor, new FuelData(FuelPotency.Low, 80),
            ItemID.WoodenTable, new FuelData(FuelPotency.Low, 80),
            ItemID.WorkBench, new FuelData(FuelPotency.Low, 80),
            ItemID.WoodenSink, new FuelData(FuelPotency.Low, 80),
            ItemID.Bed, new FuelData(FuelPotency.Low, 80),
            ItemID.Bench, new FuelData(FuelPotency.Low, 80),
            ItemID.Bookcase, new FuelData(FuelPotency.Low, 80),
            ItemID.Dresser, new FuelData(FuelPotency.Low, 80),
            ItemID.GrandfatherClock, new FuelData(FuelPotency.Low, 80),
            ItemID.Piano, new FuelData(FuelPotency.Low, 80),
            ItemID.Sofa, new FuelData(FuelPotency.Low, 80),
            ItemID.Chest, new FuelData(FuelPotency.Low, 80),

            // Misc wooden stuff
            ItemID.WoodenBeam, new FuelData(FuelPotency.Low, 80),
            ItemID.WoodWall, new FuelData(FuelPotency.Low, 80),
            ItemID.WoodenFence, new FuelData(FuelPotency.Low, 80),
            ItemID.GraveMarker, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.CrossGraveMarker, new FuelData(FuelPotency.VeryLow, 80),
            ItemID.WoodenSpike, new FuelData(FuelPotency.Low, 80),
            ItemID.BambooBlock, new FuelData(FuelPotency.Low, 80),
            ItemID.PineTreeBlock, new FuelData(FuelPotency.Low, 120),
            ItemID.WoodenCrate, new FuelData(FuelPotency.Medium, 80),
            ItemID.WoodFishingPole, new FuelData(FuelPotency.Medium, 80),
            ItemID.WoodenBoomerang, new FuelData(FuelPotency.Medium, 80),
            ItemID.WoodenArrow, new FuelData(FuelPotency.Medium, 80),
            ItemID.WoodenBow, new FuelData(FuelPotency.Medium, 80),
            ItemID.WoodenHammer, new FuelData(FuelPotency.Medium, 80),
            ItemID.WoodenSword, new FuelData(FuelPotency.Medium, 80),
            ItemID.WoodYoyo, new FuelData(FuelPotency.Medium, 80),
            ItemID.WoodHelmet, new FuelData(FuelPotency.Medium, 80),
            ItemID.WoodBreastplate, new FuelData(FuelPotency.Medium, 80),
            ItemID.WoodGreaves, new FuelData(FuelPotency.Medium, 80),

            ItemID.RichMahoganyBow, new FuelData(FuelPotency.Medium, 80),
            ItemID.RichMahoganyHammer, new FuelData(FuelPotency.Medium, 80),
            ItemID.RichMahoganySword, new FuelData(FuelPotency.Medium, 80),
            ItemID.RichMahoganyHelmet, new FuelData(FuelPotency.Medium, 80),
            ItemID.RichMahoganyBreastplate, new FuelData(FuelPotency.Medium, 80),
            ItemID.RichMahoganyGreaves, new FuelData(FuelPotency.Medium, 80),

            ItemID.EbonwoodBow, new FuelData(FuelPotency.Medium, 80),
            ItemID.EbonwoodHammer, new FuelData(FuelPotency.Medium, 80),
            ItemID.EbonwoodSword, new FuelData(FuelPotency.Medium, 80),
            ItemID.EbonwoodHelmet, new FuelData(FuelPotency.Medium, 80),
            ItemID.EbonwoodBreastplate, new FuelData(FuelPotency.Medium, 80),
            ItemID.EbonwoodGreaves, new FuelData(FuelPotency.Medium, 80),

            ItemID.ShadewoodBow, new FuelData(FuelPotency.Medium, 80),
            ItemID.ShadewoodHammer, new FuelData(FuelPotency.Medium, 80),
            ItemID.ShadewoodSword, new FuelData(FuelPotency.Medium, 80),
            ItemID.ShadewoodHelmet, new FuelData(FuelPotency.Medium, 80),
            ItemID.ShadewoodBreastplate, new FuelData(FuelPotency.Medium, 80),
            ItemID.ShadewoodGreaves, new FuelData(FuelPotency.Medium, 80),

            ItemID.PearlwoodBow, new FuelData(FuelPotency.Medium, 80),
            ItemID.PearlwoodHammer, new FuelData(FuelPotency.Medium, 80),
            ItemID.PearlwoodSword, new FuelData(FuelPotency.Medium, 80),
            ItemID.PearlwoodHelmet, new FuelData(FuelPotency.Medium, 80),
            ItemID.PearlwoodBreastplate, new FuelData(FuelPotency.Medium, 80),
            ItemID.PearlwoodGreaves, new FuelData(FuelPotency.Medium, 80),

            ItemID.BorealWoodBow, new FuelData(FuelPotency.Medium, 80),
            ItemID.BorealWoodHammer, new FuelData(FuelPotency.Medium, 80),
            ItemID.BorealWoodSword, new FuelData(FuelPotency.Medium, 80),
            ItemID.BorealWoodHelmet, new FuelData(FuelPotency.Medium, 80),
            ItemID.BorealWoodBreastplate, new FuelData(FuelPotency.Medium, 80),
            ItemID.BorealWoodGreaves, new FuelData(FuelPotency.Medium, 80),

            ItemID.PalmWoodBow, new FuelData(FuelPotency.Medium, 80),
            ItemID.PalmWoodHammer, new FuelData(FuelPotency.Medium, 80),
            ItemID.PalmWoodSword, new FuelData(FuelPotency.Medium, 80),
            ItemID.PalmWoodHelmet, new FuelData(FuelPotency.Medium, 80),
            ItemID.PalmWoodBreastplate, new FuelData(FuelPotency.Medium, 80),
            ItemID.PalmWoodGreaves, new FuelData(FuelPotency.Medium, 80),

            ItemID.SpookyHelmet, new FuelData(FuelPotency.Medium, 80),
            ItemID.SpookyBreastplate, new FuelData(FuelPotency.Medium, 80),
            ItemID.SpookyLeggings, new FuelData(FuelPotency.Medium, 80),

            ItemID.AshWoodBow, new FuelData(FuelPotency.Low, 120),
            ItemID.AshWoodHammer, new FuelData(FuelPotency.Low, 120),
            ItemID.AshWoodSword, new FuelData(FuelPotency.Low, 120),
            ItemID.AshWoodHelmet, new FuelData(FuelPotency.Low, 120),
            ItemID.AshWoodBreastplate, new FuelData(FuelPotency.Low, 120),
            ItemID.AshWoodGreaves, new FuelData(FuelPotency.Low, 120),
            // TODO continue...

            // other wood furniture
            ItemID.EbonwoodPlatform, new FuelData(FuelPotency.Low, 80),
            ItemID.EbonwoodChair, new FuelData(FuelPotency.Low, 80),
            ItemID.EbonwoodDoor, new FuelData(FuelPotency.Low, 80),
            ItemID.EbonwoodTable, new FuelData(FuelPotency.Low, 80),
            ItemID.EbonwoodWorkBench, new FuelData(FuelPotency.Low, 80),
            ItemID.EbonwoodSink, new FuelData(FuelPotency.Low, 80),
            ItemID.EbonwoodBed, new FuelData(FuelPotency.Low, 80),
            ItemID.EbonwoodBookcase, new FuelData(FuelPotency.Low, 80),
            ItemID.EbonwoodDresser, new FuelData(FuelPotency.Low, 80),
            ItemID.EbonwoodClock, new FuelData(FuelPotency.Low, 80),
            ItemID.EbonwoodPiano, new FuelData(FuelPotency.Low, 80),
            ItemID.EbonwoodSofa, new FuelData(FuelPotency.Low, 80),
            ItemID.EbonwoodChest, new FuelData(FuelPotency.Low, 80),

            ItemID.RichMahoganyPlatform, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahoganyChair, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahoganyDoor, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahoganyTable, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahoganyWorkBench, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahoganySink, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahoganyBed, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahoganyBookcase, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahoganyDresser, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahoganyClock, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahoganyPiano, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahoganySofa, new FuelData(FuelPotency.Low, 80),
            ItemID.RichMahoganyChest, new FuelData(FuelPotency.Low, 80),

            ItemID.ShadewoodPlatform, new FuelData(FuelPotency.Low, 80),
            ItemID.ShadewoodChair, new FuelData(FuelPotency.Low, 80),
            ItemID.ShadewoodDoor, new FuelData(FuelPotency.Low, 80),
            ItemID.ShadewoodTable, new FuelData(FuelPotency.Low, 80),
            ItemID.ShadewoodWorkBench, new FuelData(FuelPotency.Low, 80),
            ItemID.ShadewoodSink, new FuelData(FuelPotency.Low, 80),
            ItemID.ShadewoodBed, new FuelData(FuelPotency.Low, 80),
            ItemID.ShadewoodBookcase, new FuelData(FuelPotency.Low, 80),
            ItemID.ShadewoodDresser, new FuelData(FuelPotency.Low, 80),
            ItemID.ShadewoodClock, new FuelData(FuelPotency.Low, 80),
            ItemID.ShadewoodPiano, new FuelData(FuelPotency.Low, 80),
            ItemID.ShadewoodSofa, new FuelData(FuelPotency.Low, 80),
            ItemID.ShadewoodChest, new FuelData(FuelPotency.Low, 80),

            ItemID.PearlwoodPlatform, new FuelData(FuelPotency.Low, 80),
            ItemID.PearlwoodChair, new FuelData(FuelPotency.Low, 80),
            ItemID.PearlwoodDoor, new FuelData(FuelPotency.Low, 80),
            ItemID.PearlwoodTable, new FuelData(FuelPotency.Low, 80),
            ItemID.PearlwoodWorkBench, new FuelData(FuelPotency.Low, 80),
            ItemID.PearlwoodSink, new FuelData(FuelPotency.Low, 80),
            ItemID.PearlwoodBed, new FuelData(FuelPotency.Low, 80),
            ItemID.PearlwoodBookcase, new FuelData(FuelPotency.Low, 80),
            ItemID.PearlwoodDresser, new FuelData(FuelPotency.Low, 80),
            ItemID.PearlwoodClock, new FuelData(FuelPotency.Low, 80),
            ItemID.PearlwoodPiano, new FuelData(FuelPotency.Low, 80),
            ItemID.PearlwoodSofa, new FuelData(FuelPotency.Low, 80),
            ItemID.PearlwoodChest, new FuelData(FuelPotency.Low, 80),

            ItemID.BorealWoodPlatform, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWoodChair, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWoodDoor, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWoodTable, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWoodWorkBench, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWoodSink, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWoodBed, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWoodBookcase, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWoodDresser, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWoodClock, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWoodPiano, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWoodSofa, new FuelData(FuelPotency.Low, 80),
            ItemID.BorealWoodChest, new FuelData(FuelPotency.Low, 80),

            ItemID.PalmWoodPlatform, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodChair, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodDoor, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodTable, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodWorkBench, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodSink, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodBed, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodBench, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodBookcase, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodDresser, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodClock, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodPiano, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodSofa, new FuelData(FuelPotency.Low, 80),
            ItemID.PalmWoodChest, new FuelData(FuelPotency.Low, 80),

            ItemID.DynastyPlatform, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastyChair, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastyDoor, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastyTable, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastyWorkBench, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastySink, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastyBed, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastyBookcase, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastyDresser, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastyClock, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastyPiano, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastySofa, new FuelData(FuelPotency.Low, 80),
            ItemID.DynastyChest, new FuelData(FuelPotency.Low, 80),

            ItemID.SpookyPlatform, new FuelData(FuelPotency.Low, 80),
            ItemID.SpookyChair, new FuelData(FuelPotency.Low, 80),
            ItemID.SpookyDoor, new FuelData(FuelPotency.Low, 80),
            ItemID.SpookyTable, new FuelData(FuelPotency.Low, 80),
            ItemID.SpookySink, new FuelData(FuelPotency.Low, 80),
            ItemID.SpookyBed, new FuelData(FuelPotency.Low, 80),
            ItemID.SpookyBookcase, new FuelData(FuelPotency.Low, 80),
            ItemID.SpookyDresser, new FuelData(FuelPotency.Low, 80),
            ItemID.SpookyClock, new FuelData(FuelPotency.Low, 80),
            ItemID.SpookyPiano, new FuelData(FuelPotency.Low, 80),
            ItemID.SpookySofa, new FuelData(FuelPotency.Low, 80),
            ItemID.SpookyChest, new FuelData(FuelPotency.Low, 80),

            ItemID.LivingWoodPlatform, new FuelData(FuelPotency.Low, 80),
            ItemID.LivingWoodChair, new FuelData(FuelPotency.Low, 80),
            ItemID.LivingWoodDoor, new FuelData(FuelPotency.Low, 80),
            ItemID.LivingWoodTable, new FuelData(FuelPotency.Low, 80),
            ItemID.LivingWoodSink, new FuelData(FuelPotency.Low, 80),
            ItemID.LivingWoodBed, new FuelData(FuelPotency.Low, 80),
            ItemID.LivingWoodBookcase, new FuelData(FuelPotency.Low, 80),
            ItemID.LivingWoodDresser, new FuelData(FuelPotency.Low, 80),
            ItemID.LivingWoodClock, new FuelData(FuelPotency.Low, 80),
            ItemID.LivingWoodPiano, new FuelData(FuelPotency.Low, 80),
            ItemID.LivingWoodSofa, new FuelData(FuelPotency.Low, 80),
            ItemID.LivingWoodChest, new FuelData(FuelPotency.Low, 80),

            ItemID.AshWoodPlatform, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.AshWoodChair, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.AshWoodDoor, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.AshWoodTable, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.AshWoodSink, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.AshWoodBed, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.AshWoodBookcase, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.AshWoodDresser, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.AshWoodClock, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.AshWoodPiano, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.AshWoodSofa, new FuelData(FuelPotency.VeryLow, 120),
            ItemID.AshWoodChest, new FuelData(FuelPotency.VeryLow, 120),
            // TODO continue...

            // Critters :dread:
            ItemID.Bunny, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Squirrel, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.SquirrelRed, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Penguin, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Bird, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.BlueJay, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Cardinal, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.YellowCockatiel, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.GrayCockatiel, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Duck, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Seagull, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.MallardDuck, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Owl, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Toucan, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Grebe, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Worm, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.EnchantedNightcrawler, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Firefly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Lavafly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.LightningBug, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Frog, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Grasshopper, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Snail, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.GlowingSnail, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.MagmaSnail, new FuelData(FuelPotency.Low, 140, critter: true),
            ItemID.Buggy, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Grubby, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Sluggy, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Turtle, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.TurtleJungle, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Maggot, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Mouse, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Rat, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Scorpion, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.BlackScorpion, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Goldfish, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Pupfish, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.WaterStrider, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.Seahorse, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.BlackDragonfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.BlueDragonfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.GreenDragonfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.OrangeDragonfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.RedDragonfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.YellowDragonfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.JuliaButterfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.MonarchButterfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.PurpleEmperorButterfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.RedAdmiralButterfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.SulphurButterfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.TreeNymphButterfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.UlyssesButterfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.ZebraSwallowtailButterfly, new FuelData(FuelPotency.Low, 100, critter: true),
            ItemID.HellButterfly, new FuelData(FuelPotency.Low, 140, critter: true),

            // Fishes
            ItemID.Bass, new FuelData(FuelPotency.Low, 100),
            ItemID.RedSnapper, new FuelData(FuelPotency.Low, 100),
            ItemID.RockLobster, new FuelData(FuelPotency.Low, 100),
            ItemID.Salmon, new FuelData(FuelPotency.Low, 100),
            ItemID.Shrimp, new FuelData(FuelPotency.Low, 100),
            ItemID.Trout, new FuelData(FuelPotency.Low, 100),
            ItemID.Tuna, new FuelData(FuelPotency.Low, 100),
            ItemID.AtlanticCod, new FuelData(FuelPotency.Low, 100),
            ItemID.ArmoredCavefish, new FuelData(FuelPotency.Low, 100),
            ItemID.ChaosFish, new FuelData(FuelPotency.Low, 100),
            ItemID.CrimsonTigerfish, new FuelData(FuelPotency.Low, 100),
            ItemID.Damselfish, new FuelData(FuelPotency.Low, 100),
            ItemID.DoubleCod, new FuelData(FuelPotency.Low, 100),
            ItemID.Ebonkoi, new FuelData(FuelPotency.Low, 100),
            ItemID.FlarefinKoi, new FuelData(FuelPotency.Medium, 200),
            ItemID.Flounder, new FuelData(FuelPotency.Low, 100),
            ItemID.FrostMinnow, new FuelData(FuelPotency.Low, 200),
            ItemID.Hemopiranha, new FuelData(FuelPotency.Low, 100),
            ItemID.Honeyfin, new FuelData(FuelPotency.Low, 100),
            ItemID.NeonTetra, new FuelData(FuelPotency.Low, 100),
            ItemID.Obsidifish, new FuelData(FuelPotency.Low, 100),
            ItemID.PrincessFish, new FuelData(FuelPotency.Low, 100),
            ItemID.Prismite, new FuelData(FuelPotency.Low, 100),
            ItemID.Stinkfish, new FuelData(FuelPotency.Low, 100),
            ItemID.Stinkfish, new FuelData(FuelPotency.Low, 100),
            ItemID.VariegatedLardfish, new FuelData(FuelPotency.Low, 100),
            ItemID.SpecularFish, new FuelData(FuelPotency.Low, 100),
            ItemID.GreenJellyfish, new FuelData(FuelPotency.Low, 100),
            ItemID.PinkJellyfish, new FuelData(FuelPotency.Low, 100),
            ItemID.BlueJellyfish, new FuelData(FuelPotency.Low, 100)
        );


        /// <summary> 
        /// Damage adjustment for items while in a Macrocosm subworld. 
        /// Applies a tooltip indicating if the item is stronger or weaker.
        /// </summary>
        public static float[] DamageAdjustment { get; } = ItemID.Sets.Factory.CreateFloatSet(defaultState: 1f,
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

        public static bool[] WingTimeDependsOnAtmosphericDensity { get; } = ItemID.Sets.Factory.CreateBoolSet(
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

        public static float[] WingTimeMultiplier_Moon { get; } = ItemID.Sets.Factory.CreateFloatSet(defaultState: 1f,
            ItemID.LongRainbowTrailWings, 0.5f,
            ItemID.Hoverboard, 0.3f,
            ItemID.Jetpack, 0.3f
        );

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
