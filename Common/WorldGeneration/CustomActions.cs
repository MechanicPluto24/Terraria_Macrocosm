using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration
{
    public class CustomActions
    {
        // By GroxTheGreat
        public class SetModTile : GenAction
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

        // By GroxTheGreat
        public class SetMapBrightness : GenAction
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

        // By GroxTheGreat
        public class RadialDitherTopMiddle : GenAction
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

        // By GroxTheGreat
        public class PlaceModWall : GenAction
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

        // By GroxTheGreat
        public class IsInWorld : GenAction
        {
            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY)
                    return Fail();
                return UnitApply(origin, x, y, args);
            }
        }

        // By GroxTheGreat
        public class ClearTileSafely : GenAction
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
    }
}
