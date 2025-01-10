using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Common.Bases.Tiles
{
    /// <summary>
    /// A very custom modded tree implementation, with extra configuration options. 
    /// <br/> It has its own tile type, compared to <see cref="ModTree"/> which is just a <see cref="TileID.Trees"/> biome variant.
    /// <br/> Based on lion8cake's implementation of CreamTree @ "Confection REBAKED" mod.
    /// </summary>
    public abstract partial class CustomTree : ModTile, ITree
    {
        public enum TreeType
        {
            Tree,
            Palm,
            Custom
        };

        /// <summary> Array of valid tile types. </summary>
        public abstract int[] GrowsOnTileId { get; set; }

        /// <summary> Min tree height in tiles </summary>
        public abstract int TreeHeightMin { get; }

        /// <summary> Max tree height in tiles </summary>
        public abstract int TreeHeightMax { get; }

        /// <summary> Padding for the tree top </summary>
        public abstract int TreeTopPaddingNeeded { get; }

        public virtual TreeType FramingType => TreeType.Tree;
        public virtual TreeType DrawingType => TreeType.Tree;

        public virtual int WoodType => ItemID.Wood;
        public virtual int AcornType => ItemID.Acorn;

        /// <summary> For tree shake purposes </summary>
        public TreeTypes CountsAsTreeType => TreeTypes.None;
        public int VanillaCount => 0;
        public virtual int PlantTileId => Type;

        /// <summary> Random paint on the CelebrationMK10 seed </summary>
        public virtual bool TenthAniversaryRandomColor => false;

        public virtual TreePaintingSettings PaintingSettings { get; } = new()
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        public WorldGen.GrowTreeSettings GrowTreeSettings => new()
        {
            GroundTest = GroundTest,
            WallTest = WallTest,
            TreeHeightMax = TreeHeightMax,
            TreeHeightMin = TreeHeightMin,
            TreeTileType = Type,
            TreeTopPaddingNeeded = TreeTopPaddingNeeded,
            SaplingTileType = Sapling.Type
        };

        public virtual TileTypeStylePair Sapling { get; }

        /// <summary> Determine variant at tile coordinates. Defaults to a single variant (0) </summary>
        public virtual int GetVariant(int x, int y) => 0;

        /// <summary> Determine tree top frame size. Allows you to modify the <paramref name="treeFrame"/> </summary>
        public abstract void GetTopTextureFrame(int i, int j, ref int treeFrame, out int topTextureFrameWidth, out int topTextureFrameHeight);

        /// <summary> Check ground tile valid for this tree. Defaults to a <see cref="GrowsOnTileId"/> check </summary>
        //public virtual bool GroundTest(int tileType) => GrowsOnTileId.Contains(tileType) || tileType == Type;
        public virtual bool GroundTest(int tileType) => GrowsOnTileId.Contains(tileType) || tileType == Type;

        /// <summary> Check wall behing valid for this tree. Defaults to vanilla logic </summary>
        public virtual bool WallTest(int wallType) => WorldGen.DefaultTreeWallTest(wallType);

        /// <summary> Tree leaf gore type </summary>
        public virtual int TreeLeaf() => -1;

        /// <summary> Shake logic. Return true to also allow vanilla shake logic to run, based on <see cref="CountsAsTreeType"/> </summary>
        public virtual bool Shake(int x, int y, ref bool createLeaves) => true;

        /// <summary> Use for custom tile frame (<see cref="FramingType"/> == <see cref="TreeType.Custom"/>)</summary>
        public virtual void CustomTileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) { }

        /// <summary> Use for growing (<see cref="FramingType"/> == <see cref="TreeType.Custom"/>)</summary>
        public virtual bool CustomGrowTree(int x, int y) { return false; }

        /// <summary> Use for custom drawing (<see cref="FramingType"/> == <see cref="TreeType.Custom"/>)</summary>
        public virtual void CustomPostDrawTree(int x, int y, Tile tile, SpriteBatch spriteBatch, double treeWindCounter, Vector2 unscaledPosition, Vector2 zero, float topsWindFactor, float branchWindFactor) { }

        public virtual Asset<Texture2D> GetTexture() => ModContent.RequestIfExists<Texture2D>(Texture, out var texture) ? texture : null;

        /// <summary> Use this to get a custom tops texture. Autoloaded with a <c>"_Tops"</c> suffix, and <c>"_Tops_X"</c> for extra >0 variants; defaults to an empty texture if not existent </summary>
        public virtual Asset<Texture2D> GetTopsTexture(int variant) => ModContent.RequestIfExists<Texture2D>(Texture + "_Tops" + (variant > 0 ? $"_{variant}" : ""), out var texture, AssetRequestMode.ImmediateLoad) ? texture : Macrocosm.EmptyTex;

        /// <summary> Use this to get a custom branch texture. Autoloaded with a <c>"_Branches"</c> suffix, and <c>"_Branches_X"</c> for extra >0 variants; defaults to an empty texture if not existent </summary>
        public virtual Asset<Texture2D> GetBranchTexture(int variant) => ModContent.RequestIfExists<Texture2D>(Texture + "_Branches" + (variant > 0 ? $"_{variant}" : ""), out var texture, AssetRequestMode.ImmediateLoad) ? texture : Macrocosm.EmptyTex;

        public override void SetStaticDefaults()
        {
            Main.tileAxe[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLavaDeath[Type] = false;

            TileID.Sets.IsATreeTrunk[Type] = true;
            TileID.Sets.IsShakeable[Type] = true;
            TileID.Sets.GetsDestroyedForMeteors[Type] = true;
            TileID.Sets.GetsCheckedForLeaves[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;

            TileSets.PaintingSettings[Type] = PaintingSettings;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            width = 20;
            height = 20;

            if (tileFrameX >= 176)
                tileFrameX = (short)(tileFrameX - 176);
        }

        public sealed override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];
            if (i > 5 && j > 5 && i < Main.maxTilesX - 5 && j < Main.maxTilesY - 5 && tile.HasTile && Main.tileFrameImportant[Type])
            {
                switch (FramingType)
                {
                    case TreeType.Tree:
                        WorldGen.CheckTreeWithSettings(i, j, new WorldGen.CheckTreeSettings() { IsGroundValid = GroundTest });
                        break;
                    case TreeType.Palm:
                        CheckPalmTree(i, j);
                        break;
                    default:
                        CustomTileFrame(i, j, ref resetFrame, ref noBreak);
                        break;
                }
            }

            return false;
        }

        /// <summary> Adapted from WorldGen.GrowPalmTree, but without the sand checks </summary>
        protected void CheckPalmTree(int i, int j)
        {
            int typeAbove = -1;
            int typeBelow = -1;

            int type = Main.tile[i, j].TileType;
            int frameX = Main.tile[i, j].TileFrameX;
            int frameY = Main.tile[i, j].TileFrameY;

            if (Main.tile[i, j - 1].HasTile)
                typeAbove = Main.tile[i, j - 1].TileType;

            if (Main.tile[i, j + 1].HasTile)
                typeBelow = Main.tile[i, j + 1].TileType;

            if (!GroundTest(typeBelow) && typeBelow != type)
                WorldGen.KillTile(i, j);

            if ((Main.tile[i, j].TileFrameX == 66 || Main.tile[i, j].TileFrameX == 220) && !GroundTest(typeBelow))
                WorldGen.KillTile(i, j);

            if (typeAbove != type && Main.tile[i, j].TileFrameX <= 44)
                Main.tile[i, j].TileFrameX = (short)(WorldGen.genRand.Next(7, 10) * 22);
            else if (typeAbove != type && Main.tile[i, j].TileFrameX == 66)
                Main.tile[i, j].TileFrameX = 220;

            if (Main.tile[i, j].TileFrameX != frameX && Main.tile[i, j].TileFrameY != frameY && frameX >= 0 && frameY >= 0)
                WorldGen.DiamondTileFrame(i, j);
        }

        /// <summary>
        /// Grow a tree of this type. 
        /// <br/> You can also use <see cref="WorldGen.TryGrowingTreeByType(int, int, int)"/>
        /// </summary>
        public bool GrowTree(int x, int y)
        {
            var result = FramingType switch
            {
                TreeType.Tree => GrowRegularTree(x, y),
                TreeType.Palm => GrowPalmTree(x, y),
                _ => CustomGrowTree(x, y),
            };

            if (result)
                GrowEffects(x, y);

            return result;
        }

        public virtual void GrowEffects(int x, int y)
        {
            int leaf = TreeLeaf();
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SpecialFX, -1, -1, null, 1, x, y, 1f, leaf);

            if (Main.netMode == NetmodeID.SinglePlayer)
                WorldGen.TreeGrowFX(x, y, 1, leaf, hitTree: true);
        }

        /// <summary> Adapted clone of the WorldGen.GrowPalmTree method  </summary>
        protected bool GrowPalmTree(int x, int y)
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

            Tile tile = Main.tile[x, groundY];
            Tile tileAbove = Main.tile[x, groundY - 1];

            if (!tile.HasTile || tile.IsHalfBlock || tile.Slope != SlopeType.Solid)
                return false;

            if (tileAbove.WallType != 0 || tileAbove.LiquidAmount != 0)
                return false;

            if (!GroundTest(tile.TileType))
                return false;

            if (!WorldGen.EmptyTileCheck(x, x, groundY - 2, groundY - 1, Sapling.Type))
                return false;

            if (!WorldGen.EmptyTileCheck(x - 1, x + 1, groundY - 30, groundY - 3, Sapling.Type))
                return false;

            byte color = 0;
            if (Main.tenthAnniversaryWorld && !WorldGen.gen && TenthAniversaryRandomColor)
                color = (byte)WorldGen.genRand.Next(1, 13);

            int height = WorldGen.genRand.Next(TreeHeightMin, TreeHeightMax + 1);
            int randomFrameY = WorldGen.genRand.Next(-8, 9) * 2;
            short frameY = 0;

            for (int j = 0; j < height; j++)
            {
                tile = Main.tile[x, groundY - 1 - j];
                if (j == 0)
                {
                    tile.HasTile = true;
                    tile.TileType = Type;
                    tile.TileFrameX = 66;
                    tile.TileFrameY = 0;
                    tile.TileColor = color;
                    continue;
                }

                if (j == height - 1)
                {
                    tile.HasTile = true;
                    tile.TileType = Type;
                    tile.TileFrameX = (short)(22 * WorldGen.genRand.Next(4, 7));
                    tile.TileFrameY = frameY;
                    tile.TileColor = color;
                    continue;
                }

                if (frameY != randomFrameY)
                {
                    double progress = j / (double)height;
                    if (progress >= 0.25)
                        frameY = (short)(frameY + (short)(Math.Sign(randomFrameY) * 2));
                }

                tile.HasTile = true;
                tile.TileType = Type;
                tile.TileFrameX = (short)(22 * WorldGen.genRand.Next(0, 3));
                tile.TileFrameY = frameY;
                tile.TileColor = color;
            }

            WorldGen.RangeFrame(x - 2, groundY - height - 1, x + 2, groundY + 1);
            NetMessage.SendTileSquare(-1, x, groundY - height, 1, height);
            return true;
        }

        /// <summary> Adapted clone of the WorldGen.GrowTree method </summary>
        protected bool GrowRegularTree(int x, int y)
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
            if (!groundTile.HasUnactuatedTile || groundTile.IsHalfBlock || groundTile.Slope != 0)
                return false;

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
            if (Main.tenthAnniversaryWorld && !WorldGen.gen && TenthAniversaryRandomColor)
                cache.Color = (byte)WorldGen.genRand.Next(1, 13);

            bool branchFrameLeft = false;
            bool branchFrameRight = false;
            int frameNumber;
            for (int j = groundY - treeHeight; j < groundY; j++)
            {
                // Grow trunk
                Tile treeTile = Main.tile[x, j];
                treeTile.TileFrameNumber = (byte)WorldGen.genRand.Next(3);
                treeTile.HasTile = true;
                treeTile.TileType = Type;
                treeTile.UseBlockColors(cache);

                frameNumber = WorldGen.genRand.Next(3);
                int frame = WorldGen.genRand.Next(10);

                if (j == groundY - 1 || j == groundY - treeHeight)
                    frame = 0;

                while (((frame == 5 || frame == 7) && branchFrameLeft) || ((frame == 6 || frame == 7) && branchFrameRight))
                    frame = WorldGen.genRand.Next(10);

                branchFrameLeft = false;
                branchFrameRight = false;

                if (frame == 5 || frame == 7)
                    branchFrameLeft = true;

                if (frame == 6 || frame == 7)
                    branchFrameRight = true;

                switch (frame)
                {
                    case 1:
                        if (frameNumber == 0)
                        {
                            treeTile.TileFrameX = 0;
                            treeTile.TileFrameY = 66;
                        }
                        if (frameNumber == 1)
                        {
                            treeTile.TileFrameX = 0;
                            treeTile.TileFrameY = 88;
                        }
                        if (frameNumber == 2)
                        {
                            treeTile.TileFrameX = 0;
                            treeTile.TileFrameY = 110;
                        }
                        break;
                    case 2:
                        if (frameNumber == 0)
                        {
                            treeTile.TileFrameX = 22;
                            treeTile.TileFrameY = 0;
                        }
                        if (frameNumber == 1)
                        {
                            treeTile.TileFrameX = 22;
                            treeTile.TileFrameY = 22;
                        }
                        if (frameNumber == 2)
                        {
                            treeTile.TileFrameX = 22;
                            treeTile.TileFrameY = 44;
                        }
                        break;
                    case 3:
                        if (frameNumber == 0)
                        {
                            treeTile.TileFrameX = 44;
                            treeTile.TileFrameY = 66;
                        }
                        if (frameNumber == 1)
                        {
                            treeTile.TileFrameX = 44;
                            treeTile.TileFrameY = 88;
                        }
                        if (frameNumber == 2)
                        {
                            treeTile.TileFrameX = 44;
                            treeTile.TileFrameY = 110;
                        }
                        break;
                    case 4:
                        if (frameNumber == 0)
                        {
                            treeTile.TileFrameX = 22;
                            treeTile.TileFrameY = 66;
                        }
                        if (frameNumber == 1)
                        {
                            treeTile.TileFrameX = 22;
                            treeTile.TileFrameY = 88;
                        }
                        if (frameNumber == 2)
                        {
                            treeTile.TileFrameX = 22;
                            treeTile.TileFrameY = 110;
                        }
                        break;
                    case 5:
                        if (frameNumber == 0)
                        {
                            treeTile.TileFrameX = 88;
                            treeTile.TileFrameY = 0;
                        }
                        if (frameNumber == 1)
                        {
                            treeTile.TileFrameX = 88;
                            treeTile.TileFrameY = 22;
                        }
                        if (frameNumber == 2)
                        {
                            treeTile.TileFrameX = 88;
                            treeTile.TileFrameY = 44;
                        }
                        break;
                    case 6:
                        if (frameNumber == 0)
                        {
                            treeTile.TileFrameX = 66;
                            treeTile.TileFrameY = 66;
                        }
                        if (frameNumber == 1)
                        {
                            treeTile.TileFrameX = 66;
                            treeTile.TileFrameY = 88;
                        }
                        if (frameNumber == 2)
                        {
                            treeTile.TileFrameX = 66;
                            treeTile.TileFrameY = 110;
                        }
                        break;
                    case 7:
                        if (frameNumber == 0)
                        {
                            treeTile.TileFrameX = 110;
                            treeTile.TileFrameY = 66;
                        }
                        if (frameNumber == 1)
                        {
                            treeTile.TileFrameX = 110;
                            treeTile.TileFrameY = 88;
                        }
                        if (frameNumber == 2)
                        {
                            treeTile.TileFrameX = 110;
                            treeTile.TileFrameY = 110;
                        }
                        break;
                    default:
                        if (frameNumber == 0)
                        {
                            treeTile.TileFrameX = 0;
                            treeTile.TileFrameY = 0;
                        }
                        if (frameNumber == 1)
                        {
                            treeTile.TileFrameX = 0;
                            treeTile.TileFrameY = 22;
                        }
                        if (frameNumber == 2)
                        {
                            treeTile.TileFrameX = 0;
                            treeTile.TileFrameY = 44;
                        }
                        break;
                }

                // Add large branches
                if (frame == 5 || frame == 7)
                {
                    treeTile = Main.tile[x - 1, j];
                    treeTile.HasTile = true;
                    treeTile.TileType = Type;
                    treeTile.UseBlockColors(cache);
                    frameNumber = WorldGen.genRand.Next(3);
                    if (WorldGen.genRand.Next(3) < 2)
                    {
                        if (frameNumber == 0)
                        {
                            treeTile.TileFrameX = 44;
                            treeTile.TileFrameY = 198;
                        }
                        if (frameNumber == 1)
                        {
                            treeTile.TileFrameX = 44;
                            treeTile.TileFrameY = 220;
                        }
                        if (frameNumber == 2)
                        {
                            treeTile.TileFrameX = 44;
                            treeTile.TileFrameY = 242;
                        }
                    }
                    else
                    {
                        if (frameNumber == 0)
                        {
                            treeTile.TileFrameX = 66;
                            treeTile.TileFrameY = 0;
                        }
                        if (frameNumber == 1)
                        {
                            treeTile.TileFrameX = 66;
                            treeTile.TileFrameY = 22;
                        }
                        if (frameNumber == 2)
                        {
                            treeTile.TileFrameX = 66;
                            treeTile.TileFrameY = 44;
                        }
                    }
                }

                if (frame == 6 || frame == 7)
                {
                    treeTile = Main.tile[x + 1, j];
                    treeTile.HasTile = true;
                    treeTile.TileType = Type;
                    treeTile.UseBlockColors(cache);

                    frameNumber = WorldGen.genRand.Next(3);

                    if (WorldGen.genRand.Next(3) < 2)
                    {
                        if (frameNumber == 0)
                        {
                            treeTile.TileFrameX = 66;
                            treeTile.TileFrameY = 198;
                        }
                        if (frameNumber == 1)
                        {
                            treeTile.TileFrameX = 66;
                            treeTile.TileFrameY = 220;
                        }
                        if (frameNumber == 2)
                        {
                            treeTile.TileFrameX = 66;
                            treeTile.TileFrameY = 242;
                        }
                    }
                    else
                    {
                        if (frameNumber == 0)
                        {
                            treeTile.TileFrameX = 88;
                            treeTile.TileFrameY = 66;
                        }
                        if (frameNumber == 1)
                        {
                            treeTile.TileFrameX = 88;
                            treeTile.TileFrameY = 88;
                        }
                        if (frameNumber == 2)
                        {
                            treeTile.TileFrameX = 88;
                            treeTile.TileFrameY = 110;
                        }
                    }
                }
            }

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

                frameNumber = WorldGen.genRand.Next(3);
                if (frameNumber == 0)
                {
                    rootTileRight.TileFrameX = 22;
                    rootTileRight.TileFrameY = 132;
                }
                if (frameNumber == 1)
                {
                    rootTileRight.TileFrameX = 22;
                    rootTileRight.TileFrameY = 154;
                }
                if (frameNumber == 2)
                {
                    rootTileRight.TileFrameX = 22;
                    rootTileRight.TileFrameY = 176;
                }
            }

            if (hasRootLeft)
            {
                Tile rootTileLeft = Main.tile[x - 1, groundY - 1];
                rootTileLeft.HasTile = true;

                rootTileLeft.TileType = Type;
                rootTileLeft.UseBlockColors(cache);

                frameNumber = WorldGen.genRand.Next(3);
                if (frameNumber == 0)
                {
                    rootTileLeft.TileFrameX = 44;
                    rootTileLeft.TileFrameY = 132;
                }
                if (frameNumber == 1)
                {
                    rootTileLeft.TileFrameX = 44;
                    rootTileLeft.TileFrameY = 154;
                }
                if (frameNumber == 2)
                {
                    rootTileLeft.TileFrameX = 44;
                    rootTileLeft.TileFrameY = 176;
                }
            }

            // Frame bottom tile based on left/right roots
            frameNumber = WorldGen.genRand.Next(3);
            if (hasRootLeft && hasRootRight)
            {
                if (frameNumber == 0)
                {
                    Main.tile[x, groundY - 1].TileFrameX = 88;
                    Main.tile[x, groundY - 1].TileFrameY = 132;
                }
                if (frameNumber == 1)
                {
                    Main.tile[x, groundY - 1].TileFrameX = 88;
                    Main.tile[x, groundY - 1].TileFrameY = 154;
                }
                if (frameNumber == 2)
                {
                    Main.tile[x, groundY - 1].TileFrameX = 88;
                    Main.tile[x, groundY - 1].TileFrameY = 176;
                }
            }
            else if (hasRootLeft)
            {
                if (frameNumber == 0)
                {
                    Main.tile[x, groundY - 1].TileFrameX = 0;
                    Main.tile[x, groundY - 1].TileFrameY = 132;
                }
                if (frameNumber == 1)
                {
                    Main.tile[x, groundY - 1].TileFrameX = 0;
                    Main.tile[x, groundY - 1].TileFrameY = 154;
                }
                if (frameNumber == 2)
                {
                    Main.tile[x, groundY - 1].TileFrameX = 0;
                    Main.tile[x, groundY - 1].TileFrameY = 176;
                }
            }
            else if (hasRootRight)
            {
                if (frameNumber == 0)
                {
                    Main.tile[x, groundY - 1].TileFrameX = 66;
                    Main.tile[x, groundY - 1].TileFrameY = 132;
                }
                if (frameNumber == 1)
                {
                    Main.tile[x, groundY - 1].TileFrameX = 66;
                    Main.tile[x, groundY - 1].TileFrameY = 154;
                }
                if (frameNumber == 2)
                {
                    Main.tile[x, groundY - 1].TileFrameX = 66;
                    Main.tile[x, groundY - 1].TileFrameY = 176;
                }
            }

            if (!WorldGen.genRand.NextBool(13))
            {
                frameNumber = WorldGen.genRand.Next(3);
                if (frameNumber == 0)
                {
                    Main.tile[x, groundY - treeHeight].TileFrameX = 22;
                    Main.tile[x, groundY - treeHeight].TileFrameY = 198;
                }
                if (frameNumber == 1)
                {
                    Main.tile[x, groundY - treeHeight].TileFrameX = 22;
                    Main.tile[x, groundY - treeHeight].TileFrameY = 220;
                }
                if (frameNumber == 2)
                {
                    Main.tile[x, groundY - treeHeight].TileFrameX = 22;
                    Main.tile[x, groundY - treeHeight].TileFrameY = 242;
                }
            }
            else
            {
                frameNumber = WorldGen.genRand.Next(3);
                if (frameNumber == 0)
                {
                    Main.tile[x, groundY - treeHeight].TileFrameX = 0;
                    Main.tile[x, groundY - treeHeight].TileFrameY = 198;
                }
                if (frameNumber == 1)
                {
                    Main.tile[x, groundY - treeHeight].TileFrameX = 0;
                    Main.tile[x, groundY - treeHeight].TileFrameY = 220;
                }
                if (frameNumber == 2)
                {
                    Main.tile[x, groundY - treeHeight].TileFrameX = 0;
                    Main.tile[x, groundY - treeHeight].TileFrameY = 242;
                }
            }

            WorldGen.RangeFrame(x - 2, groundY - treeHeight - 1, x + 2, groundY + 1);

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendTileSquare(-1, x - 1, groundY - treeHeight, 3, treeHeight);

            return true;
        }

        #region Loot & Shaking
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            int dropItem = ItemID.None;
            int secondaryItem = ItemID.None;
            int dropItemStack = 1;
            bool bonusWood = false;

            Tile tile = Main.tile[i, j];

            if (FramingType == TreeType.Palm)
            {
                dropItem = WoodType;
                //if (Main.tenthAnniversaryWorld)
                //    dropItemStack += WorldGen.genRand.Next(2, 5);

                if (tile.TileFrameX <= 132 && tile.TileFrameX >= 88)
                    secondaryItem = AcornType;

                int k;
                for (k = j; !Main.tile[i, k].HasTile || !Main.tileSolid[Main.tile[i, k].TileType]; k++) { }
                if (Main.tile[i, k].HasTile)
                    dropItem = WoodType;
            }
            else
            {
                if (tile.TileFrameX >= 22 && tile.TileFrameY >= 198)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (WorldGen.genRand.NextBool(2))
                        {
                            int k;
                            for (k = j; !Main.tile[i, k].HasTile || !Main.tileSolid[Main.tile[i, k].TileType] || Main.tileSolidTop[Main.tile[i, k].TileType]; k++) { }
                            if (Main.tile[i, k].HasTile)
                            {
                                dropItem = WoodType;
                                secondaryItem = AcornType;
                            }
                        }
                        else
                        {
                            dropItem = WoodType;
                        }
                    }
                }
                else
                {
                    dropItem = WoodType;
                }
            }

            WorldGen.GetTreeBottom(i, j, out var x, out var y);
            int axe = Utility.GetClosestPlayer(new Vector2(x * 16, y * 16), 16, 16).CurrentItem().axe;

            if (WorldGen.genRand.Next(100) < axe || Main.rand.NextBool(3))
                bonusWood = true;

            if (bonusWood)
                dropItemStack++;

            yield return new Item(dropItem, dropItemStack);
            yield return new Item(secondaryItem);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Main.tile[i, j];
            if (fail && Main.netMode != NetmodeID.MultiplayerClient && TileID.Sets.IsShakeable[tile.TileType])
            {
                (int[] treeShakeX, int[] treeShakeY) = Utility.WorldGen_treeShakeXY;
                if (Utility.WorldGen_numTreeShakes == Utility.WorldGen_maxTreeShakes)
                    return;

                WorldGen.GetTreeBottom(i, j, out var x, out var y);
                for (int k = 0; k < Utility.WorldGen_numTreeShakes; k++)
                {
                    if (treeShakeX[k] == x && treeShakeY[k] == y)
                        return;
                }

                treeShakeX[Utility.WorldGen_numTreeShakes] = x;
                treeShakeY[Utility.WorldGen_numTreeShakes] = y;
                Utility.WorldGen_numTreeShakes = Utility.WorldGen_numTreeShakes + 1;
                y--;

                while (y > 10 && Main.tile[x, y].HasTile && TileID.Sets.IsShakeable[Main.tile[x, y].TileType])
                    y--;

                y++;

                if (!WorldGen.IsTileALeafyTreeTop(x, y) || Collision.SolidTiles(x - 2, x + 2, y - 2, y + 2))
                    return;

                bool createLeaves = true;
                bool result = Shake(x, y, ref createLeaves);
                if (createLeaves)
                {
                    int leaf = TreeLeaf();
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SpecialFX, -1, -1, null, 1, x, y, 1f, leaf);

                    if (Main.netMode == NetmodeID.SinglePlayer)
                        WorldGen.TreeGrowFX(x, y, 1, leaf, hitTree: true);
                }

                if (result && CountsAsTreeType > 0)
                {

                }
            }
        }

        #endregion
        #region Rendering


        private class CustomTreeTopRenderTargetHolder(CustomTree tree) : TilePaintSystemV2.TreeTopRenderTargetHolder
        {
            public override void Prepare()
            {
                Asset<Texture2D> asset = tree.GetTopsTexture(Key.TextureStyle);
                asset.Wait?.Invoke();
                PrepareTextureIfNecessary(asset.Value);
            }

            public override void PrepareShader()
            {
                PrepareShader(Key.PaintColor, tree.PaintingSettings);
            }
        }

        private class CustomTreeBranchRenderTargetHolder(CustomTree tree) : TilePaintSystemV2.TreeBranchTargetHolder
        {
            public override void Prepare()
            {
                Asset<Texture2D> asset = tree.GetBranchTexture(Key.TextureStyle);
                asset.Wait?.Invoke();
                PrepareTextureIfNecessary(asset.Value);
            }

            public override void PrepareShader()
            {
                PrepareShader(Key.PaintColor, tree.PaintingSettings);
            }
        }

        private readonly Dictionary<TilePaintSystemV2.TreeFoliageVariantKey, CustomTreeBranchRenderTargetHolder> treeBranchRenders = new();

        private readonly Dictionary<TilePaintSystemV2.TreeFoliageVariantKey, CustomTreeTopRenderTargetHolder> treeTopRenders = new();

        public Texture2D TryGetTreeTopAndRequestIfNotReady(int x, int y, int paintColor)
        {
            int variant = GetVariant(x, y);
            TilePaintSystemV2.TreeFoliageVariantKey key = new()
            {
                TextureStyle = variant,
                PaintColor = paintColor,
                TextureIndex = Type // We use the tiletype as the index instead of the actual tree index since we can't insert our own index
            };

            if (treeTopRenders.TryGetValue(key, out var value) && value.IsReady)
                return (Texture2D)(object)value.Target;

            value = new CustomTreeTopRenderTargetHolder(this) { Key = key };
            treeTopRenders.TryAdd(key, value);

            if (!value.IsReady)
                Utility.TilePaintSystemV2_AddRequest(value);

            return GetTopsTexture(variant).Value;
        }

        public Texture2D TryGetTreeBranchAndRequestIfNotReady(int x, int y, int paintColor)
        {
            int variant = GetVariant(x, y);
            TilePaintSystemV2.TreeFoliageVariantKey key = new()
            {
                TextureStyle = variant,
                PaintColor = paintColor,
                TextureIndex = Type // We use the tiletype as the index instead of the actual tree index since we can't insert our own index
            };

            if (treeBranchRenders.TryGetValue(key, out var value) && value.IsReady)
                return (Texture2D)(object)value.Target;

            value = new CustomTreeBranchRenderTargetHolder(this) { Key = key };
            treeBranchRenders.TryAdd(key, value);

            if (!value.IsReady)
                Utility.TilePaintSystemV2_AddRequest(value);

            return GetBranchTexture(variant).Value;
        }

        public sealed override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);

            double treeWindCounter = Utility.TreeWindCounter;
            Vector2 unscaledPosition = Main.Camera.UnscaledPosition;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
                zero = Vector2.Zero;

            float topsWindFactor = 0.08f;
            float branchWindFactor = 0.06f;

            Tile tile = Main.tile[i, j];
            if (!tile.HasTile)
                return;

            switch (DrawingType)
            {
                case TreeType.Palm:
                    PostDrawPalmTree(i, j, spriteBatch, tile, treeWindCounter, unscaledPosition, zero, topsWindFactor);
                    break;
                case TreeType.Tree:
                    PostDrawRegularTree(i, j, spriteBatch, tile, treeWindCounter, unscaledPosition, zero, topsWindFactor, branchWindFactor);
                    break;
                default:
                    CustomPostDrawTree(i, j, tile, spriteBatch, treeWindCounter, unscaledPosition, zero, topsWindFactor, branchWindFactor);
                    break;
            }

            spriteBatch.End();
            spriteBatch.Begin(); // No params as PostDraw doesn't use spritebatch with params
        }

        protected void PostDrawPalmTree(int x, int y, SpriteBatch spriteBatch, Tile tile, double treeWindCounter, Vector2 unscaledPosition, Vector2 zero, float topsWindFactor)
        {
            short frameX = tile.TileFrameX;
            bool wallBehind = tile.WallType > 0;

            if (frameX < 88 || frameX > 132)
                return;

            int treeFrame = 0;
            switch (frameX)
            {
                case 110:
                    treeFrame = 1;
                    break;
                case 132:
                    treeFrame = 2;
                    break;
            }

            int xOffset = 0;
            if (!GetTreeFoliageData(x, y, xOffset, ref treeFrame, out int floorY, out int topTextureFrameWidth, out int topTextureFrameHeight))
                return;

            byte tileColor = tile.TileColor;
            int variant = GetVariant(x, y);

            Texture2D treeTopTexture = TryGetTreeTopAndRequestIfNotReady(Type, variant, tileColor);
            Vector2 position = new Vector2(x * 16 - (int)unscaledPosition.X - 42 + Main.tile[x, y].TileFrameY + topTextureFrameWidth / 2, y * 16 - (int)unscaledPosition.Y) + zero;
            float windCycle = 0f;
            if (!wallBehind)
                windCycle = Main.instance.TilesRenderer.GetWindCycle(x, y, treeWindCounter);

            position.X += windCycle * 2f;
            position.Y += Math.Abs(windCycle) * 2f;

            Color color = Lighting.GetColor(x, y);
            if (tile.IsTileFullbright)
                color = Color.White;

            Rectangle frame = new(treeFrame * (topTextureFrameWidth + 2), 0, topTextureFrameWidth, topTextureFrameHeight);
            spriteBatch.Draw(treeTopTexture, position, frame, color, windCycle * topsWindFactor, new Vector2(topTextureFrameWidth / 2, topTextureFrameHeight), 1f, SpriteEffects.None, 0f);
        }

        protected void PostDrawRegularTree(int x, int y, SpriteBatch spriteBatch, Tile tile, double treeWindCounter, Vector2 unscaledPosition, Vector2 zero, float topsWindFactor, float branchWindFactor)
        {
            short frameX = tile.TileFrameX;
            short frameY = tile.TileFrameY;
            bool wallBehind = tile.WallType > 0;

            if (frameY >= 198 && frameX >= 22)
            {
                int treeFrame = WorldGen.GetTreeFrame(tile);
                switch (frameX)
                {
                    case 22:
                        {
                            int xOffset = 0;
                            int grassPosX = x + xOffset;
                            if (!GetTreeFoliageData(x, y, xOffset, ref treeFrame, out int floorY, out int topTextureFrameWidth, out int topTextureFrameHeight))
                                return;

                            EmitLeaves(x, y, grassPosX, floorY);

                            byte tileColor = tile.TileColor;
                            int variant = GetVariant(x, y);

                            Texture2D treeTopTexture = TryGetTreeTopAndRequestIfNotReady(Type, variant, tileColor);
                            Vector2 position = new Vector2(x * 16 - (int)unscaledPosition.X + 8, y * 16 - (int)unscaledPosition.Y + 16) + zero;
                            float windCycle = 0f;

                            if (!wallBehind)
                                windCycle = Main.instance.TilesRenderer.GetWindCycle(x, y, treeWindCounter);

                            position.X += windCycle * 2f;
                            position.Y += Math.Abs(windCycle) * 2f;

                            Color color6 = Lighting.GetColor(x, y);

                            if (tile.IsTileFullbright)
                                color6 = Color.White;

                            Rectangle frame = new(treeFrame * (topTextureFrameWidth + 2), 0, topTextureFrameWidth, topTextureFrameHeight);
                            spriteBatch.Draw(treeTopTexture, position, frame, color6, windCycle * topsWindFactor, new Vector2(topTextureFrameWidth / 2, topTextureFrameHeight), 1f, SpriteEffects.None, 0f);
                            break;
                        }

                    case 44:
                        {
                            int grassPosX = x;
                            int xOffset = 1;
                            if (!GetTreeFoliageData(x, y, xOffset, ref treeFrame, out int floorY2, out _, out _))
                                return;

                            EmitLeaves(x, y, grassPosX + xOffset, floorY2);

                            byte tileColor = tile.TileColor;
                            int variant = GetVariant(x, y);

                            Texture2D treeBranchTexture = TryGetTreeBranchAndRequestIfNotReady(Type, variant, tileColor);
                            Vector2 position = new Vector2(x * 16, y * 16) - unscaledPosition.Floor() + zero + new Vector2(16f, 12f);
                            float windCycle = 0f;

                            if (!wallBehind)
                                windCycle = Main.instance.TilesRenderer.GetWindCycle(x, y, treeWindCounter);

                            if (windCycle > 0f)
                                position.X += windCycle;

                            position.X += Math.Abs(windCycle) * 2f;
                            Color color = Lighting.GetColor(x, y);

                            if (tile.IsTileFullbright)
                                color = Color.White;

                            spriteBatch.Draw(treeBranchTexture, position, (Rectangle?)new Rectangle(0, treeFrame * 42, 40, 40), color, windCycle * branchWindFactor, new Vector2(40f, 24f), 1f, SpriteEffects.None, 0f);
                            break;
                        }

                    case 66:
                        {
                            int grassPosX = x;
                            int xOffset = -1;
                            if (!GetTreeFoliageData(x, y, xOffset, ref treeFrame, out int floorY, out _, out _))
                                return;

                            EmitLeaves(x, y, grassPosX + xOffset, floorY);

                            byte tileColor = tile.TileColor;
                            int variant = GetVariant(x, y);

                            Texture2D treeBranchTexture = TryGetTreeBranchAndRequestIfNotReady(Type, variant, tileColor);
                            Vector2 position = new Vector2(x * 16, y * 16) - unscaledPosition.Floor() + zero + new Vector2(0f, 18f);
                            float windCycle = 0f;

                            if (!wallBehind)
                                windCycle = Main.instance.TilesRenderer.GetWindCycle(x, y, treeWindCounter);

                            if (windCycle < 0f)
                                position.X += windCycle;

                            position.X -= Math.Abs(windCycle) * 2f;
                            Color color = Lighting.GetColor(x, y);

                            if (tile.IsTileFullbright)
                                color = Color.White;

                            spriteBatch.Draw(treeBranchTexture, position, (Rectangle?)new Rectangle(42, treeFrame * 42, 40, 40), color, windCycle * branchWindFactor, new Vector2(0f, 30f), 1f, SpriteEffects.None, 0f);
                            break;
                        }
                }
            }
        }

        protected bool GetTreeFoliageData(int i, int j, int xOffset, ref int treeFrame, out int floorY, out int topTextureFrameWidth, out int topTextureFrameHeight)
        {
            GetTopTextureFrame(i, j, ref treeFrame, out topTextureFrameWidth, out topTextureFrameHeight);

            int x = i + xOffset;
            floorY = j;
            return true;
        }

        public virtual void EmitLeaves(int tilePosX, int tilePosY, int grassPosX, int grassPosY)
        {
            int leaf = TreeLeaf();
            if (leaf < 0)
                return;

            int leafFrequency = Utility.TreeLeafFrequency;
            bool isActiveAndNotPaused = !Main.gamePaused && Main.instance.IsActive;
            UnifiedRandom rand = Utility.TileRendererRandom;

            if (!isActiveAndNotPaused)
                return;

            Tile tile = Main.tile[tilePosX, tilePosY];
            if (tile.LiquidAmount > 0)
                return;

            if (!WorldGen.DoesWindBlowAtThisHeight(tilePosY))
                leafFrequency = 10000;

            bool onGrassPosX = tilePosX - grassPosX != 0;
            if (onGrassPosX)
                leafFrequency *= 3;

            if (!rand.NextBool(leafFrequency))
                return;

            Vector2 position = new(tilePosX * 16 + 8, tilePosY * 16 + 8);
            if (onGrassPosX)
            {
                int offset = tilePosX - grassPosX;
                position.X += offset * 12;
                int num5 = 0;
                if (tile.TileFrameY == 220)
                {
                    num5 = 1;
                }
                else if (tile.TileFrameY == 242)
                {
                    num5 = 2;
                }
                if (tile.TileFrameX == 66)
                {
                    switch (num5)
                    {
                        case 0:
                            position += new Vector2(0f, -6f);
                            break;
                        case 1:
                            position += new Vector2(0f, -6f);
                            break;
                        case 2:
                            position += new Vector2(0f, 8f);
                            break;
                    }
                }
                else
                {
                    switch (num5)
                    {
                        case 0:
                            position += new Vector2(0f, 4f);
                            break;
                        case 1:
                            position += new Vector2(2f, -6f);
                            break;
                        case 2:
                            position += new Vector2(6f, -6f);
                            break;
                    }
                }
            }
            else
            {
                position += new Vector2(-16f, -16f);
            }

            if (!WorldGen.SolidTile(position.ToTileCoordinates()))
            {
                Gore.NewGoreDirect(new EntitySource_Misc(""), position, Terraria.Utils.RandomVector2(Main.rand, -2, 2), leaf, 0.7f + Main.rand.NextFloat() * 0.6f).Frame.CurrentColumn = Main.tile[tilePosX, tilePosY].TileColor;
            }
        }
        #endregion
    }
}
