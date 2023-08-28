using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Base
{
    /*
	 * A class used by the newer gen methods to make calling them easier without needing to provide a ton of parameters multiple times.
	 * 
	 *  tiles: An array of tiles to use to generate this hallway.
	 *  orderTiles: If true, gets tile ids in order; otherwise gets them at random.
	 *  walls: An array of walls to use to generate the inside of this hallway. Default is none.
	 *  orderWalls: If true, gets wall ids in order; otherwise gets them at random.
	 *  slope: if true, smoothes the gen with slopes.
	 *  CanPlace(x, y, tileID, wallID): an optional Func that can be used to have custom behavior about placing tiles. 
	 *  CanPlaceWall(x, y, tileID, wallID): an optional Func that can be used to have custom behavior about placing walls. 
	 */
    internal class GenConditions
    {
        public int[] tiles;
        public int[] walls;
        public bool orderTiles = false;
        public bool orderWalls = false;
        public bool slope = false;
        public Func<int, int, int, int, bool> CanPlace = null;
        public Func<int, int, int, int, bool> CanPlaceWall = null;

        public int GetTile(int index)
        {
            return tiles == null || tiles.Length <= index ? -1 : tiles[index];
        }

        public int GetWall(int index)
        {
            return walls == null || walls.Length <= index ? -1 : walls[index];
        }
    }

    #region Custom GenShapes
    internal class ShapeChasmSideways : GenShape
    {
        public int startheight = 20, endheight = 5, length = 60, variance, randomHeading;
        public float[] heightVariance;
        public bool dir = true;

        public ShapeChasmSideways(int startheight, int endheight, int length, int variance, int randomHeading, float[] heightVariance = null, bool dir = true)
        {
            this.startheight = startheight;
            this.endheight = endheight;
            this.length = length;
            this.variance = variance;
            this.randomHeading = randomHeading;
            this.heightVariance = heightVariance;
            this.dir = dir;
        }

        public void ResetChasmParams(int startheight, int endheight, int length, int variance, int randomHeading, float[] heightVariance = null, bool dir = true)
        {
            this.startheight = startheight;
            this.endheight = endheight;
            this.length = length;
            this.variance = variance;
            this.randomHeading = randomHeading;
            this.heightVariance = heightVariance;
            this.dir = dir;
        }

        private bool DoChasm(Point origin, GenAction action, int startheight, int endheight, int length, int variance, int randomHeading, float[] heightVariance, bool dir)
        {
            Point trueOrigin = origin;
            for (int m = 0; m < length; m++)
            {
                int height = (int)MathHelper.Lerp(startheight, endheight, m / (float)length);
                if (heightVariance != null)
                {
                    height = Math.Max(endheight, (int)(startheight * Utils.MultiLerp(m / (float)length, heightVariance)));
                }
                int x = trueOrigin.X + (dir ? m : -m);
                int y = trueOrigin.Y + (startheight - height);
                if (variance != 0)
                {
                    y += Main.rand.NextBool(2) ? -Main.rand.Next(variance) : Main.rand.Next(variance);
                }
                if (randomHeading != 0)
                {
                    y += randomHeading * (m / 2);
                }
                int yend = y + height - (startheight - height);
                int difference = yend - y;
                for (int m2 = y; m2 < yend; m2++)
                {
                    int y2 = m2;
                    if (!UnitApply(action, trueOrigin, x, y2) && _quitOnFail)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool Perform(Point origin, GenAction action)
        {
            return DoChasm(origin, action, startheight, endheight, length, variance, randomHeading, heightVariance, dir);
        }
    }

    internal class ShapeChasm : GenShape
    {
        public int startwidth = 20, endwidth = 5, depth = 60, variance, randomHeading;
        public float[] widthVariance;
        public bool dir = true;

        public ShapeChasm(int startwidth, int endwidth, int depth, int variance, int randomHeading, float[] widthVariance = null, bool dir = true)
        {
            this.startwidth = startwidth;
            this.endwidth = endwidth;
            this.depth = depth;
            this.variance = variance;
            this.randomHeading = randomHeading;
            this.widthVariance = widthVariance;
            this.dir = dir;
        }

        public void ResetChasmParams(int startwidth, int endwidth, int depth, int variance, int randomHeading, float[] widthVariance = null, bool dir = true)
        {
            this.startwidth = startwidth;
            this.endwidth = endwidth;
            this.depth = depth;
            this.variance = variance;
            this.randomHeading = randomHeading;
            this.widthVariance = widthVariance;
            this.dir = dir;
        }

        private bool DoChasm(Point origin, GenAction action, int startwidth, int endwidth, int depth, int variance, int randomHeading, float[] widthVariance, bool dir)
        {
            Point trueOrigin = origin;
            for (int m = 0; m < depth; m++)
            {
                int width = (int)MathHelper.Lerp(startwidth, endwidth, m / (float)depth);
                if (widthVariance != null)
                {
                    width = Math.Max(endwidth, (int)(startwidth * Utils.MultiLerp(m / (float)depth, widthVariance)));
                }
                int x = trueOrigin.X + (startwidth - width);
                int y = trueOrigin.Y + (dir ? m : -m);
                if (variance != 0)
                {
                    x += Main.rand.NextBool(2) ? -Main.rand.Next(variance) : Main.rand.Next(variance);
                }
                if (randomHeading != 0)
                {
                    x += randomHeading * (m / 2);
                }
                int xend = x + width - (startwidth - width);
                for (int m2 = x; m2 < xend; m2++)
                {
                    int x2 = m2;
                    if (!UnitApply(action, trueOrigin, x2, y) && _quitOnFail)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool Perform(Point origin, GenAction action)
        {
            return DoChasm(origin, action, startwidth, endwidth, depth, variance, randomHeading, widthVariance, dir);
        }
    }

    #endregion

    #region Custom GenActions
    internal class IsInWorld : GenAction
    {
        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY)
                return Fail();
            return UnitApply(origin, x, y, args);
        }
    }

    internal class SetModTile : GenAction
    {
        public ushort type;
        public short frameX = -1;
        public short frameY = -1;
        public bool doFraming;
        public bool doNeighborFraming;
        public Func<int, int, Tile, bool> canReplace;

        public SetModTile(ushort type, bool setSelfFrames = false, bool setNeighborFrames = true)
        {
            this.type = type;
            doFraming = setSelfFrames;
            doNeighborFraming = setNeighborFrames;
        }

        public SetModTile ExtraParams(Func<int, int, Tile, bool> canReplace, int frameX = -1, int frameY = -1)
        {
            this.canReplace = canReplace;
            this.frameX = (short)frameX;
            this.frameY = (short)frameY;
            return this;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY)
                return false;
            if (canReplace == null || canReplace != null && canReplace(x, y, _tiles[x, y]))
            {
                _tiles[x, y].ResetToType(type);
                if (frameX > -1)
                    _tiles[x, y].TileFrameX = frameX;
                if (frameY > -1)
                    _tiles[x, y].TileFrameY = frameY;
                if (doFraming)
                {
                    WorldUtils.TileFrame(x, y, doNeighborFraming);
                }
            }
            return UnitApply(origin, x, y, args);
        }
    }

    internal class SetMapBrightness : GenAction
    {
        public byte brightness;

        public SetMapBrightness(byte brightness)
        {
            this.brightness = brightness;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY) return false;
            Main.Map.UpdateLighting(x, y, Math.Max(Main.Map[x, y].Light, brightness));
            return UnitApply(origin, x, y, args);
        }
    }


    internal class PlaceModWall : GenAction
    {
        public ushort type;
        public bool neighbors;
        public Func<int, int, Tile, bool> canReplace;

        public PlaceModWall(int type, bool neighbors = true)
        {
            this.type = (ushort)type;
            this.neighbors = neighbors;
        }

        public PlaceModWall ExtraParams(Func<int, int, Tile, bool> canReplace)
        {
            this.canReplace = canReplace;
            return this;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY) return false;
            if (canReplace == null || canReplace != null && canReplace(x, y, _tiles[x, y]))
            {
                _tiles[x, y].WallType = type;
                WorldGen.SquareWallFrame(x, y);
                if (neighbors)
                {
                    WorldGen.SquareWallFrame(x + 1, y);
                    WorldGen.SquareWallFrame(x - 1, y);
                    WorldGen.SquareWallFrame(x, y - 1);
                    WorldGen.SquareWallFrame(x, y + 1);
                }
            }
            return UnitApply(origin, x, y, args);
        }
    }

    internal class RadialDitherTopMiddle : GenAction
    {
        private int _width, _height;
        private float _innerRadius, _outerRadius;

        public RadialDitherTopMiddle(int width, int height, float innerRadius, float outerRadius)
        {
            _width = width;
            _height = height;
            _innerRadius = innerRadius;
            _outerRadius = outerRadius;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            Vector2 value = new((float)origin.X + _width / 2, origin.Y);
            Vector2 value2 = new(x, y);
            float num = Vector2.Distance(value2, value);
            float num2 = Math.Max(0f, Math.Min(1f, (num - _innerRadius) / (_outerRadius - _innerRadius)));
            if (_random.NextDouble() > num2)
            {
                return UnitApply(origin, x, y, args);
            }
            return Fail();
        }
    }

    internal class ClearTileSafely : GenAction
    {
        private bool _frameNeighbors;

        public ClearTileSafely(bool frameNeighbors = false)
        {
            _frameNeighbors = frameNeighbors;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
                return false;
            _tiles[x, y].ClearTile();
            if (_frameNeighbors)
            {
                WorldGen.TileFrame(x + 1, y);
                WorldGen.TileFrame(x - 1, y);
                WorldGen.TileFrame(x, y + 1);
                WorldGen.TileFrame(x, y - 1);
            }
            return UnitApply(origin, x, y, args);
        }
    }
    #endregion

    #region Custom Conditions
    internal class IsNotSloped : GenCondition
    {
        protected override bool CheckValidity(int x, int y)
        {
            return _tiles[x, y].HasTile && _tiles[x, y].Slope == 0 && !_tiles[x, y].IsHalfBlock;
        }
    }
    internal class IsSloped : GenCondition
    {
        protected override bool CheckValidity(int x, int y)
        {
            return _tiles[x, y].HasTile && (_tiles[x, y].Slope > 0 || _tiles[x, y].IsHalfBlock);
        }
    }
    #endregion

    #region Custom Modifiers

    #endregion
}