using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Items.Weapons.Melee;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Tiles.Blocks;
using Macrocosm.Content.Tiles.Furniture.Industrial;
using Macrocosm.Content.Tiles.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.WorldGeneration.Structures
{
    [Obsolete("Replaced with LunarHouse")]
    public class MoonBaseOutpost : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            Rectangle room = GetRoom(origin);

            if (!WorldGen.InWorld(room.X, room.Y))
                return false;

            if (!structures.CanPlace(room))
                return false;

            // Base structure
            WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Actions.SetTileKeepWall((ushort)TileType<IndustrialPlating>()), new Actions.SetFrames(frameNeighbors: true)));

            // Walls
            WorldUtils.Gen(new Point(room.X + 1, room.Y + 1), new Shapes.Rectangle(room.Width - 2, room.Height - 2), Actions.Chain(new Actions.ClearTile(frameNeighbors: true), new Actions.PlaceWall((ushort)WallType<IndustrialPlatingWall>())));
            WorldUtils.Gen(new Point(room.X + 1, room.Y + room.Height - 2), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Actions.ClearWall(frameNeighbors: true), new Actions.PlaceWall((ushort)WallType<IndustrialHazardWall>())));

            StylizeRoomCorners(room);

            // Check for tile blockage to the left, add door if clear
            var info = new TileNeighbourInfo(room.X - 1, room.Y + room.Height - 1 - 2).Solid;
            if (!Main.tile[room.X - 1, room.Y + room.Height - 1 - 2].HasTile && info.Count <= 3)
            {
                WorldUtils.Gen(new Point(room.X, room.Y + room.Height - 1 - 3), new Shapes.Rectangle(1, 3), new Actions.ClearTile(frameNeighbors: true));
                WorldGen.PlaceTile(room.X, room.Y + room.Height - 1 - 3, TileType<IndustrialDoorClosed>(), true);
            }

            info = new TileNeighbourInfo(room.X + room.Width, room.Y + room.Height - 1 - 2).Solid;
            if (!Main.tile[room.X + room.Width, room.Y + room.Height - 1 - 2].HasTile && info.Count <= 3)
            {
                WorldUtils.Gen(new Point(room.X + room.Width - 1, room.Y + room.Height - 1 - 3), new Shapes.Rectangle(1, 3), new Actions.ClearTile(frameNeighbors: true));
                WorldGen.PlaceTile(room.X + room.Width - 1, room.Y + room.Height - 1 - 3, TileType<IndustrialDoorClosed>(), true);
            }

            // Place the chest
            var mainLoot = new List<int>
            {
                ItemType<ClawWrench>(),
                ItemType<HandheldEngine>(),
                ItemType<Copernicus>(),
                //ItemType<SomeSummonerWeapon>(),
            };
            Utility.GenerateChest(room.X + room.Width / 2, room.Y + room.Height - 2, TileType<IndustrialChest>(), 0,
                [
                    mainLoot.GetRandom(WorldGen.genRand),
                    ItemID.SuperHealingPotion,
                    ItemID.SuperManaPotion
                ],
                [
                    1,
                    WorldGen.genRand.Next(10),
                    WorldGen.genRand.Next(8),
                ],
                randomPrefix: true
            );

            structures.AddProtectedStructure(room, padding: 10);

            return true;
        }

        private Rectangle GetRoom(Point origin)
        {
            bool solidDown = WorldUtils.Find(origin, Searches.Chain(new Searches.Down(25), new Conditions.IsSolid()), out Point solidGround);

            if (!solidDown)
                return new(-1, -1, 0, 0);

            int maxY = Terraria.Utils.Clamp(solidGround.Y - origin.Y, 8, 12);
            int roomHeight = (int)(maxY * 0.75);

            bool solidLeft = WorldUtils.Find(origin, Searches.Chain(new Searches.Left(5), new Conditions.IsSolid()), out Point leftBound);
            bool solidRight = WorldUtils.Find(origin, Searches.Chain(new Searches.Right(5), new Conditions.IsSolid()), out Point rightBound);
            if (!solidLeft) leftBound = new Point(origin.X - 5, origin.Y);
            if (!solidRight) rightBound = new Point(origin.X + 5, origin.Y);

            int maxX = Terraria.Utils.Clamp(rightBound.X - leftBound.X, 0, 10);
            Rectangle room = new Rectangle(0, 0, 12, 7);
            room.X = Math.Min(leftBound.X, rightBound.X - maxX);
            room.Y = solidGround.Y - roomHeight;
            return room;
        }

        private void StylizeRoomCorners(Rectangle room)
        {
            var tileTopLeft = Main.tile[room.X, room.Y];
            var tileTopRight = Main.tile[room.X + room.Width - 1, room.Y];

            TileNeighbourInfo infoTopLeft = new(room.X, room.Y);
            TileNeighbourInfo infoTopRight = new(room.X + room.Width - 1, room.Y);

            if (infoTopLeft.Solid.Count == 2)
                tileTopLeft.Slope = SlopeType.SlopeDownRight;

            if (infoTopRight.Solid.Count == 2)
                tileTopRight.Slope = SlopeType.SlopeDownLeft;


            Tile tile = Main.tile[room.X + 1, room.Y + 1];
            if (!tile.HasTile)
            {
                WorldGen.PlaceTile(room.X + 1, room.Y + 1, TileType<IndustrialPlating>(), true);
                tile.Slope = SlopeType.SlopeUpLeft;
            }

            tile = Main.tile[room.X + room.Width - 2, room.Y + 1];
            if (!tile.HasTile)
            {
                WorldGen.PlaceTile(room.X + room.Width - 2, room.Y + 1, TileType<IndustrialPlating>(), true);
                tile.Slope = SlopeType.SlopeUpRight;
            }
        }
    }
}
