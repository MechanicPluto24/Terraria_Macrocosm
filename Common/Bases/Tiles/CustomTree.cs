using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
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
    public abstract partial class CustomTree : ModTile
    {
        public enum TreeCategory
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

        public virtual TreeCategory FramingMode => TreeCategory.Tree;
        public virtual TreeCategory DrawingMode => TreeCategory.Tree;

        public virtual int WoodType => ItemID.Wood;
        public virtual int AcornType => ItemID.Acorn;

        /// <summary> For tree shake purposes </summary>
        public virtual TreeTypes CountsAsTreeType => TreeTypes.None;

        /// <summary> Tree leaf gore type </summary>
        public virtual int TreeLeaf => -1;

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

        /// <summary> Determine tree top frame size. Also allows you to modify the <paramref name="treeFrame"/> </summary>
        public abstract void GetFoliageData(int i, int j, ref int treeFrame, out int topTextureFrameWidth, out int topTextureFrameHeight);

        /// <summary> Check ground tile valid for this tree. Defaults to a <see cref="GrowsOnTileId"/> check </summary>
        public virtual bool GroundTest(int tileType) => GrowsOnTileId.Contains(tileType) || tileType == Type;

        /// <summary> Check wall behing valid for this tree. Defaults to vanilla logic </summary>
        public virtual bool WallTest(int wallType) => WorldGen.DefaultTreeWallTest(wallType);

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
            TileID.Sets.PreventsTileHammeringIfOnTopOfIt[Type] = true;

            TileSets.PaintingSettings[Type] = PaintingSettings;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            width = 20;
            height = 20;
        }

        /// <summary> Use for custom tile frame (<see cref="FramingMode"/> == <see cref="TreeCategory.Custom"/>)</summary>
        public virtual void CustomTileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) { }

        public sealed override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];
            if (i > 5 && j > 5 && i < Main.maxTilesX - 5 && j < Main.maxTilesY - 5 && tile.HasTile && Main.tileFrameImportant[Type])
            {
                switch (FramingMode)
                {
                    case TreeCategory.Tree:
                        WorldGen.CheckTreeWithSettings(i, j, new WorldGen.CheckTreeSettings() { IsGroundValid = GroundTest });
                        break;
                    case TreeCategory.Palm:
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

        /// <summary> Use for growing (<see cref="FramingMode"/> == <see cref="TreeCategory.Custom"/>)</summary>
        public virtual bool CustomGrowTree(int i, int j) { return false; }

        /// <summary>
        /// Grow a tree of this type. 
        /// <br/> You can also use <see cref="WorldGen.TryGrowingTreeByType(int, int, int)"/> and <see cref="WorldGen.AttemptToGrowTreeFromSapling(int, int, bool)"/>
        /// </summary>
        public bool GrowTree(int x, int y)
        {
            var result = FramingMode switch
            {
                TreeCategory.Tree => GrowRegularTree(x, y),
                TreeCategory.Palm => GrowPalmTree(x, y),
                _ => CustomGrowTree(x, y),
            };

            if (result && WorldGen.PlayerLOS(x, y))
                LeafEffects(x, y);

            return result;
        }

        public virtual void LeafEffects(int x, int y, bool hitOnly = false)
        {
            int leaf = TreeLeaf;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SpecialFX, -1, -1, null, 1, x, y, 1f, TreeLeaf);

            if (Main.netMode == NetmodeID.SinglePlayer)
                WorldGen.TreeGrowFX(x, y, 1, TreeLeaf, hitOnly);
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

        public virtual IEnumerable<Item> GetExtraItemDrops(int i, int j)
        {
            yield break;
        }

        public sealed override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            int dropItem = ItemID.None;
            int secondaryItem = ItemID.None;
            int dropItemStack = 1;
            bool bonusWood = false;

            Tile tile = Main.tile[i, j];

            if (FramingMode == TreeCategory.Palm)
            {
                dropItem = WoodType;
                if (Main.tenthAnniversaryWorld)
                    dropItemStack += WorldGen.genRand.Next(2, 5);

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

            foreach (var item in GetExtraItemDrops(i, j))
                yield return item;
        }

        public virtual bool IsTileALeafyTreeTop(int i, int j) => WorldGen.IsTileALeafyTreeTop(i, j);

        /// <summary> Shake logic. Return true to also allow vanilla shake logic to run, based on <see cref="CountsAsTreeType"/> </summary>
        public virtual bool Shake(int x, int y, IEntitySource source, ref bool createLeaves) => true;

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

                if (!IsTileALeafyTreeTop(x, y) || Collision.SolidTiles(x - 2, x + 2, y - 2, y + 2))
                    return;

                bool createLeaves = true;
                bool result = Shake(x, y, WorldGen.GetItemSource_FromTreeShake(x, y), ref createLeaves);
                if (createLeaves)
                    LeafEffects(x, y, hitOnly: true);

                if (result && CountsAsTreeType > 0)
                    Utility.WorldGen_ShakeTree(i, j);
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
                TileRendering.AddTilePaintRequest(value);

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
                TileRendering.AddTilePaintRequest(value);

            return GetBranchTexture(variant).Value;
        }

        public virtual bool ShouldTileDrawFoliage(int i, int j, short tileFrameX, short tileFrameY)
        {
            return DrawingMode switch
            {
                TreeCategory.Tree => tileFrameX >= 22 && tileFrameY >= 198,
                TreeCategory.Palm => tileFrameX >= 88 && tileFrameX <= 132,
                _ => false,
            };
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (ShouldTileDrawFoliage(i, j, drawData.tileFrameX, drawData.tileFrameY))
                Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (!tile.HasTile)
                return;

            DrawTreeFoliage(i, j, spriteBatch, TileRendering.TreeWindCounter, Main.Camera.UnscaledPosition);
        }

        public virtual void DrawTreeFoliage(int i, int j, SpriteBatch spriteBatch, double treeWindCounter, Vector2 unscaledPosition, float topsWindFactor = 0.08f, float branchWindFactor = 0.06f)
        {
            Tile tile = Main.tile[i, j];
            if (DrawingMode == TreeCategory.Tree)
            {
                int treeFrame = WorldGen.GetTreeFrame(tile);
                switch (tile.TileFrameX)
                {
                    case 22:
                        {
                            GetFoliageData(i, j, ref treeFrame, out int topTextureFrameWidth, out int topTextureFrameHeight);
                            EmitLeaves_RegularTree(i, j, foliagePosX: i);

                            byte tileColor = tile.TileColor;
                            int variant = GetVariant(i, j);

                            Texture2D treeTopTexture = TryGetTreeTopAndRequestIfNotReady(Type, variant, tileColor);
                            Vector2 position = new(i * 16 - (int)unscaledPosition.X + 8, j * 16 - (int)unscaledPosition.Y + 16);
                            float windCycle = 0f;

                            if (tile.WallType == WallID.None)
                                windCycle = Main.instance.TilesRenderer.GetWindCycle(i, j, treeWindCounter);

                            position.X += windCycle * 2f;
                            position.Y += Math.Abs(windCycle) * 2f;

                            Color color6 = Lighting.GetColor(i, j);

                            if (tile.IsTileFullbright)
                                color6 = Color.White;

                            Rectangle frame = new(treeFrame * (topTextureFrameWidth + 2), 0, topTextureFrameWidth, topTextureFrameHeight);
                            spriteBatch.Draw(treeTopTexture, position, frame, color6, windCycle * topsWindFactor, new Vector2(topTextureFrameWidth / 2, topTextureFrameHeight), 1f, SpriteEffects.None, 0f);
                            break;
                        }

                    case 44:
                        {
                            GetFoliageData(i, j, ref treeFrame, out _, out _);
                            EmitLeaves_RegularTree(i, j, foliagePosX: i + 1);

                            byte tileColor = tile.TileColor;
                            int variant = GetVariant(i, j);

                            Texture2D treeBranchTexture = TryGetTreeBranchAndRequestIfNotReady(Type, variant, tileColor);
                            Vector2 position = new Vector2(i * 16, j * 16) - unscaledPosition.Floor() + new Vector2(16f, 12f);
                            float windCycle = 0f;

                            if (tile.WallType == WallID.None)
                                windCycle = Main.instance.TilesRenderer.GetWindCycle(i, j, treeWindCounter);

                            if (windCycle > 0f)
                                position.X += windCycle;

                            position.X += Math.Abs(windCycle) * 2f;
                            Color color = Lighting.GetColor(i, j);

                            if (tile.IsTileFullbright)
                                color = Color.White;

                            spriteBatch.Draw(treeBranchTexture, position, (Rectangle?)new Rectangle(0, treeFrame * 42, 40, 40), color, windCycle * branchWindFactor, new Vector2(40f, 24f), 1f, SpriteEffects.None, 0f);
                            break;
                        }

                    case 66:
                        {
                            GetFoliageData(i, j, ref treeFrame, out _, out _);
                            EmitLeaves_RegularTree(i, j, foliagePosX: i - 1);

                            byte tileColor = tile.TileColor;
                            int variant = GetVariant(i, j);

                            Texture2D treeBranchTexture = TryGetTreeBranchAndRequestIfNotReady(Type, variant, tileColor);
                            Vector2 position = new Vector2(i * 16, j * 16) - unscaledPosition.Floor() + new Vector2(0f, 18f);
                            float windCycle = 0f;

                            if (tile.WallType == WallID.None)
                                windCycle = Main.instance.TilesRenderer.GetWindCycle(i, j, treeWindCounter);

                            if (windCycle < 0f)
                                position.X += windCycle;

                            position.X -= Math.Abs(windCycle) * 2f;
                            Color color = Lighting.GetColor(i, j);

                            if (tile.IsTileFullbright)
                                color = Color.White;

                            spriteBatch.Draw(treeBranchTexture, position, (Rectangle?)new Rectangle(42, treeFrame * 42, 40, 40), color, windCycle * branchWindFactor, new Vector2(0f, 30f), 1f, SpriteEffects.None, 0f);
                            break;
                        }
                }
            }
            else if (DrawingMode == TreeCategory.Palm)
            {
                int treeFrame = tile.TileFrameX switch
                {
                    110 => 1,
                    132 => 2,
                    _ => 0,
                };

                GetFoliageData(i, j, ref treeFrame, out int topTextureFrameWidth, out int topTextureFrameHeight);

                byte tileColor = tile.TileColor;
                int variant = GetVariant(i, j);

                Texture2D treeTopTexture = TryGetTreeTopAndRequestIfNotReady(Type, variant, tileColor);
                Vector2 position = new(i * 16 - (int)unscaledPosition.X - 42 + Main.tile[i, j].TileFrameY + topTextureFrameWidth / 2, j * 16 - (int)unscaledPosition.Y);
                float windCycle = 0f;
                if (tile.WallType == WallID.None)
                    windCycle = Main.instance.TilesRenderer.GetWindCycle(i, j, treeWindCounter);

                position.X += windCycle * 2f;
                position.Y += Math.Abs(windCycle) * 2f;

                Color color = Lighting.GetColor(i, j);
                if (tile.IsTileFullbright)
                    color = Color.White;

                Rectangle frame = new(treeFrame * (topTextureFrameWidth + 2), 0, topTextureFrameWidth, topTextureFrameHeight);
                spriteBatch.Draw(treeTopTexture, position, frame, color, windCycle * topsWindFactor, new Vector2(topTextureFrameWidth / 2, topTextureFrameHeight), 1f, SpriteEffects.None, 0f);
            }
        }
        #endregion

        #region Particles

        /// <summary> 
        /// Used for emitting leaves and other particles. 
        /// <br/> Runs on ALL tiles, when game is active and not paused, only on non-<see cref="TreeCategory.Tree"/> types. 
        /// <br/> Defaults to <see langword="false"/>. 
        /// </summary>
        public virtual bool CanEmitLeaves(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible, int leafFrequency, UnifiedRandom rand, out Vector2 offset, out Vector2? velocityOverride, out float? scaleOverride)
        {
            offset = Vector2.Zero;
            velocityOverride = null;
            scaleOverride = null;
            return FramingMode == TreeCategory.Tree && DrawingMode == TreeCategory.Tree; // Regular trees can emit by default, under custom logic. 
        }

        public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
        {
            if (ShouldEmitDefaultLeaves(i, j)) // Regular trees handled by foliage renderer
                return;

            if (tile.LiquidAmount > 0)
                return;

            int leafFrequency = TileRendering.TreeLeafFrequency;
            if (!WorldGen.DoesWindBlowAtThisHeight(j))
                leafFrequency = 10000;

            if (CanEmitLeaves(i, j, tile, tileFrameX, tileFrameY, tileLight, visible, leafFrequency, TileRendering.TileRendererRandom, out Vector2 offset, out Vector2? velocityOverride, out float? scaleOverride))
            {
                Vector2 position = new Vector2(i * 16 + 8, j * 16 + 8) + offset;
                Vector2 velocity = velocityOverride ?? Terraria.Utils.RandomVector2(Main.rand, -2f, 2f);
                float scale = scaleOverride ?? 0.7f + Main.rand.NextFloat() * 0.6f;
                EmitLeaf(position, velocity, scale, tile.TileColor);
            }
        }

        protected virtual void EmitLeaf(Vector2 position, Vector2 velocity, float scale, byte tileColor)
        {
            Gore gore = Gore.NewGoreDirect(new EntitySource_Misc(""), position, velocity, TreeLeaf, scale);
            gore.Frame.CurrentColumn = tileColor;
        }

        protected bool ShouldEmitDefaultLeaves(int i, int j) => FramingMode == TreeCategory.Tree && DrawingMode == TreeCategory.Tree && TreeLeaf >= 0;
        private void EmitLeaves_RegularTree(int i, int j, int foliagePosX = 0)
        {
            if (!ShouldEmitDefaultLeaves(i, j))
                return;

            Tile tile = Main.tile[i, j];
            if (!Main.instance.IsActive || Main.gamePaused || tile.LiquidAmount > 0)
                return;

            int leafFrequency = TileRendering.TreeLeafFrequency;
            UnifiedRandom rand = TileRendering.TileRendererRandom;

            if (!WorldGen.DoesWindBlowAtThisHeight(j))
                leafFrequency = 10000;

            bool notCentered = i - foliagePosX != 0;
            if (notCentered)
                leafFrequency *= 3;

            if (!rand.NextBool(leafFrequency))
                return;

            Vector2 position = new(i * 16 + 8, j * 16 + 8);

            if (notCentered)
            {
                int offset = i - foliagePosX;
                position.X += offset * 12;
                position += tile.TileFrameX == 66 
                    ? tile.TileFrameY == 242  ? new Vector2(0, -6) : new Vector2(0, 8)
                    : tile.TileFrameY switch {
                        220 => new Vector2(2f, -6f),
                        242 => new Vector2(6f, -6f),
                        _ => new Vector2(0f, 4f)
                    };
            }
            else
            {
                position += new Vector2(-16f, -16f);
            }

            if (!WorldGen.SolidTile(position.ToTileCoordinates()))
            {
                Vector2 velocity = Terraria.Utils.RandomVector2(Main.rand, -2f, 2f);
                float scale = 0.7f + Main.rand.NextFloat() * 0.6f;
                EmitLeaf(position, velocity, scale, tile.TileColor);
            }
        }

        #endregion
    }
}
