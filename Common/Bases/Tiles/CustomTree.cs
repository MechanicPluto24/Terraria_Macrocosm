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
    public abstract class CustomTree : ModTile, ITree
    {
        public override void Load()
        {
            On_WorldGen.TryGrowingTreeByType += On_WorldGen_TryGrowingTreeByType;
            On_WorldGen.GetTreeType += On_WorldGen_GetTreeType; ;
        }

        public override void Unload()
        {
            On_WorldGen.TryGrowingTreeByType -= On_WorldGen_TryGrowingTreeByType;
            On_WorldGen.GetTreeType -= On_WorldGen_GetTreeType; ;
        }

        /// <summary> Array of valid tile types. </summary>
        public abstract int[] GrowsOnTileId { get; set; }

        /// <summary> Min tree height in tiles </summary>
        public abstract int TreeHeightMin { get; }

        /// <summary> Max tree height in tiles </summary>
        public abstract int TreeHeightMax { get; }

        /// <summary> Padding for the tree top </summary>
        public abstract int TreeTopPaddingNeeded { get; }

        public virtual int WoodType => ItemID.Wood;
        public virtual int AcornType => ItemID.Acorn;

        /// <summary> For tree shake purposes </summary>
        public TreeTypes CountsAsTreeType => TreeTypes.None;
        public int VanillaCount => 0;
        public virtual int PlantTileId => Type;

        /// <summary> Grow with branches or not </summary>
        public virtual bool AllowBranches => true;

        /// <summary> Random paint on the CelebrationMK10 seed </summary>
        public virtual bool TenthAniversaryRandomColor => false;

        public virtual TreePaintingSettings TreePaintingSettings { get; } = new()
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        /// <summary> The sapling tile type and style </summary>
        public virtual int SaplingGrowthType(ref int style)
        {
            style = 1;
            return TileID.Saplings;
        }

        /// <summary> Determine variant at tile coordinates. Defaults to a single variant (0) </summary>
        public virtual int GetVariant(int x, int y) => 0;

        /// <summary> Called when destroyed </summary>
        public virtual void Harvest(bool bonusWood) { }

        /// <summary> Determine tree top frame size. Allows you to modify the <paramref name="treeFrame"/> </summary>
        protected abstract void GetTopTextureFrame(int i, int j, ref int treeFrame, out int topTextureFrameWidth, out int topTextureFrameHeight);

        /// <summary> Check ground tile valid for this tree. Defaults to a <see cref="GrowsOnTileId"/> check </summary>
        public virtual bool GroundTest(int tileType) => GrowsOnTileId.Contains(tileType);

        /// <summary> Check wall behing valid for this tree. Defaults to vanilla logic </summary>
        public virtual bool WallTest(int wallType) => WorldGen.DefaultTreeWallTest(wallType);

        /// <summary> Tree leaf gore type </summary>
        public virtual int TreeLeaf() => -1;

        /// <summary> Shake logic. Return true to also allow vanilla shake logic to run, based on <see cref="CountsAsTreeType"/> </summary>
        public virtual bool Shake(int x, int y, ref bool createLeaves) => true;

        public virtual Asset<Texture2D> GetTexture() => ModContent.RequestIfExists<Texture2D>(Texture, out var texture) ? texture : null;

        /// <summary> Use this to get a custom tops texture. Autoloaded with a <c>"_Tops"</c> suffix, and <c>"_Tops_X"</c> for extra >0 variants; defaults to an empty texture if not existent </summary>
        public virtual Asset<Texture2D> GetTopsTexture(int variant) => ModContent.RequestIfExists<Texture2D>(Texture + "_Tops" + (variant > 0 ? $"_{variant}" : ""), out var texture) ? texture : Macrocosm.EmptyTex;

        /// <summary> Use this to get a custom branch texture. Autoloaded with a <c>"_Branches"</c> suffix, and <c>"_Branches_X"</c> for extra >0 variants; defaults to an empty texture if not existent </summary>
        public virtual Asset<Texture2D> GetBranchTexture(int variant) => ModContent.RequestIfExists<Texture2D>(Texture + "_Branches" + (variant > 0 ? $"_{variant}" : ""), out var texture) ? texture : Macrocosm.EmptyTex;

        private static MethodInfo worldGen_ShakeTree_methodInfo;
        private static FieldInfo worldGen_numTreeShakes_fieldInfo;
        private static FieldInfo worldGen_maxTreeShakes_fieldInfo;
        private static FieldInfo worldGen_treeShakeX_fieldInfo;
        private static FieldInfo worldGen_treeShakeY_fieldInfo;
        private static FieldInfo tilePaintSystemV2_requests_fieldInfo;
        private static FieldInfo tileDrawing_treeWindCounter_fieldInfo;
        private static FieldInfo tileDrawing_isActiveAndNotPaused_fieldInfo;
        private static FieldInfo tileDrawing_leafFrequency_fieldInfo;
        private static FieldInfo tileDrawing_rand_fieldInfo;

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
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            width = 20;
            height = 20;

            if (tileFrameX >= 176)
                tileFrameX = (short)(tileFrameX - 176);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];
            if (i > 5 && j > 5 && i < Main.maxTilesX - 5 && j < Main.maxTilesY - 5 && Main.tile[i, j] != null)
            {
                if (tile.HasTile)
                {
                    if (Main.tileFrameImportant[Type])
                    {
                        WorldGen.CheckTree(i, j);
                    }
                }
            }

            return false;
        }

        private bool On_WorldGen_TryGrowingTreeByType(On_WorldGen.orig_TryGrowingTreeByType orig, int treeTileType, int checkedX, int checkedY)
        {
            if (TileLoader.GetTile(treeTileType) is CustomTree customTree)
                return customTree.GrowTree(checkedX, checkedY);

            return orig(treeTileType, checkedX, checkedY);
        }

        /// <summary>
        /// Grow a tree of this type. You can also use <see cref="WorldGen.TryGrowingTreeByType(int, int, int)"/>
        /// </summary>
        public bool GrowTree(int x, int y)
        {
            int style = 0;
            WorldGen.GrowTreeSettings settings = new()
            {
                GroundTest = GroundTest,
                WallTest = WallTest,
                TreeHeightMax = TreeHeightMax,
                TreeHeightMin = TreeHeightMin,
                TreeTileType = Type,
                TreeTopPaddingNeeded = TreeTopPaddingNeeded,
                SaplingTileType = (ushort)SaplingGrowthType(ref style)
            };

            int tileY;
            for (tileY = y; Main.tile[x, tileY].TileType == settings.SaplingTileType; tileY++) { }

            if (Main.tile[x - 1, tileY - 1].LiquidAmount != 0 || Main.tile[x, tileY - 1].LiquidAmount != 0 || Main.tile[x + 1, tileY - 1].LiquidAmount != 0)
                return false;

            Tile groundTile = Main.tile[x, tileY];
            if (!groundTile.HasUnactuatedTile || groundTile.IsHalfBlock || groundTile.Slope != 0)
                return false;

            bool wall = settings.WallTest(Main.tile[x, tileY - 1].WallType);
            if (!settings.GroundTest(groundTile.TileType) || !wall)
                return false;

            if ((!Main.tile[x - 1, tileY].HasTile || !settings.GroundTest(Main.tile[x - 1, tileY].TileType)) && (!Main.tile[x + 1, tileY].HasTile || !settings.GroundTest(Main.tile[x + 1, tileY].TileType)))
                return false;

            TileColorCache cache = Main.tile[x, tileY].BlockColorAndCoating();
            if (Main.tenthAnniversaryWorld && !WorldGen.gen && TenthAniversaryRandomColor)
                cache.Color = (byte)WorldGen.genRand.Next(1, 13);

            int treeHeight = WorldGen.genRand.Next(settings.TreeHeightMin, settings.TreeHeightMax + 1);
            int paddedHeight = treeHeight + settings.TreeTopPaddingNeeded;

            if (!WorldGen.EmptyTileCheck(x - 2, x + 2, tileY - paddedHeight, tileY - 1, settings.SaplingTileType))
                return false;

            bool branchRootFrameLeft = false;
            bool branchRootFrameRight = false;
            int frameNumber;
            for (int j = tileY - treeHeight; j < tileY; j++)
            {
                Tile treeTile = Main.tile[x, j];
                treeTile.TileFrameNumber = (byte)WorldGen.genRand.Next(3);
                treeTile.HasTile = true;
                treeTile.TileType = settings.TreeTileType;
                treeTile.UseBlockColors(cache);

                frameNumber = WorldGen.genRand.Next(3);
                int frame = WorldGen.genRand.Next(10);

                if (j == tileY - 1 || j == tileY - treeHeight)
                    frame = 0;

                while (((frame == 5 || frame == 7) && branchRootFrameLeft) || ((frame == 6 || frame == 7) && branchRootFrameRight))
                    frame = WorldGen.genRand.Next(10);

                branchRootFrameLeft = false;
                branchRootFrameRight = false;

                if (AllowBranches)
                {
                    if (frame == 5 || frame == 7)
                        branchRootFrameLeft = true;

                    if (frame == 6 || frame == 7)
                        branchRootFrameRight = true;
                }

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

                if (frame == 5 || frame == 7)
                {
                    treeTile = Main.tile[x - 1, j];
                    treeTile.HasTile = true;
                    treeTile.TileType = settings.TreeTileType;
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
                    treeTile.TileType = settings.TreeTileType;
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

            if (AllowBranches)
            {
                bool hasBranchLeft = false;
                bool hasBranchRight = false;

                if (Main.tile[x - 1, tileY].HasUnactuatedTile && !Main.tile[x - 1, tileY].IsHalfBlock && Main.tile[x - 1, tileY].Slope == 0 && WorldGen.IsTileTypeFitForTree(Main.tile[x - 1, tileY].TileType))
                    hasBranchLeft = true;

                if (Main.tile[x + 1, tileY].HasUnactuatedTile && !Main.tile[x + 1, tileY].IsHalfBlock && Main.tile[x + 1, tileY].Slope == 0 && WorldGen.IsTileTypeFitForTree(Main.tile[x + 1, tileY].TileType))
                    hasBranchRight = true;

                if (WorldGen.genRand.NextBool(3))
                    hasBranchLeft = false;

                if (WorldGen.genRand.NextBool(3))
                    hasBranchRight = false;

                if (hasBranchRight)
                {
                    Tile branchTile = Main.tile[x + 1, tileY - 1];
                    branchTile.HasTile = true;

                    Main.tile[x + 1, tileY - 1].TileType = settings.TreeTileType;
                    Main.tile[x + 1, tileY - 1].UseBlockColors(cache);

                    frameNumber = WorldGen.genRand.Next(3);
                    if (frameNumber == 0)
                    {
                        Main.tile[x + 1, tileY - 1].TileFrameX = 22;
                        Main.tile[x + 1, tileY - 1].TileFrameY = 132;
                    }
                    if (frameNumber == 1)
                    {
                        Main.tile[x + 1, tileY - 1].TileFrameX = 22;
                        Main.tile[x + 1, tileY - 1].TileFrameY = 154;
                    }
                    if (frameNumber == 2)
                    {
                        Main.tile[x + 1, tileY - 1].TileFrameX = 22;
                        Main.tile[x + 1, tileY - 1].TileFrameY = 176;
                    }
                }

                if (hasBranchLeft)
                {
                    Tile branchTile = Main.tile[x - 1, tileY - 1];
                    branchTile.HasTile = true;

                    Main.tile[x - 1, tileY - 1].TileType = settings.TreeTileType;
                    Main.tile[x - 1, tileY - 1].UseBlockColors(cache);

                    frameNumber = WorldGen.genRand.Next(3);
                    if (frameNumber == 0)
                    {
                        Main.tile[x - 1, tileY - 1].TileFrameX = 44;
                        Main.tile[x - 1, tileY - 1].TileFrameY = 132;
                    }
                    if (frameNumber == 1)
                    {
                        Main.tile[x - 1, tileY - 1].TileFrameX = 44;
                        Main.tile[x - 1, tileY - 1].TileFrameY = 154;
                    }
                    if (frameNumber == 2)
                    {
                        Main.tile[x - 1, tileY - 1].TileFrameX = 44;
                        Main.tile[x - 1, tileY - 1].TileFrameY = 176;
                    }
                }

                frameNumber = WorldGen.genRand.Next(3);
                if (hasBranchLeft && hasBranchRight)
                {
                    if (frameNumber == 0)
                    {
                        Main.tile[x, tileY - 1].TileFrameX = 88;
                        Main.tile[x, tileY - 1].TileFrameY = 132;
                    }
                    if (frameNumber == 1)
                    {
                        Main.tile[x, tileY - 1].TileFrameX = 88;
                        Main.tile[x, tileY - 1].TileFrameY = 154;
                    }
                    if (frameNumber == 2)
                    {
                        Main.tile[x, tileY - 1].TileFrameX = 88;
                        Main.tile[x, tileY - 1].TileFrameY = 176;
                    }
                }
                else if (hasBranchLeft)
                {
                    if (frameNumber == 0)
                    {
                        Main.tile[x, tileY - 1].TileFrameX = 0;
                        Main.tile[x, tileY - 1].TileFrameY = 132;
                    }
                    if (frameNumber == 1)
                    {
                        Main.tile[x, tileY - 1].TileFrameX = 0;
                        Main.tile[x, tileY - 1].TileFrameY = 154;
                    }
                    if (frameNumber == 2)
                    {
                        Main.tile[x, tileY - 1].TileFrameX = 0;
                        Main.tile[x, tileY - 1].TileFrameY = 176;
                    }
                }
                else if (hasBranchRight)
                {
                    if (frameNumber == 0)
                    {
                        Main.tile[x, tileY - 1].TileFrameX = 66;
                        Main.tile[x, tileY - 1].TileFrameY = 132;
                    }
                    if (frameNumber == 1)
                    {
                        Main.tile[x, tileY - 1].TileFrameX = 66;
                        Main.tile[x, tileY - 1].TileFrameY = 154;
                    }
                    if (frameNumber == 2)
                    {
                        Main.tile[x, tileY - 1].TileFrameX = 66;
                        Main.tile[x, tileY - 1].TileFrameY = 176;
                    }
                }
            }

            if (!WorldGen.genRand.NextBool(13))
            {
                frameNumber = WorldGen.genRand.Next(3);
                if (frameNumber == 0)
                {
                    Main.tile[x, tileY - treeHeight].TileFrameX = 22;
                    Main.tile[x, tileY - treeHeight].TileFrameY = 198;
                }
                if (frameNumber == 1)
                {
                    Main.tile[x, tileY - treeHeight].TileFrameX = 22;
                    Main.tile[x, tileY - treeHeight].TileFrameY = 220;
                }
                if (frameNumber == 2)
                {
                    Main.tile[x, tileY - treeHeight].TileFrameX = 22;
                    Main.tile[x, tileY - treeHeight].TileFrameY = 242;
                }
            }
            else
            {
                frameNumber = WorldGen.genRand.Next(3);
                if (frameNumber == 0)
                {
                    Main.tile[x, tileY - treeHeight].TileFrameX = 0;
                    Main.tile[x, tileY - treeHeight].TileFrameY = 198;
                }
                if (frameNumber == 1)
                {
                    Main.tile[x, tileY - treeHeight].TileFrameX = 0;
                    Main.tile[x, tileY - treeHeight].TileFrameY = 220;
                }
                if (frameNumber == 2)
                {
                    Main.tile[x, tileY - treeHeight].TileFrameX = 0;
                    Main.tile[x, tileY - treeHeight].TileFrameY = 242;
                }
            }

            WorldGen.RangeFrame(x - 2, tileY - treeHeight - 1, x + 2, tileY + 1);

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendTileSquare(-1, x - 1, tileY - treeHeight, 3, treeHeight);

            int leaf = TreeLeaf();
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SpecialFX, -1, -1, null, 1, x, y, 1f, leaf);

            if (Main.netMode == NetmodeID.SinglePlayer)
                WorldGen.TreeGrowFX(x, y, 1, leaf, hitTree: true);

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

            WorldGen.GetTreeBottom(i, j, out var x, out var y);
            int axe = Utility.GetClosestPlayer(new Vector2(x * 16, y * 16), 16, 16).CurrentItem().axe;

            if (WorldGen.genRand.Next(100) < axe || Main.rand.NextBool(3))
                bonusWood = true;

            if (bonusWood)
                dropItemStack++;

            Harvest(bonusWood);

            yield return new Item(dropItem, dropItemStack);
            yield return new Item(secondaryItem);
        }

        private TreeTypes On_WorldGen_GetTreeType(On_WorldGen.orig_GetTreeType orig, int tileType)
        {
            if (TileLoader.GetTile(tileType) is CustomTree customTree)
                return customTree.CountsAsTreeType;

            return orig(tileType);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Main.tile[i, j];
            if (fail)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && TileID.Sets.IsShakeable[tile.TileType])
                {
                    if (ShakeTree(i, j) && CountsAsTreeType > 0)
                    {
                        worldGen_ShakeTree_methodInfo ??= typeof(WorldGen).GetMethod("ShakeTree", BindingFlags.NonPublic | BindingFlags.Static);
                        worldGen_ShakeTree_methodInfo.Invoke(null, [i, j]);
                    }
                }
            }
        }

        public bool ShakeTree(int i, int j)
        {
            worldGen_numTreeShakes_fieldInfo ??= typeof(WorldGen).GetField("numTreeShakes", BindingFlags.NonPublic | BindingFlags.Static);
            worldGen_maxTreeShakes_fieldInfo ??= typeof(WorldGen).GetField("maxTreeShakes", BindingFlags.NonPublic | BindingFlags.Static);
            worldGen_treeShakeX_fieldInfo ??= typeof(WorldGen).GetField("treeShakeX", BindingFlags.NonPublic | BindingFlags.Static);
            worldGen_treeShakeY_fieldInfo ??= typeof(WorldGen).GetField("treeShakeY", BindingFlags.NonPublic | BindingFlags.Static);

            int numTreeShakes = (int)worldGen_numTreeShakes_fieldInfo.GetValue(null);
            int maxTreeShakes = (int)worldGen_maxTreeShakes_fieldInfo.GetValue(null);
            int[] treeShakeX = (int[])worldGen_treeShakeY_fieldInfo.GetValue(null);
            int[] treeShakeY = (int[])worldGen_treeShakeY_fieldInfo.GetValue(null);

            if (numTreeShakes == maxTreeShakes)
                return false;

            WorldGen.GetTreeBottom(i, j, out var x, out var y);
            for (int k = 0; k < numTreeShakes; k++)
            {
                if (treeShakeX[k] == x && treeShakeY[k] == y)
                    return false;
            }

            treeShakeX[numTreeShakes] = x;
            treeShakeY[numTreeShakes] = y;
            worldGen_numTreeShakes_fieldInfo.SetValue(null, ++numTreeShakes);
            y--;

            while (y > 10 && Main.tile[x, y].HasTile && TileID.Sets.IsShakeable[Main.tile[x, y].TileType])
                y--;

            y++;

            if (!WorldGen.IsTileALeafyTreeTop(x, y) || Collision.SolidTiles(x - 2, x + 2, y - 2, y + 2))
                return false;

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

            return result;
        }
        #endregion

        #region Rendering
        private class CustomTreeTopRenderTargetHolder(CustomTree tree) : TilePaintSystemV2.ARenderTargetHolder
        {
            public TilePaintSystemV2.TreeFoliageVariantKey Key;

            public override void Prepare()
            {
                Asset<Texture2D> asset = tree.GetTopsTexture(Key.TextureIndex);
                asset.Wait?.Invoke();
                PrepareTextureIfNecessary(asset.Value);
            }

            public override void PrepareShader()
            {
                PrepareShader(Key.PaintColor, tree.TreePaintingSettings);
            }
        }
        private class CustomTreeBranchTargetHolder(CustomTree tree) : TilePaintSystemV2.ARenderTargetHolder
        {
            public TilePaintSystemV2.TreeFoliageVariantKey Key;

            public override void Prepare()
            {
                Asset<Texture2D> asset = tree.GetBranchTexture(Key.TextureIndex);
                asset ??= Macrocosm.EmptyTex;
                asset.Wait?.Invoke();
                PrepareTextureIfNecessary(asset.Value);
            }

            public override void PrepareShader()
            {
                PrepareShader(Key.PaintColor, tree.TreePaintingSettings);
            }
        }

        private readonly Dictionary<TilePaintSystemV2.TreeFoliageVariantKey, CustomTreeBranchTargetHolder> treeBranchRenders = new();

        private readonly Dictionary<TilePaintSystemV2.TreeFoliageVariantKey, CustomTreeTopRenderTargetHolder> treeTopRenders = new();

        public Texture2D TryGetTreeTopAndRequestIfNotReady(int x, int y, int paintColor)
        {
            int variant = GetVariant(x, y);
            return GetTopsTexture(variant).Value;

            TilePaintSystemV2.TreeFoliageVariantKey treeFoliageVariantKey = new()
            {
                TextureStyle = variant,
                PaintColor = paintColor,
                TextureIndex = Type // We use the tiletype as the index instead of the actual tree index since we can't insert our own index
            };

            TilePaintSystemV2.TreeFoliageVariantKey lookupKey = treeFoliageVariantKey;

            if (treeTopRenders.TryGetValue(lookupKey, out var value) && value.IsReady)
                return (Texture2D)(object)value.Target;

            tilePaintSystemV2_requests_fieldInfo ??= typeof(TilePaintSystemV2).GetField("_requests", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
            var requests = (List<TilePaintSystemV2.ARenderTargetHolder>)tilePaintSystemV2_requests_fieldInfo.GetValue(Main.instance.TilePaintSystem);
            value = new CustomTreeTopRenderTargetHolder(this) { Key = lookupKey };
            treeTopRenders.Add(lookupKey, value);

            if (!value.IsReady)
                requests.Add(value);

            return GetTopsTexture(variant).Value;
        }

        public Texture2D TryGetTreeBranchAndRequestIfNotReady(int x, int y, int paintColor)
        {
            int variant = GetVariant(x, y);
            return GetBranchTexture(variant).Value;

            TilePaintSystemV2.TreeFoliageVariantKey treeFoliageVariantKey = new()
            {
                TextureStyle = variant,
                PaintColor = paintColor,
                TextureIndex = Type // We use the tiletype as the index instead of the actual tree index since we can't insert our own index
            };

            TilePaintSystemV2.TreeFoliageVariantKey lookupKey = treeFoliageVariantKey;

            if (treeBranchRenders.TryGetValue(lookupKey, out var value) && value.IsReady)
                return (Texture2D)(object)value.Target;

            tilePaintSystemV2_requests_fieldInfo ??= typeof(TilePaintSystemV2).GetField("_requests", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
            var requests = (List<TilePaintSystemV2.ARenderTargetHolder>)tilePaintSystemV2_requests_fieldInfo.GetValue(Main.instance.TilePaintSystem);
            value = new CustomTreeBranchTargetHolder(this) { Key = lookupKey };
            treeBranchRenders.Add(lookupKey, value);

            if (!value.IsReady)
                requests.Add(value);

            return GetBranchTexture(variant).Value;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);

            Draw(i, j, spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(); // No params as PostDraw doesn't use spritebatch with params
        }

        protected virtual void Draw(int x, int y, SpriteBatch spriteBatch)
        {
            tileDrawing_treeWindCounter_fieldInfo ??= typeof(TileDrawing).GetField("_treeWindCounter", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);

            double treeWindCounter = (double)tileDrawing_treeWindCounter_fieldInfo.GetValue(Main.instance.TilesRenderer);
            Vector2 unscaledPosition = Main.Camera.UnscaledPosition;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            float topsFactor = 0.08f;
            float branchFactor = 0.06f;

            Tile tile = Main.tile[x, y];
            if (!tile.HasTile)
                return;

            short frameX = tile.TileFrameX;
            short frameY = tile.TileFrameY;
            bool wall = tile.WallType > 0;
            if (frameY >= 198 && frameX >= 22)
            {
                int treeFrame = WorldGen.GetTreeFrame(tile);
                switch (frameX)
                {
                    case 22:
                        {
                            int tileX = 0;
                            int grassPosX = x + tileX;
                            if (!GetTreeFoliageData(x, y, tileX, ref treeFrame, out int floorY, out int topTextureFrameWidth, out int topTextureFrameHeight))
                                return;

                            EmitLeaves(x, y, grassPosX, floorY);

                            byte tileColor = tile.TileColor;
                            Texture2D treeTopTexture = TryGetTreeTopAndRequestIfNotReady(x, y, tileColor);

                            Vector2 position = new Vector2(x * 16 - (int)unscaledPosition.X + 8, y * 16 - (int)unscaledPosition.Y + 16) + zero;
                            float windCycle = 0f;

                            if (!wall)
                                windCycle = Main.instance.TilesRenderer.GetWindCycle(x, y, treeWindCounter);

                            position.X += windCycle * 2f;
                            position.Y += Math.Abs(windCycle) * 2f;

                            Color color6 = Lighting.GetColor(x, y);

                            if (tile.IsTileFullbright)
                                color6 = Color.White;

                            Rectangle frame = new(treeFrame * (topTextureFrameWidth + 2), 0, topTextureFrameWidth, topTextureFrameHeight);
                            spriteBatch.Draw(treeTopTexture, position, frame, color6, windCycle * topsFactor, new Vector2(topTextureFrameWidth / 2, topTextureFrameHeight), 1f, 0, 0f);
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
                            Texture2D treeBranchTexture2 = TryGetTreeBranchAndRequestIfNotReady(x, y, tileColor);

                            Vector2 position = new Vector2(x * 16, y * 16) - unscaledPosition.Floor() + zero + new Vector2(16f, 12f);
                            float windCycle = 0f;

                            if (!wall)
                                windCycle = Main.instance.TilesRenderer.GetWindCycle(x, y, treeWindCounter);

                            if (windCycle > 0f)
                                position.X += windCycle;

                            position.X += Math.Abs(windCycle) * 2f;
                            Color color = Lighting.GetColor(x, y);

                            if (tile.IsTileFullbright)
                                color = Color.White;

                            spriteBatch.Draw(treeBranchTexture2, position, (Rectangle?)new Rectangle(0, treeFrame * 42, 40, 40), color, windCycle * branchFactor, new Vector2(40f, 24f), 1f, 0, 0f);
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
                            Texture2D treeBranchTexture = TryGetTreeBranchAndRequestIfNotReady(x, y, tileColor);

                            Vector2 position = new Vector2(x * 16, y * 16) - unscaledPosition.Floor() + zero + new Vector2(0f, 18f);
                            float windCycle = 0f;

                            if (!wall)
                                windCycle = Main.instance.TilesRenderer.GetWindCycle(x, y, treeWindCounter);

                            if (windCycle < 0f)
                                position.X += windCycle;

                            position.X -= Math.Abs(windCycle) * 2f;
                            Color color = Lighting.GetColor(x, y);

                            if (tile.IsTileFullbright)
                                color = Color.White;

                            spriteBatch.Draw(treeBranchTexture, position, (Rectangle?)new Rectangle(42, treeFrame * 42, 40, 40), color, windCycle * branchFactor, new Vector2(0f, 30f), 1f, 0, 0f);
                            break;
                        }
                }
            }
        }

        private bool GetTreeFoliageData(int i, int j, int xOffset, ref int treeFrame, out int floorY, out int topTextureFrameWidth, out int topTextureFrameHeight)
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

            tileDrawing_isActiveAndNotPaused_fieldInfo ??= typeof(TileDrawing).GetField("_isActiveAndNotPaused", BindingFlags.NonPublic | BindingFlags.Instance);
            tileDrawing_leafFrequency_fieldInfo ??= typeof(TileDrawing).GetField("_leafFrequency", BindingFlags.NonPublic | BindingFlags.Instance);
            tileDrawing_rand_fieldInfo ??= typeof(TileDrawing).GetField("_rand", BindingFlags.NonPublic | BindingFlags.Instance);

            bool isActiveAndNotPaused = (bool)tileDrawing_isActiveAndNotPaused_fieldInfo.GetValue(Main.instance.TilesRenderer);
            int leafFrequency = (int)tileDrawing_leafFrequency_fieldInfo.GetValue(Main.instance.TilesRenderer);
            UnifiedRandom rand = (UnifiedRandom)tileDrawing_rand_fieldInfo.GetValue(Main.instance.TilesRenderer);

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
