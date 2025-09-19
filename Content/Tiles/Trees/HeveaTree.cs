using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.DataStructures;
using Macrocosm.Content.Items.Plants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Content.Tiles.Trees;

public class HeveaTree : CustomTree
{
    public override TreePaintingSettings PaintingSettings => new()
    {
        UseSpecialGroups = true,
        SpecialGroupMinimalHueValue = 11f / 72f,
        SpecialGroupMaximumHueValue = 0.25f,
        SpecialGroupMinimumSaturationValue = 0.88f,
        SpecialGroupMaximumSaturationValue = 1f
    };

    public override int[] GrowsOnTileId { get; set; } = [TileID.JungleGrass];
    public override int TreeHeightMin => 13;
    public override int TreeHeightMax => 22;
    public override int TreeTopPaddingNeeded => 40;

    public override TreeCategory FramingMode => TreeCategory.Tree;
    public override TreeCategory DrawingMode => TreeCategory.Tree;

    public override int WoodType => ModContent.ItemType<HeveaWood>();
    public override int AcornType => ModContent.ItemType<Items.Plants.HeveaSapling>();
    public override TileTypeStylePair Sapling => new(ModContent.TileType<HeveaSapling>(), 0);

    public override TreeTypes CountsAsTreeType => TreeTypes.Jungle;
    public override bool Shake(int x, int y, IEntitySource source, ref bool createLeaves)
    {
        if (WorldGen.genRand.NextBool(8))
        {
            Item.NewItem(source, new Vector2(x, y) * 16, ModContent.ItemType<RubberTreeSap>());
            return false;
        }

        return true;
    }

    public override IEnumerable<Item> GetExtraItemDrops(int i, int j)
    {
        if (WorldGen.genRand.NextBool(10))
            yield return new Item(ModContent.ItemType<RubberTreeSap>());
    }

    

    public override int TreeLeaf => ModContent.GoreType<HeveaLeaf>();
   

    public override void GetFoliageData(int i, int j, ref int treeFrame, out int topTextureFrameWidth, out int topTextureFrameHeight)
    {
        topTextureFrameWidth = 118;
        topTextureFrameHeight = 96;
    }

    public override bool CanEmitLeaves(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible, int leafFrequency, UnifiedRandom rand, out Vector2 offset, out Vector2? velocityOverride, out float? scaleOverride)
    {
        velocityOverride = null;
        scaleOverride = null;
        offset = default;

        if(IsTileALeafyTreeTop(i, j) && rand.NextBool(leafFrequency) && visible)
        {
            offset = new Vector2(rand.NextFloat(-32f, 32f), -rand.NextFloat(32f, 32f * 3f));
            return true;
        }

        return false;
    }
}