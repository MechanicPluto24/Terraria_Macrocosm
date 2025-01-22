using Macrocosm.Common.Sets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Trees
{
    public class RubberTreeSapling : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorValidTiles = [TileID.JungleGrass];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawFlipHorizontal = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.StyleMultiplier = 3;
            TileObjectData.addTile(Type);

            TileID.Sets.TreeSapling[Type] = false; // Do not run regular tree sapling logic
            TileID.Sets.CommonSapling[Type] = true;
            TileSets.SaplingTreeGrowthType[Type] = ModContent.TileType<RubberTree>();

            TileID.Sets.SwaysInWindBasic[Type] = true;

            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]); // Make this tile interact with golf balls in the same way other plants do

            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Sapling"));
            DustType = DustID.JungleGrass;
            AdjTiles = [TileID.Saplings];
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void RandomUpdate(int i, int j)
        {
            if(WorldGen.genRand.NextBool(20))
                WorldGen.AttemptToGrowTreeFromSapling(i, j, j > (int)Main.worldSurface - 1);
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects effects)
        {
            if (i % 2 == 0)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
        }
    }
}