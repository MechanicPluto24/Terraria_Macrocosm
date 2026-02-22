using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace Macrocosm.Common.TileFrame;

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
    /// Style currently used for blocks with special corner-aware frames (e.g. Gemspark, Conveyor Belt).
    /// <br/> Uses vanilla's <see cref="Framing.SelfFrame8Way"/>, which already handles the equivalent of <see cref="CommonFraming"/>
    /// <br/> Make sure to still set <see cref="TileID.Sets.GemsparkFramingTypes"/> for the tile type to its type.
    /// </summary>
    public static void GemsparkFraming(int i, int j, bool resetFrame = false, int? customVariation = null)
    {
        Tile tile = Main.tile[i, j];

        if (customVariation.HasValue)
        {
            tile.TileFrameNumber = customVariation.Value;
            resetFrame = false;
        }

        Framing.SelfFrame8Way(i, j, tile, resetFrame);
    }

    /// <summary>
    /// Applies special slope frames adapted from vanilla Terraria's HasSlopeFrames behavior (e.g. Conveyor Belt)
    /// <br/> Make sure to still set <see cref="TileID.Sets.HasSlopeFrames"/> for the tile type to true.
    /// </summary>
    public static void SlopeFraming(int i, int j)
    {
        Tile tile = Main.tile[i, j];

        if (tile.IsHalfBlock)
            return;

        int type = tile.TileType;
        SlopeType slope = tile.Slope;

        Tile tileTop = Main.tile[i, j - 1];
        Tile tileRight = Main.tile[i + 1, j];
        Tile tileBottom = Main.tile[i, j + 1];
        Tile tileLeft = Main.tile[i - 1, j];
        Tile tileTopLeft = Main.tile[i - 1, j - 1];
        Tile tileTopRight = Main.tile[i + 1, j - 1];
        Tile tileBottomLeft = Main.tile[i - 1, j + 1];
        Tile tileBottomRight = Main.tile[i + 1, j + 1];

        int top = tileTop.HasTile ? tileTop.TileType : -1;
        int right = tileRight.HasTile ? tileRight.TileType : -1;
        int bottom = tileBottom.HasTile ? tileBottom.TileType : -1;
        int left = tileLeft.HasTile ? tileLeft.TileType : -1;
        int topLeft = tileTopLeft.HasTile ? tileTopLeft.TileType : -1;
        int topRight = tileTopRight.HasTile ? tileTopRight.TileType : -1;
        int bottomLeft = tileBottomLeft.HasTile ? tileBottomLeft.TileType : -1;
        int bottomRight = tileBottomRight.HasTile ? tileBottomRight.TileType : -1;

        if (slope == SlopeType.Solid)
        {
            // Check which cardinal neighbors are same-type AND sloped toward this tile
            bool topSloped = top == type && tileTop.TopSlope;
            bool leftSloped = left == type && tileLeft.LeftSlope;
            bool rightSloped = right == type && tileRight.RightSlope;
            bool bottomSloped = bottom == type && tileBottom.BottomSlope;

            int slopedCount = (topSloped ? 1 : 0) + (leftSloped ? 1 : 0) + (rightSloped ? 1 : 0) + (bottomSloped ? 1 : 0);

            int offsetX = 0;
            int offsetY = 0;

            if (slopedCount > 2)
            {
                int backslashCount =
                    (tileTop.HasTile && tileTop.Slope == SlopeType.SlopeDownLeft ? 1 : 0) +
                    (tileRight.HasTile && tileRight.Slope == SlopeType.SlopeDownLeft ? 1 : 0) +
                    (tileBottom.HasTile && tileBottom.Slope == SlopeType.SlopeUpRight ? 1 : 0) +
                    (tileLeft.HasTile && tileLeft.Slope == SlopeType.SlopeUpRight ? 1 : 0);

                int slashCount =
                    (tileTop.HasTile && tileTop.Slope == SlopeType.SlopeDownRight ? 1 : 0) +
                    (tileRight.HasTile && tileRight.Slope == SlopeType.SlopeUpLeft ? 1 : 0) +
                    (tileBottom.HasTile && tileBottom.Slope == SlopeType.SlopeUpLeft ? 1 : 0) +
                    (tileLeft.HasTile && tileLeft.Slope == SlopeType.SlopeDownRight ? 1 : 0);

                if (backslashCount == slashCount)
                {
                    offsetX = 2;
                    offsetY = 4;
                }
                else if (backslashCount > slashCount)
                {
                    bool topLeftSolid = topLeft == type && tileTopLeft.Slope == SlopeType.Solid;
                    bool bottomRightSolid = bottomRight == type && tileBottomRight.Slope == SlopeType.Solid;

                    if (topLeftSolid && bottomRightSolid)
                    {
                        offsetY = 4;
                    }
                    else if (bottomRightSolid)
                    {
                        offsetX = 6;
                    }
                    else
                    {
                        offsetX = 7;
                        offsetY = 1;
                    }
                }
                else
                {
                    bool topRightSolid = topRight == type && tileTopRight.Slope == SlopeType.Solid;
                    bool bottomLeftSolid = bottomLeft == type && tileBottomLeft.Slope == SlopeType.Solid;

                    if (topRightSolid && bottomLeftSolid)
                    {
                        offsetX = 1;
                        offsetY = 4;
                    }
                    else if (bottomLeftSolid)
                    {
                        offsetX = 7;
                    }
                    else
                    {
                        offsetX = 6;
                        offsetY = 1;
                    }
                }

                tile.TileFrameX = (short)((18 + offsetX) * 18);
                tile.TileFrameY = (short)(offsetY * 18);
            }
            else if (slopedCount == 2)
            {
                bool hasFrame = false;

                if (topSloped && leftSloped && bottom == type && right == type)
                {
                    offsetY = 2;
                    hasFrame = true;
                }
                else if (topSloped && rightSloped && bottom == type && left == type)
                {
                    offsetX = 1;
                    offsetY = 2;
                    hasFrame = true;
                }
                else if (rightSloped && bottomSloped && top == type && left == type)
                {
                    offsetX = 1;
                    offsetY = 3;
                    hasFrame = true;
                }
                else if (bottomSloped && leftSloped && top == type && right == type)
                {
                    offsetY = 3;
                    hasFrame = true;
                }

                if (hasFrame)
                {
                    tile.TileFrameX = (short)((18 + offsetX) * 18);
                    tile.TileFrameY = (short)(offsetY * 18);
                }
            }
        }
        else
        {
            int side = -1;
            int other = -1;
            int diagonal = -1;
            int baseOffsetX = 0;
            int baseOffsetY = 0;

            switch (slope)
            {
                case SlopeType.SlopeDownLeft:
                    side = left;
                    other = bottom;
                    diagonal = bottomLeft;
                    baseOffsetX = 1;
                    break;

                case SlopeType.SlopeDownRight:
                    side = right;
                    other = bottom;
                    diagonal = bottomRight;
                    break;

                case SlopeType.SlopeUpLeft:
                    side = left;
                    other = top;
                    diagonal = topLeft;
                    baseOffsetX = 1;
                    baseOffsetY = 1;
                    break;

                case SlopeType.SlopeUpRight:
                    side = right;
                    other = top;
                    diagonal = topRight;
                    baseOffsetY = 1;
                    break;
            }

            int offsetX = baseOffsetX;
            int offsetY = baseOffsetY;

            if (side != type || other != type || diagonal != type)
            {
                if (side == type && other == type)
                    offsetX += 2;
                else if (side == type)
                    offsetX += 4;
                else if (other == type)
                {
                    offsetX += 4;
                    offsetY += 2;
                }
                else
                {
                    offsetX += 2;
                    offsetY += 2;
                }
            }

            tile.TileFrameX = (short)((18 + offsetX) * 18);
            tile.TileFrameY = (short)(offsetY * 18);
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

        if (tileTop.HasTile) // half blocks allowed
        {
            if (tileTop.Slope == SlopeType.Solid ||
                ((slope == SlopeType.SlopeUpLeft || slope == SlopeType.SlopeUpRight) &&
                 (tileTop.Slope == SlopeType.SlopeDownLeft || tileTop.Slope == SlopeType.SlopeDownRight)))
            {
                top = tileTop.TileType;
            }
        }

        if (tileRight.HasTile && !tileRight.IsHalfBlock)
        {
            if (tileRight.Slope == SlopeType.Solid ||
                ((slope == SlopeType.SlopeUpRight || slope == SlopeType.SlopeDownRight) &&
                 (tileRight.Slope == SlopeType.SlopeDownLeft || tileRight.Slope == SlopeType.SlopeUpLeft)))
            {
                right = tileRight.TileType;
            }
        }

        if (tileBottom.HasTile && !tileBottom.IsHalfBlock)
        {
            if (tileBottom.Slope == SlopeType.Solid ||
                ((slope == SlopeType.SlopeDownLeft || slope == SlopeType.SlopeDownRight) &&
                 (tileBottom.Slope == SlopeType.SlopeUpLeft || tileBottom.Slope == SlopeType.SlopeUpRight)))
            {
                bottom = tileBottom.TileType;
            }
        }

        if (tileLeft.HasTile && !tileLeft.IsHalfBlock)
        {
            if (tileLeft.Slope == SlopeType.Solid ||
                ((slope == SlopeType.SlopeUpLeft || slope == SlopeType.SlopeDownLeft) &&
                 (tileLeft.Slope == SlopeType.SlopeDownRight || tileLeft.Slope == SlopeType.SlopeUpRight)))
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
                if (right == type && bottom == type)
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
                else if (bottom == type)
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
                else if (top == type)
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

        if (frame.X >= 0 && frame.Y >= 0)
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
