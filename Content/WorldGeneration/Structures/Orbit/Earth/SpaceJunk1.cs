using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Tiles.Rubble;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria;
using System.Collections;
using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Blocks;
using Macrocosm.Content.Walls;
using Microsoft.Xna.Framework;

namespace Macrocosm.Content.WorldGeneration.Structures.Orbit.Earth;

public class SpaceJunk1 : Structure
{
    public sealed override void PostPlace(Point16 origin)
    {

        ushort[] clearableTiles = [
            (ushort)ModContent.TileType<IndustrialPlating>()
        ];

        ushort[] clearableWalls = [
            (ushort)VariantWall.WallType<IndustrialPlatingWall>(),
            (ushort)VariantWall.WallType<IndustrialHazardWall>(),
            (ushort)VariantWall.WallType<IndustrialSquarePaneledWall>(),
            (ushort)VariantWall.WallType<IndustrialTrimmingWall>(),
            (ushort)VariantWall.WallType<IndustrialPlatingWall>(WallSafetyType.Natural),
            (ushort)VariantWall.WallType<IndustrialHazardWall>(WallSafetyType.Natural),
            (ushort)VariantWall.WallType<IndustrialSquarePaneledWall>(WallSafetyType.Natural),
            (ushort)VariantWall.WallType<IndustrialTrimmingWall>(WallSafetyType.Natural),
        ];

        // Age room
        WorldUtils.Gen(new Point(origin.X, origin.Y), new Shapes.Rectangle(Size.X, Size.Y), Actions.Chain(new Modifiers.Dither(0.85), new Modifiers.Blotches(), new Modifiers.OnlyWalls(clearableWalls), new Actions.ClearWall(true)));
        WorldUtils.Gen(new Point(origin.X, origin.Y), new Shapes.Rectangle(Size.X, Size.Y), Actions.Chain(new Modifiers.Dither(0.95), new Modifiers.OnlyTiles(clearableTiles), new Actions.ClearTile(frameNeighbors: true)));

        for (int i = origin.X; i < origin.X + Size.X; i++)
        {
            for (int j = origin.Y; j < origin.Y + Size.Y; j++)
            {
                Tile tile = Main.tile[i, j];
                if (TileSets.RandomStyles[tile.TileType] > 1)
                    Utility.SetTileStyle(i, j, WorldGen.genRand.Next(TileSets.RandomStyles[tile.TileType]), WorldGen.genRand.Next(2));

                if (WorldGen.genRand.NextBool() && Main.tile[i, j].GetModTile() is IToggleableTile toggleable)
                    toggleable.ToggleTile(i, j);
            }
        }

        // Make walls unsafe
        Utility.ConvertWallSafetyInArea(origin.X, origin.Y, Size.X, Size.Y, WallSafetyType.Natural);

        float smallWireSpawnChance = 0.11f;
        float mediumWireSpawnChance = 0.1f;
        float largeWireSpawnChance = 0.05f;
        ushort industrialPlatingType = (ushort)ModContent.TileType<IndustrialPlating>();

        for (int i = origin.X; i < origin.X + Size.X; i++)
        {
            for (int j = origin.Y; j < origin.Y + Size.Y - 2; j++)
            {
                Tile tile = Main.tile[i, j];

                if (WorldGen.genRand.NextFloat() < largeWireSpawnChance)
                {
                    if (tile.TileType == industrialPlatingType)
                        Utility.TryPlaceObject(i, j + 1, ModContent.TileType<LooseWiresLargeNatural>(), style: WorldGen.genRand.Next(3));
                }
                if (WorldGen.genRand.NextFloat() < mediumWireSpawnChance)
                {
                    if (tile.TileType == industrialPlatingType)
                        Utility.TryPlaceObject(i, j + 1, ModContent.TileType<LooseWiresMediumNatural>(), style: WorldGen.genRand.Next(2));
                }
                if (WorldGen.genRand.NextFloat() < smallWireSpawnChance)
                {
                    if (tile.TileType == industrialPlatingType)
                        Utility.TryPlaceObject(i, j + 1, ModContent.TileType<LooseWiresSmallNatural>(), style: WorldGen.genRand.Next(2));
                }

            }
        }
    }
}
