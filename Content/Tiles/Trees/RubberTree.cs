using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Content.Items.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Trees
{
    public class RubberTree : CustomTree
    {
        public override TreePaintingSettings TreePaintingSettings => new()
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

        public override int WoodType => ItemID.RichMahogany;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        // This is the primary texture for the trunk. Branches and foliage use different settings.
        public override Asset<Texture2D> GetTexture() => base.GetTexture();
        public override Asset<Texture2D> GetTopsTexture(int variant) => base.GetTopsTexture(variant);
        public override Asset<Texture2D> GetBranchTexture(int variant) => base.GetBranchTexture(variant);

        public override bool AllowBranches => false;

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<RubberTreeSapling>();
        }

        public override bool Shake(int x, int y, ref bool createLeaves)
        {
            Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), new Vector2(x, y) * 16, ModContent.ItemType<Coal>()); //testing lol
            return false;
        }

        public override int TreeLeaf() => ModContent.GoreType<RubberTreeLeaf>();

        protected override void GetTopTextureFrame(int i, int j, ref int treeFrame, out int topTextureFrameWidth, out int topTextureFrameHeight)
        {
            topTextureFrameWidth = 104;
            topTextureFrameHeight = 92;
        }
    }
}