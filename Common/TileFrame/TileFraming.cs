using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace Macrocosm.Common.TileFrame
{
    public class TileFraming
    {
        // Framing legend:
        // ---------------
        //  The tile   | T	
        //  Solid      | #	
        //  Empty      | _
        //  Don't care |  
        // ---------------
        /// <summary> 
        /// Style currently used for plating blocks with special frames (e.g. Industrial Plating) 
        /// Adapted from the Gemspark blend style 
        /// </summary>
        public static void PlatingStyle(int i, int j, bool resetFrame = false, bool countHalfBlocks = false, int? customVariation = null)
        {
            Tile tile = Main.tile[i, j];
            //var info = new TileNeighbourInfo(i, j).TypedSolid(tile.TileType);
            CommonFraming(i, j, resetFrame, countHalfBlocks, customVariation);

            // TODO: integrate this slope check logic to a TileNeightbourInfo delegate (if possible)

            Tile tileTop = Main.tile[i, j - 1];
            Tile tileBottom = Main.tile[i, j + 1];
            Tile tileLeft = Main.tile[i - 1, j];
            Tile tileRight = Main.tile[i + 1, j];
            Tile tileTopLeft = Main.tile[i - 1, j - 1];
            Tile tileBottomLeft = Main.tile[i - 1, j + 1];
            Tile tileTopRight = Main.tile[i + 1, j - 1];
            Tile tileBottomRight = Main.tile[i + 1, j + 1];

            bool top = tileTop.HasTile && tileTop.TileType == tile.TileType;
            if (tileTop.IsSloped() && !tileTop.TopSlope)
                top = false;

            bool bottom = tileBottom.HasTile && tileBottom.TileType == tile.TileType && (countHalfBlocks || !tileBottom.IsHalfBlock);
            if (tileBottom.IsSloped() && !tileBottom.BottomSlope)
                bottom = false;

            bool left = tileLeft.HasTile && tileLeft.TileType == tile.TileType && (countHalfBlocks || !tileLeft.IsHalfBlock);
            if (tileLeft.IsSloped() && !tileLeft.LeftSlope)
                left = false;

            bool right = tileRight.HasTile && tileRight.TileType == tile.TileType && (countHalfBlocks || !tileRight.IsHalfBlock);
            if (tileRight.IsSloped() && !tileRight.RightSlope)
                right = false;

            bool topLeft = tileTopLeft.HasTile && tileTopLeft.TileType == tile.TileType && (countHalfBlocks || !tileTopLeft.IsHalfBlock);
            if (tileTopLeft.IsSloped() && (tileTopLeft.BottomSlope || tileTopLeft.RightSlope))
                topLeft = false;

            bool topRight = tileTopRight.HasTile && tileTopRight.TileType == tile.TileType && (countHalfBlocks || !tileTopRight.IsHalfBlock);
            if (tileTopRight.IsSloped() && (tileTopRight.BottomSlope || tileTopRight.LeftSlope))
                topRight = false;

            bool bottomLeft = tileBottomLeft.HasTile && tileBottomLeft.TileType == tile.TileType && (countHalfBlocks || !tileBottomLeft.IsHalfBlock);
            if (tileBottomLeft.IsSloped() && (tileBottomLeft.TopSlope || tileBottomLeft.RightSlope))
                bottomLeft = false;

            bool bottomRight = tileBottomRight.HasTile && tileBottomRight.TileType == tile.TileType && (countHalfBlocks || !tileBottomRight.IsHalfBlock);
            if (tileBottomRight.IsSloped() && (tileBottomRight.TopSlope || tileBottomRight.LeftSlope))
                bottomRight = false;

            if (tile.Slope is SlopeType.SlopeDownLeft)
            {
                top = false;
                right = false;
                topRight = false;
            }

            if (tile.Slope is SlopeType.SlopeDownRight)
            {
                top = false;
                left = false;
                topLeft = false;
            }

            if (tile.Slope is SlopeType.SlopeUpLeft)
            {
                bottom = false;
                right = false;
                bottomRight = false;
            }

            if (tile.Slope is SlopeType.SlopeUpRight)
            {
                bottom = false;
                left = false;
                bottomLeft = false;
            }

            if (tile.IsHalfBlock)
            {
                top = false;
                topLeft = false;
                topRight = false;

                right = false;
                left = false;
            }

            int count = new[] { top, topRight, topLeft, bottom, bottomRight, bottomLeft, right, left }.Count(b => b);

            //   _  
            // _ T #
            //   # _
            if (right && bottom && !top && !left && !bottomRight)
                tile.SetFrame(234, 0);

            // _ #  
            // _ T #
            //   _  
            if (top && right && !bottom && !left && !topRight)
                tile.SetFrame(234, 36);

            //   _  
            // # T _
            // _ #  
            if (left && bottom && !top && !right && !bottomLeft)
                tile.SetFrame(270, 0);

            // _ #  
            // # T _
            //   _  
            if (top && left && !bottom && !right && !topLeft)
                tile.SetFrame(270, 36);


            //   # _
            // _ T #
            //   # _
            if (top && bottom & right && !left && !topRight && !bottomRight)
                tile.SetFrame(234, 18);

            // _ #  
            // # T _
            // _ #  
            if (top && bottom & left && !right && !topLeft && !bottomLeft)
                tile.SetFrame(270, 18);

            // _ # _
            // # T #
            //   _  
            if (left && right && top && !bottom && !topLeft && !topRight)
                tile.SetFrame(252, 36);

            //   _  
            // # T #
            // _ # _
            if (left && right && bottom && !top && !bottomRight && !bottomLeft)
                tile.SetFrame(252, 0);

            //   # _
            // _ T #
            //   # #
            if (bottom && bottomRight && right && !topRight && top && !left)
                tile.SetFrame(288, 0);

            //   # #
            // _ T #
            //   # _
            if (top && topRight && right && !bottomRight && bottom && !left)
                tile.SetFrame(288, 18);

            // _ #  
            // # T _
            // # #  
            if (bottom && bottomLeft && left && !topLeft && top && !right)
                tile.SetFrame(306, 0);

            // # #  
            // # T _
            // _ #  
            if (top && topLeft && left && !bottomLeft && bottom && !right)
                tile.SetFrame(306, 18);

            //   _   
            // # T # 
            // _ # # 
            if (right && bottomRight && bottom && !bottomLeft && left && !top)
                tile.SetFrame(288, 36);

            //   _   
            // # T # 
            // # # _ 
            if (left && bottomLeft && bottom && !bottomRight && right && !top)
                tile.SetFrame(306, 36);

            // _ # # 
            // # T #
            //   _   
            if (right && topRight && top && !topLeft && left && !bottom)
                tile.SetFrame(288, 54);

            // # # _ 
            // # T # 
            // _ _   
            if (left && topLeft && top && !topRight && right && !bottom)
                tile.SetFrame(306, 54);

            // Neighbour count dependent frames
            switch (count)
            {
                case 4:
                    {
                        // _ # _
                        // # T #
                        // _ # _
                        if (top && right && bottom && left)
                            tile.SetFrame(252, 18);

                        break;
                    }

                case 5:
                    {
                        // _ # _
                        // # T #
                        // _ # #
                        if (!topLeft && !topRight && !bottomLeft)
                            tile.SetFrame(216, 54);

                        // _ # _
                        // # T #
                        // # # _
                        if (!topLeft && !topRight && !bottomRight)
                            tile.SetFrame(234, 54);

                        // _ # #
                        // # T #
                        // _ # _
                        if (!bottomLeft && !bottomRight && !topLeft)
                            tile.SetFrame(216, 72);

                        // # # _
                        // # T #
                        // _ # _
                        if (!bottomLeft && !bottomRight && !topRight)
                            tile.SetFrame(234, 72);

                        break;
                    }

                case 6:
                    {
                        // _ # #
                        // # T #
                        // # # _
                        if (!topLeft && !bottomRight)
                            tile.SetFrame(306, 72);

                        // # # _
                        // # T #
                        // _ # #
                        if (!topRight && !bottomLeft)
                            tile.SetFrame(288, 72);

                        break;
                    }

                case 7:
                    {
                        // # # _
                        // # T #
                        // # # #
                        if (!topRight)
                            tile.SetFrame(270, 54);

                        // _ # #
                        // # T #
                        // # # #
                        if (!topLeft)
                            tile.SetFrame(252, 54);

                        // # # #
                        // # T #
                        // # # _
                        if (!bottomRight)
                            tile.SetFrame(270, 72);

                        // # # #
                        // # T #
                        // _ # #
                        if (!bottomLeft)
                            tile.SetFrame(252, 72);

                        break;
                    }
            }
        }

        public static void OutlineSlopeFraming(int i, int j, bool resetFrame = false, int? customVariation = null)
        {
            Tile tile = Main.tile[i, j];
            int type = tile.TileType;
            SlopeType slope = tile.Slope;

            // Neighbour references 
            Tile tileTop = Main.tile[i, j - 1];
            Tile tileRight = Main.tile[i + 1, j];
            Tile tileBottom = Main.tile[i, j + 1];
            Tile tileLeft = Main.tile[i - 1, j];

            int top = -1, right = -1, bottom = -1, left = -1;
            if (tileTop.HasTile && !tileTop.IsHalfBlock)
            {
                if (tileTop.Slope == SlopeType.Solid ||
                    (slope == SlopeType.SlopeUpLeft && tileTop.Slope == SlopeType.SlopeDownLeft) ||
                    (slope == SlopeType.SlopeUpRight && tileTop.Slope == SlopeType.SlopeDownRight))
                {
                    top = tileTop.TileType;
                }
            }

            if (tileRight.HasTile && !tileRight.IsHalfBlock)
            {
                if (tileRight.Slope == SlopeType.Solid ||
                    (slope == SlopeType.SlopeUpRight && tileRight.Slope == SlopeType.SlopeUpLeft) ||
                    (slope == SlopeType.SlopeDownRight && tileRight.Slope == SlopeType.SlopeDownLeft))
                {
                    right = tileRight.TileType;
                }
            }

            if (tileBottom.HasTile && !tileBottom.IsHalfBlock)
            {
                if (tileBottom.Slope == SlopeType.Solid ||
                    (slope == SlopeType.SlopeDownLeft && tileBottom.Slope == SlopeType.SlopeUpLeft) ||
                    (slope == SlopeType.SlopeDownRight && tileBottom.Slope == SlopeType.SlopeUpRight))
                {
                    bottom = tileBottom.TileType;
                }
            }

            if (tileLeft.HasTile && !tileLeft.IsHalfBlock)
            {
                if (tileLeft.Slope == SlopeType.Solid ||
                    (slope == SlopeType.SlopeUpLeft && tileLeft.Slope == SlopeType.SlopeUpRight) ||
                    (slope == SlopeType.SlopeDownLeft && tileLeft.Slope == SlopeType.SlopeDownRight))
                {
                    left = tileLeft.TileType;
                }
            }

            int variation;
            if (customVariation.HasValue)
            {
                variation = customVariation.Value;
            }
            else if (!resetFrame)
            {
                variation = tile.TileFrameNumber;
            }
            else
            {
                variation = WorldGen.genRand.Next(3);
                tile.TileFrameNumber = variation;
            }

            Point frame = new(-1, -1);
            switch (slope)
            {
                case SlopeType.SlopeDownRight:
                    if(right == type && bottom == type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 234;
                                frame.Y = 0;
                                break;

                            case 1:
                                frame.X = 270;
                                frame.Y = 0;
                                break;

                            case 2:
                                frame.X = 306;
                                frame.Y = 0;
                                break;
                        }
                    }
                    else if (right == type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 342;
                                frame.Y = 0;
                                break;

                            case 1:
                                frame.X = 378;
                                frame.Y = 0;
                                break;

                            case 2:
                                frame.X = 414;
                                frame.Y = 0;
                                break;
                        }
                    }
                    else if(bottom == type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 234;
                                frame.Y = 36;
                                break;

                            case 1:
                                frame.X = 270;
                                frame.Y = 36;
                                break;

                            case 2:
                                frame.X = 306;
                                frame.Y = 36;
                                break;
                        }
                    }
                    else
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 342;
                                frame.Y = 36;
                                break;

                            case 1:
                                frame.X = 378;
                                frame.Y = 36;
                                break;

                            case 2:
                                frame.X = 414;
                                frame.Y = 36;
                                break;
                        }
                    }
                    break;

                case SlopeType.SlopeDownLeft:
                    if (left == type && bottom == type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 252;
                                frame.Y = 0;
                                break;

                            case 1:
                                frame.X = 288;
                                frame.Y = 0;
                                break;

                            case 2:
                                frame.X = 324;
                                frame.Y = 0;
                                break;
                        }
                    }
                    else if (left == type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 360;
                                frame.Y = 0;
                                break;

                            case 1:
                                frame.X = 396;
                                frame.Y = 0;
                                break;

                            case 2:
                                frame.X = 432;
                                frame.Y = 0;
                                break;
                        }
                    }
                    else if (bottom == type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 252;
                                frame.Y = 36;
                                break;

                            case 1:
                                frame.X = 288;
                                frame.Y = 36;
                                break;

                            case 2:
                                frame.X = 324;
                                frame.Y = 36;
                                break;
                        }
                    }
                    else
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 360;
                                frame.Y = 36;
                                break;

                            case 1:
                                frame.X = 396;
                                frame.Y = 36;
                                break;

                            case 2:
                                frame.X = 432;
                                frame.Y = 36;
                                break;
                        }
                    }
                    break;

                case SlopeType.SlopeUpRight:
                    if (right == type && top == type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 234;
                                frame.Y = 18;
                                break;

                            case 1:
                                frame.X = 270;
                                frame.Y = 18;
                                break;

                            case 2:
                                frame.X = 306;
                                frame.Y = 18;
                                break;
                        }
                    }
                    else if (right == type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 342;
                                frame.Y = 18;
                                break;

                            case 1:
                                frame.X = 378;
                                frame.Y = 18;
                                break;

                            case 2:
                                frame.X = 414;
                                frame.Y = 18;
                                break;
                        }
                    }
                    else if (top == type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 234;
                                frame.Y = 54;
                                break;

                            case 1:
                                frame.X = 270;
                                frame.Y = 54;
                                break;

                            case 2:
                                frame.X = 306;
                                frame.Y = 54;
                                break;
                        }
                    }
                    else
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 342;
                                frame.Y = 54;
                                break;

                            case 1:
                                frame.X = 378;
                                frame.Y = 54;
                                break;

                            case 2:
                                frame.X = 414;
                                frame.Y = 54;
                                break;
                        }
                    }
                    break;

                case SlopeType.SlopeUpLeft:
                    if (left == type && top == type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 252;
                                frame.Y = 18;
                                break;

                            case 1:
                                frame.X = 288;
                                frame.Y = 18;
                                break;

                            case 2:
                                frame.X = 324;
                                frame.Y = 18;
                                break;
                        }
                    }
                    else if (left == type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 360;
                                frame.Y = 18;
                                break;

                            case 1:
                                frame.X = 396;
                                frame.Y = 18;
                                break;

                            case 2:
                                frame.X = 432;
                                frame.Y = 18;
                                break;
                        }
                    }
                    else if(top == type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 252;
                                frame.Y = 54;
                                break;

                            case 1:
                                frame.X = 288;
                                frame.Y = 54;
                                break;

                            case 2:
                                frame.X = 324;
                                frame.Y = 54;
                                break;
                        }
                    }
                    else
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 360;
                                frame.Y = 54;
                                break;

                            case 1:
                                frame.X = 396;
                                frame.Y = 54;
                                break;

                            case 2:
                                frame.X = 432;
                                frame.Y = 54;
                                break;
                        }
                    }
                    break;
            }

            if(frame.X >= 0 && frame.Y >= 0)
            {
                tile.TileFrameX = (short)frame.X;
                tile.TileFrameY = (short)frame.Y;
            }
        }

        /// <summary>
        /// Copy of the common framing all blocks in Terraria use, with some extra settings
        /// </summary>
        /// <param name="i"> Tile X </param>
        /// <param name="j"> Tile Y </param>
        /// <param name="countHalfBlocks"> Count half blocks for merging </param>
        /// <param name="customVariation"> Optional user defined variation, should be in the 0-2 range, randomized by default </param>
        public static void CommonFraming(int i, int j, bool resetFrame = false, bool countHalfBlocks = false, int? customVariation = null)
        {
            Tile tile = Main.tile[i, j];
            int type = tile.TileType;

            // Neighbour references 
            Tile tileTop = Main.tile[i, j - 1];
            Tile tileTopRight = Main.tile[i + 1, j - 1];
            Tile tileRight = Main.tile[i + 1, j];
            Tile tileBottomRight = Main.tile[i + 1, j + 1];
            Tile tileBottom = Main.tile[i, j + 1];
            Tile tileBottomLeft = Main.tile[i - 1, j + 1];
            Tile tileLeft = Main.tile[i - 1, j];
            Tile tileTopLeft = Main.tile[i - 1, j - 1];

            int top = tileTop.HasTile ? tileTop.TileType : -1;
            int topRight = tileTopRight.HasTile ? tileTopRight.TileType : -1;
            int right = tileRight.HasTile ? tileRight.TileType : -1;
            int bottomRight = tileBottomRight.HasTile ? tileBottomRight.TileType : -1;
            int bottom = tileBottom.HasTile ? tileBottom.TileType : -1;
            int bottomLeft = tileBottomLeft.HasTile ? tileBottomLeft.TileType : -1;
            int left = tileLeft.HasTile ? tileLeft.TileType : -1;
            int topLeft = tileTopLeft.HasTile ? tileTopLeft.TileType : -1;

            // Ignore unconnected neighbour slopes
            if (tileTop.BottomSlope)
                top = -1;

            if (tileRight.LeftSlope)
                right = -1;

            if (tileBottom.TopSlope)
                bottom = -1;

            if (tileLeft.RightSlope)
                left = -1;

            if (tileTopRight.LeftSlope)
                topRight = -1;

            if (tileTopLeft.RightSlope)
                topLeft = -1;

            if (tileBottomLeft.RightSlope || tileBottomLeft.Slope is SlopeType.SlopeDownRight || tileBottomLeft.IsHalfBlock)
                bottomLeft = -1;

            if (tileBottomLeft.LeftSlope || tileBottomLeft.Slope is SlopeType.SlopeDownLeft || tileBottomLeft.IsHalfBlock)
                bottomRight = -1;

            // If sloped, don't blend with unconnected neighbours
            if (tile.Slope is SlopeType.SlopeDownLeft)
            {
                top = -1;
                right = -1;
                topRight = -1;
            }

            if (tile.Slope is SlopeType.SlopeDownRight)
            {
                top = -1;
                left = -1;
                topLeft = -1;
            }

            if (tile.Slope is SlopeType.SlopeUpLeft)
            {
                bottom = -1;
                right = -1;
                bottomRight = -1;
            }

            if (tile.Slope is SlopeType.SlopeUpRight)
            {
                bottom = -1;
                left = -1;
                bottomLeft = -1;
            }

            // Check halfblock neighbours for connection

            if (left > -1 && tileLeft.IsHalfBlock)
            {
                if (tile.IsHalfBlock)
                    left = type;
                else if (tileLeft.TileType != type || !countHalfBlocks)
                    left = -1;
            }

            if (right > -1 && tileRight.IsHalfBlock)
            {
                if (tile.IsHalfBlock)
                    right = type;
                else if (tileRight.TileType != type || !countHalfBlocks)
                    right = -1;
            }

            if (bottomLeft > -1 && tileBottomLeft.IsHalfBlock)
            {
                if (tile.IsHalfBlock)
                    bottomLeft = type;
                else if (tileBottomLeft.TileType != type || !countHalfBlocks)
                    bottomLeft = -1;
            }

            if (bottomRight > -1 && tileBottomRight.IsHalfBlock)
            {
                if (tile.IsHalfBlock)
                    bottomRight = type;
                else if (tileBottomRight.TileType != type || !countHalfBlocks)
                    bottomRight = -1;
            }

            // If this is a halfblock, don't blend with unconnected halfblocks
            if (tile.IsHalfBlock)
            {
                if (left != type)
                    left = -1;

                if (right != type)
                    right = -1;

                top = -1;
            }

            // Ignore halfblocks below
            if (tileBottom.IsHalfBlock)
                bottom = -1;

            Point frame = new(-1, -1);

            int variation;
            if (customVariation.HasValue)
            {
                variation = customVariation.Value;
            }
            else if (!resetFrame)
            {
                variation = tile.TileFrameNumber;
            }
            else
            {
                variation = WorldGen.genRand.Next(3);
                tile.TileFrameNumber = variation;
            }

            if (frame.X < 0 || frame.Y < 0)
            {
                if (top == type && bottom == type && left == type && right == type)
                {
                    if (topLeft != type && topRight != type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 108;
                                frame.Y = 18;
                                break;
                            case 1:
                                frame.X = 126;
                                frame.Y = 18;
                                break;
                            default:
                                frame.X = 144;
                                frame.Y = 18;
                                break;
                        }
                    }
                    else if (bottomLeft != type && bottomRight != type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 108;
                                frame.Y = 36;
                                break;
                            case 1:
                                frame.X = 126;
                                frame.Y = 36;
                                break;
                            default:
                                frame.X = 144;
                                frame.Y = 36;
                                break;
                        }
                    }
                    else if (topLeft != type && bottomLeft != type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 180;
                                frame.Y = 0;
                                break;
                            case 1:
                                frame.X = 180;
                                frame.Y = 18;
                                break;
                            default:
                                frame.X = 180;
                                frame.Y = 36;
                                break;
                        }
                    }
                    else if (topRight != type && bottomRight != type)
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 198;
                                frame.Y = 0;
                                break;
                            case 1:
                                frame.X = 198;
                                frame.Y = 18;
                                break;
                            default:
                                frame.X = 198;
                                frame.Y = 36;
                                break;
                        }
                    }
                    else
                    {
                        switch (variation)
                        {
                            case 0:
                                frame.X = 18;
                                frame.Y = 18;
                                break;
                            case 1:
                                frame.X = 36;
                                frame.Y = 18;
                                break;
                            default:
                                frame.X = 54;
                                frame.Y = 18;
                                break;
                        }
                    }
                }
                else if (top != type && bottom == type && left == type && right == type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 18;
                            frame.Y = 0;
                            break;
                        case 1:
                            frame.X = 36;
                            frame.Y = 0;
                            break;
                        default:
                            frame.X = 54;
                            frame.Y = 0;
                            break;
                    }
                }
                else if (top == type && bottom != type && left == type && right == type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 18;
                            frame.Y = 36;
                            break;
                        case 1:
                            frame.X = 36;
                            frame.Y = 36;
                            break;
                        default:
                            frame.X = 54;
                            frame.Y = 36;
                            break;
                    }
                }
                else if (top == type && bottom == type && left != type && right == type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 0;
                            frame.Y = 0;
                            break;
                        case 1:
                            frame.X = 0;
                            frame.Y = 18;
                            break;
                        default:
                            frame.X = 0;
                            frame.Y = 36;
                            break;
                    }
                }
                else if (top == type && bottom == type && left == type && right != type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 72;
                            frame.Y = 0;
                            break;
                        case 1:
                            frame.X = 72;
                            frame.Y = 18;
                            break;
                        default:
                            frame.X = 72;
                            frame.Y = 36;
                            break;
                    }
                }
                else if (top != type && bottom == type && left != type && right == type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 0;
                            frame.Y = 54;
                            break;
                        case 1:
                            frame.X = 36;
                            frame.Y = 54;
                            break;
                        default:
                            frame.X = 72;
                            frame.Y = 54;
                            break;
                    }
                }
                else if (top != type && bottom == type && left == type && right != type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 18;
                            frame.Y = 54;
                            break;
                        case 1:
                            frame.X = 54;
                            frame.Y = 54;
                            break;
                        default:
                            frame.X = 90;
                            frame.Y = 54;
                            break;
                    }
                }
                else if (top == type && bottom != type && left != type && right == type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 0;
                            frame.Y = 72;
                            break;
                        case 1:
                            frame.X = 36;
                            frame.Y = 72;
                            break;
                        default:
                            frame.X = 72;
                            frame.Y = 72;
                            break;
                    }
                }
                else if (top == type && bottom != type && left == type && right != type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 18;
                            frame.Y = 72;
                            break;
                        case 1:
                            frame.X = 54;
                            frame.Y = 72;
                            break;
                        default:
                            frame.X = 90;
                            frame.Y = 72;
                            break;
                    }
                }
                else if (top == type && bottom == type && left != type && right != type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 90;
                            frame.Y = 0;
                            break;
                        case 1:
                            frame.X = 90;
                            frame.Y = 18;
                            break;
                        default:
                            frame.X = 90;
                            frame.Y = 36;
                            break;
                    }
                }
                else if (top != type && bottom != type && left == type && right == type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 108;
                            frame.Y = 72;
                            break;
                        case 1:
                            frame.X = 126;
                            frame.Y = 72;
                            break;
                        default:
                            frame.X = 144;
                            frame.Y = 72;
                            break;
                    }
                }
                else if (top != type && bottom == type && left != type && right != type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 108;
                            frame.Y = 0;
                            break;
                        case 1:
                            frame.X = 126;
                            frame.Y = 0;
                            break;
                        default:
                            frame.X = 144;
                            frame.Y = 0;
                            break;
                    }
                }
                else if (top == type && bottom != type && left != type && right != type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 108;
                            frame.Y = 54;
                            break;
                        case 1:
                            frame.X = 126;
                            frame.Y = 54;
                            break;
                        default:
                            frame.X = 144;
                            frame.Y = 54;
                            break;
                    }
                }
                else if (top != type && bottom != type && left != type && right == type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 162;
                            frame.Y = 0;
                            break;
                        case 1:
                            frame.X = 162;
                            frame.Y = 18;
                            break;
                        default:
                            frame.X = 162;
                            frame.Y = 36;
                            break;
                    }
                }
                else if (top != type && bottom != type && left == type && right != type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 216;
                            frame.Y = 0;
                            break;
                        case 1:
                            frame.X = 216;
                            frame.Y = 18;
                            break;
                        default:
                            frame.X = 216;
                            frame.Y = 36;
                            break;
                    }
                }
                else if (top != type && bottom != type && left != type && right != type)
                {
                    switch (variation)
                    {
                        case 0:
                            frame.X = 162;
                            frame.Y = 54;
                            break;
                        case 1:
                            frame.X = 180;
                            frame.Y = 54;
                            break;
                        default:
                            frame.X = 198;
                            frame.Y = 54;
                            break;
                    }
                }
            }

            if (frame.X <= -1 || frame.Y <= -1)
            {
                switch (variation)
                {
                    case 0:
                        frame.X = 18;
                        frame.Y = 18;
                        break;
                    case 1:
                        frame.X = 36;
                        frame.Y = 18;
                        break;
                    default:
                        frame.X = 54;
                        frame.Y = 18;
                        break;
                }
            }

            Main.tile[i, j].TileFrameX = (short)frame.X;
            Main.tile[i, j].TileFrameY = (short)frame.Y;
        }
    }
}
