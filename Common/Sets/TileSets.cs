using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace Macrocosm.Common.Sets;

/// <summary> Tile Sets for special behavior of some Tiles, useful for crossmod. </summary>
[ReinitializeDuringResizeArrays]
internal class TileSets
{
    /// <summary> Tile types that count for the Graveyard biome, besides <see cref="TileID.Tombstones"/> </summary>
    public static bool[] CountsForGraveyard { get; } = TileID.Sets.Factory.CreateNamedSet(nameof(CountsForGraveyard)).Description("Tile types that count for the Graveyard biome, besides TileID.Tombstones.").RegisterBoolSet();

    /// <summary> The Tree Tile type, different from <see cref="TileID.Trees"/>, that this sapling tile grows into. Used only for custom tree saplings. </summary>
    public static int[] SaplingTreeGrowthType { get; } = TileID.Sets.Factory.CreateNamedSet(nameof(SaplingTreeGrowthType)).Description("The Tree Tile type, different from TileID.Trees, that this sapling tile grows into. Used only for custom tree saplings.").RegisterIntSet(defaultState: -1);

    /// <summary> Tile types that are containers of Width and Height different from Chests or Dressers. </summary>
    // TODO: add support for net serializaton (and proper unloading but this needs to be in tML directly) 
    public static bool[] CustomContainer { get; } = TileID.Sets.Factory.CreateNamedSet(nameof(CustomContainer)).Description("Tile types that are containers of Width and Height different from Chests or Dressers.").RegisterBoolSet();

    /// <summary> Tile types with custom <see cref="TreePaintingSettings"/> </summary>
    public static TreePaintingSettings[] PaintingSettings { get; } = TileID.Sets.Factory.CreateNamedSet(nameof(PaintingSettings)).Description("Tile types with custom TreePaintingSettings").RegisterCustomSet<TreePaintingSettings>(defaultState: null);

    /// <summary> Tile types that allow liquids to flow through them. You cannot place liquids on them directly. Only works for <see cref="Main.tileSolid"/> tiles.</summary>
    public static bool[] AllowLiquids { get; } = TileID.Sets.Factory.CreateNamedSet(nameof(AllowLiquids)).Description("Tile types that allow liquids to flow through them. You cannot place liquids on them directly. Only works for Main.tileSolid tiles.").RegisterBoolSet();

    // TODO: this needs a rework
    public static int[] RandomStyles { get; } = TileID.Sets.Factory.CreateIntSet(defaultState: 1);
}
