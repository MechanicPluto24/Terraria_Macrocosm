using Macrocosm.Common.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace Macrocosm.Common.Sets;

/// <summary> Tile Sets for special behavior of some Tiles, useful for crossmod. </summary>
[ReinitializeDuringResizeArrays]
internal class TileSets
{
    /// <summary> Tile types that count for the Graveyard biome. </summary>
    public static bool[] CountsForGraveyard { get; } = TileID.Sets.Factory.CreateNamedSet(nameof(CountsForGraveyard)).Description("Tile types that count for the Graveyard biome.").RegisterBoolSet();

    /// <summary> The Tree Tile type, different from <see cref="TileID.Trees"/>, that this tile grows into. Used for custom saplings. </summary>
    public static int[] SaplingTreeGrowthType { get; } = TileID.Sets.Factory.CreateNamedSet(nameof(SaplingTreeGrowthType)).Description("The Tree Tile type, different from TileID.Trees, that this tile grows into. Used for custom saplings.").RegisterIntSet(defaultState: -1);

    /// <summary> Tile types that are containers of Width and Height different from Chests or Dressers. </summary>
    // TODO: add support for net serializaton (and proper unloading but this needs to be in tML directly) 
    public static bool[] CustomContainer { get; } = TileID.Sets.Factory.CreateNamedSet(nameof(CustomContainer)).Description("Tile types that are containers of Width and Height different from Chests or Dressers.").RegisterBoolSet();

    /// <summary> Tile types with custom <see cref="TreePaintingSettings"/> </summary>
    public static TreePaintingSettings[] PaintingSettings { get; } = TileID.Sets.Factory.CreateNamedSet(nameof(PaintingSettings)).Description("Tile types with custom TreePaintingSettings").RegisterCustomSet<TreePaintingSettings>(defaultState: null);

    // TODO: this needs a rework
    public static int[] RandomStyles { get; } = TileID.Sets.Factory.CreateIntSet(defaultState: 1);

    /// <summary>
    /// Maps tile types to their drill drop data: item yielded and an optional world-state condition.
    /// Default (unregistered) = <see cref="DrillDropData.None"/> (not drillable).
    /// Vanilla entries are registered inline; modded ore tiles register themselves in their <c>SetStaticDefaults</c>.
    /// </summary>
    public static DrillDropData[] DrillDrop { get; } = TileID.Sets.Factory.CreateNamedSet(nameof(DrillDrop)).Description("Per-tile drill drop item and optional unlock condition.").RegisterCustomSet<DrillDropData>(defaultState: DrillDropData.None,
        // Terrain
        TileID.Stone,        new DrillDropData(ItemID.StoneBlock),
        // Pre-hardmode ores — no condition
        TileID.Copper,       new DrillDropData(ItemID.CopperOre),
        TileID.Tin,          new DrillDropData(ItemID.TinOre),
        TileID.Iron,         new DrillDropData(ItemID.IronOre),
        TileID.Lead,         new DrillDropData(ItemID.LeadOre),
        TileID.Silver,       new DrillDropData(ItemID.SilverOre),
        TileID.Tungsten,     new DrillDropData(ItemID.TungstenOre),
        TileID.Gold,         new DrillDropData(ItemID.GoldOre),
        TileID.Platinum,     new DrillDropData(ItemID.PlatinumOre),
        // Evil ores
        TileID.Demonite,     new DrillDropData(ItemID.DemoniteOre),
        TileID.Crimtane,     new DrillDropData(ItemID.CrimtaneOre),
        // Special
        TileID.Meteorite,    new DrillDropData(ItemID.Meteorite),
        TileID.Hellstone,    new DrillDropData(ItemID.Hellstone),
        // Biome stone
        TileID.MarbleBlock,  new DrillDropData(ItemID.Marble),
        TileID.GraniteBlock, new DrillDropData(ItemID.Granite),
        // Hardmode ores
        TileID.Cobalt,       new DrillDropData(ItemID.CobaltOre),
        TileID.Palladium,    new DrillDropData(ItemID.PalladiumOre),
        TileID.Mythril,      new DrillDropData(ItemID.MythrilOre),
        TileID.Orichalcum,   new DrillDropData(ItemID.OrichalcumOre),
        TileID.Adamantite,   new DrillDropData(ItemID.AdamantiteOre),
        TileID.Titanium,     new DrillDropData(ItemID.TitaniumOre),
        // Chlorophyte — post-Plantera only
        TileID.Chlorophyte,  new DrillDropData(ItemID.ChlorophyteOre, Condition.DownedPlantera),
        // LunarOre
        TileID.LunarOre,     new DrillDropData(ItemID.LunarOre)
    );
}
