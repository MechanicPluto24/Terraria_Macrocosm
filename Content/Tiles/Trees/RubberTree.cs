using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.DataStructures;
using Macrocosm.Content.Items.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Trees
{
    public class RubberTree : CustomTree
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

        public override TreeType FramingType => TreeType.Custom;
        public override TreeType DrawingType => TreeType.Custom;

        public override int WoodType => ItemID.RichMahogany;
        public override TileTypeStylePair Sapling => new(ModContent.TileType<RubberTreeSapling>(), 0);

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        // This is the primary texture for the trunk. Branches and foliage use different settings.
        public override Asset<Texture2D> GetTexture() => base.GetTexture();
        public override Asset<Texture2D> GetTopsTexture(int variant) => base.GetTopsTexture(variant);
        public override Asset<Texture2D> GetBranchTexture(int variant) => base.GetBranchTexture(variant);

        public override bool CustomGrowTree(int x, int y)
        {
            if (!WorldGen.InWorld(x, y))
                return false;

            int groundY = y;
            while (Main.tile[x, groundY].TileType == Sapling.Type)
            {
                groundY++;
                if (groundY > Main.maxTilesY - 1)
                    return false;
            }

            if (Main.tile[x - 1, groundY - 1].LiquidAmount != 0 || Main.tile[x, groundY - 1].LiquidAmount != 0 || Main.tile[x + 1, groundY - 1].LiquidAmount != 0)
                return false;

            Tile groundTile = Main.tile[x, groundY];
            bool wall = WallTest(Main.tile[x, groundY - 1].WallType);
            if (!GroundTest(groundTile.TileType) || !wall)
                return false;

            if ((!Main.tile[x - 1, groundY].HasTile || !GroundTest(Main.tile[x - 1, groundY].TileType)) && (!Main.tile[x + 1, groundY].HasTile || !GroundTest(Main.tile[x + 1, groundY].TileType)))
                return false;

            int treeHeight = WorldGen.genRand.Next(TreeHeightMin, TreeHeightMax + 1);
            int paddedHeight = treeHeight + TreeTopPaddingNeeded;
            if (!WorldGen.EmptyTileCheck(x - 2, x + 2, groundY - paddedHeight, groundY - 1, Sapling.Type))
                return false;

            TileColorCache cache = Main.tile[x, groundY].BlockColorAndCoating();

            int height = WorldGen.genRand.Next(TreeHeightMin, TreeHeightMax + 1);
            for (int j = 0; j < height; j++)
            {
                Tile treeTile = Main.tile[x, groundY - 1 - j];

                // Bottom tile
                if (j == 0)
                {
                    treeTile.HasTile = true;
                    treeTile.TileType = Type;
                    treeTile.UseBlockColors(cache);

                    treeTile.TileFrameX = 0;
                    treeTile.TileFrameY = 22;

                    // Add roots left & right
                    bool hasRootLeft = false;
                    bool hasRootRight = false;

                    if (Main.tile[x - 1, groundY].HasUnactuatedTile && !Main.tile[x - 1, groundY].IsHalfBlock && Main.tile[x - 1, groundY].Slope == 0 && WorldGen.IsTileTypeFitForTree(Main.tile[x - 1, groundY].TileType))
                        hasRootLeft = true;

                    if (Main.tile[x + 1, groundY].HasUnactuatedTile && !Main.tile[x + 1, groundY].IsHalfBlock && Main.tile[x + 1, groundY].Slope == 0 && WorldGen.IsTileTypeFitForTree(Main.tile[x + 1, groundY].TileType))
                        hasRootRight = true;

                    if (WorldGen.genRand.NextBool(3))
                        hasRootLeft = false;

                    if (WorldGen.genRand.NextBool(3))
                        hasRootRight = false;

                    if (hasRootRight)
                    {
                        Tile rootTileRight = Main.tile[x + 1, groundY - 1];
                        rootTileRight.HasTile = true;
                        rootTileRight.TileType = Type;
                        rootTileRight.UseBlockColors(cache);

                        rootTileRight.TileFrameX = 44;
                        rootTileRight.TileFrameY = 22;
                    }

                    if (hasRootLeft)
                    {
                        Tile rootTileLeft = Main.tile[x - 1, groundY - 1];
                        rootTileLeft.HasTile = true;
                        rootTileLeft.TileType = Type;
                        rootTileLeft.UseBlockColors(cache);

                        rootTileLeft.TileFrameX = 66;
                        rootTileLeft.TileFrameY = 22;
                    }

                    if (hasRootLeft && hasRootRight)
                    {
                        treeTile.TileFrameX = 110;
                        treeTile.TileFrameY = 22;
                    }
                    else if (hasRootLeft)
                    {
                        treeTile.TileFrameX = 88;
                        treeTile.TileFrameY = 22;
                    }
                    else if (hasRootRight)
                    {
                        treeTile.TileFrameX = 22;
                        treeTile.TileFrameY = 22;
                    }
                    else
                    {
                        treeTile.TileFrameX = 0;
                        treeTile.TileFrameY = 22;
                    }
                }
                else if (j == height - 1)
                {
                    treeTile.HasTile = true;
                    treeTile.TileType = Type;
                    treeTile.TileFrameX = 110;
                    treeTile.TileFrameY = 0;
                    treeTile.UseBlockColors(cache);
                }
                else
                {
                    treeTile.HasTile = true;
                    treeTile.TileType = Type;
                    treeTile.TileFrameX = (short)(22 * (j % 5));
                    treeTile.TileFrameY = 0;
                    treeTile.UseBlockColors(cache);
                }
            }

            WorldGen.RangeFrame(x - 2, groundY - height - 1, x + 2, groundY + 1);
            NetMessage.SendTileSquare(-1, x, groundY - height, 1, height);
            return true;
        }

        public override void CustomTileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            int typeAbove = -1;
            int typeBelow = -1;

            Tile tile = Main.tile[i, j];
            int type = tile.TileType;

            Tile tileAbove = Main.tile[i, j - 1];
            Tile tileBelow = Main.tile[i, j + 1];
            Tile tileLeft = Main.tile[i - 1, j];
            Tile tileRight = Main.tile[i + 1, j];

            int frameX = Main.tile[i, j].TileFrameX;
            int frameY = Main.tile[i, j].TileFrameY;

            if (tileAbove.HasTile)
                typeAbove = tileAbove.TileType;

            if (tileBelow.HasTile)
                typeBelow = tileBelow.TileType;

            // Kill tile if the one below was cut
            if (typeBelow != type && !GroundTest(typeBelow))
                WorldGen.KillTile(i, j);

            // For bottom/root frames, kill if ground is no longer valid
            if (tile.TileFrameY > 0 && !GroundTest(typeBelow))
                WorldGen.KillTile(i, j);

            // Trunk frame
            if (tile.TileFrameY == 0)
            {
                // Apply the stump frames for the topmost tile
                if (typeAbove != type && tile.TileFrameX != 110)
                    tile.TileFrameX = (short)(22 * WorldGen.genRand.Next(6, 9));
            }
            // Bottom/root frame
            else if (tile.TileFrameY == 22)
            {
                if (!GroundTest(typeBelow))
                {
                    WorldGen.KillTile(i, j);
                }
                // Root frame to the right
                else if (tile.TileFrameX == 44)
                {
                    // Break if bottom of trunk is not on the left
                    if (tileLeft.TileType != type) 
                        WorldGen.KillTile(i, j);
                }
                // Root frame to the left
                else if (tile.TileFrameX == 66)
                {
                    // Break if bottom of trunk is not on the right
                    if (tileRight.TileType != type)
                        WorldGen.KillTile(i, j);
                }
                // Regular bottom frame
                else
                {
                    bool hasRootLeft = tileLeft.TileType == type && tileLeft.TileFrameX == 66 && tileLeft.TileFrameY == 22;
                    bool hasRootRight = tileRight.TileType == type && tileRight.TileFrameX == 44 && tileRight.TileFrameY == 22;

                    if (hasRootLeft && hasRootRight)
                        tile.TileFrameX = 110;
                    else if (hasRootLeft)
                        tile.TileFrameX = 88;
                    else if (hasRootRight)
                        tile.TileFrameX = 22;
                    else
                        tile.TileFrameX = 0;
                }
            }

            if ((tile.TileFrameX != frameX || tile.TileFrameY != frameY))
                WorldGen.DiamondTileFrame(i, j);
        }

        public override bool Shake(int x, int y, ref bool createLeaves)
        {
            if (WorldGen.genRand.NextBool(8))
                Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), new Vector2(x, y) * 16, ModContent.ItemType<Coal>()); //testing lol

            return false;
        }

        public override int TreeLeaf() => ModContent.GoreType<RubberTreeLeaf>();

        public override void CustomPostDrawTree(int x, int y, Tile tile, SpriteBatch spriteBatch, double treeWindCounter, Vector2 unscaledPosition, Vector2 zero, float topsWindFactor, float branchWindFactor)
        {
            if (tile.TileFrameX == 110 && tile.TileFrameY == 0)
            {
                int variant = GetVariant(x, y);
                int treeFrame = 0;
                int xOffset = 0;

                if (!GetTreeFoliageData(x, y, xOffset, ref treeFrame, out int floorY, out int topTextureFrameWidth, out int topTextureFrameHeight))
                    return;

                byte tileColor = tile.TileColor;
                Texture2D treeTopTexture = TryGetTreeTopAndRequestIfNotReady(Type, variant, tileColor);
                Vector2 position = new Vector2(x * 16 - (int)unscaledPosition.X - 44 + Main.tile[x, y].TileFrameY + topTextureFrameWidth / 2, y * 16 - (int)unscaledPosition.Y) + zero;

                float windCycle = tile.WallType == 0 ? Main.instance.TilesRenderer.GetWindCycle(x, y, treeWindCounter) : 0f;
                position.X += windCycle * 2f;
                position.Y += Math.Abs(windCycle) * 2f;

                Color color = Lighting.GetColor(x, y);
                if (tile.IsTileFullbright)
                    color = Color.White;

                Rectangle frame = new(treeFrame * (topTextureFrameWidth + 2), 0, topTextureFrameWidth, topTextureFrameHeight);
                spriteBatch.Draw(treeTopTexture, position, frame, color, windCycle * topsWindFactor, new Vector2(topTextureFrameWidth / 2, topTextureFrameHeight), 1f, SpriteEffects.None, 0f);
            }
        }

        public override void GetTopTextureFrame(int i, int j, ref int treeFrame, out int topTextureFrameWidth, out int topTextureFrameHeight)
        {
            topTextureFrameWidth = 104;
            topTextureFrameHeight = 92;
        }
    }
}