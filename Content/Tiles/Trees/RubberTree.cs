using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Trees
{
    public class RubberTree : ModTree
    {
        private string TexturePath => this.GetNamespacePath();
        private Asset<Texture2D> texture;
        private Asset<Texture2D> branchesTexture;
        private Asset<Texture2D> topsTexture;

        // This is a blind copy-paste from Vanilla's PurityPalmTree settings.
        // TODO: This needs some explanations
        public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        public override void SetStaticDefaults()
        {
            GrowsOnTileId = [TileID.JungleGrass];

            texture = ModContent.Request<Texture2D>(TexturePath);
            branchesTexture = ModContent.Request<Texture2D>(TexturePath + "_Branches");
            topsTexture = ModContent.Request<Texture2D>(TexturePath + "_Tops");
        }


        public override Asset<Texture2D> GetTexture() => texture;
        public override Asset<Texture2D> GetBranchTextures() => branchesTexture;
        public override Asset<Texture2D> GetTopTextures() => topsTexture;

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return 20;
        }

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
            // This is where fancy code could go, but let's save that for an advanced example
        }


        //public override int DropWood() => ModContent.ItemType<RubberWood>();
        public override int DropWood() => ItemID.Wood;

        public override bool Shake(int x, int y, ref bool createLeaves)
        {
            Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), new Vector2(x, y) * 16, ItemID.Wood);
            //Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), new Vector2(x, y) * 16, ModContent.ItemType<Items.Rubber>());
            //Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), new Vector2(x, y) * 16, ModContent.ItemType<Items.RubberWood>());
            return false;
        }

        public override int TreeLeaf()
        {
            return -1;
            //return ModContent.GoreType<RubberTreeLeaf>();
        }
    }
}