using Macrocosm.Common.Utils;
using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Items.Ores;
using Macrocosm.Content.Items.Refined;
using Macrocosm.Content.Subworlds.Orbit.Earth;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.Tiles.Furniture.Industrial;
using Macrocosm.Content.Walls;
using Macrocosm.Content.WorldGeneration.Structures;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.WorldBuilding;
using static Macrocosm.Common.Utils.Utility;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Subworlds
{
    public partial class EarthOrbitSubworld
    {
        public StructureMap OrbitStructureMap { get; private set; } = new();

        [Task]
        private void PlaceSpawn(GenerationProgress progress)
        {

            Structure module = new BaseSpaceStationModule();
            int x, y;
            x = (int)(Main.maxTilesX / 2);
            y = (int)(Main.maxTilesY / 2);
            Point16 origin = new(Main.spawnTileX + (int)(module.Size.X / 2), Main.spawnTileY);
            module.Place(origin, null);
        }
        [Task]
        private void Asteroids(GenerationProgress progress)
        {

            //I really do not care if they overlap eachother. BUT they do need to protect the area they spawn in
            for (int x = 50; x < (int)Main.maxTilesX - 50; x++)
            {
                for (int y = 50; y < Main.maxTilesY - 50; y++)
                {
                    if (WorldGen.genRand.NextBool(80000) && Math.Abs(Main.spawnTileX - x) > 200)
                    {
                        Utility.BlobTileRunner(x, y, (ushort)TileType<Protolith>(), 0..3, 1..4, 4..6, 1f, 4, wallType: (ushort)WallType<ProtolithWall>());
                        //very small chance to create a flesh meteor
                        if (WorldGen.genRand.NextBool(20))
                        {
                            ForEachInCircle(
                                         x,
                                         y,
                                         3,
                                         (i1, j1) =>
                                         {
                                             if (CoordinatesOutOfBounds(i1, j1))
                                             {
                                                 return;
                                             }

                                             float iDistance = Math.Abs(x - i1) / (3 * 0.5f);
                                             float jDistance = Math.Abs(y - j1) / (3 * 0.5f);
                                             if (WorldGen.genRand.NextFloat() < iDistance * 0.2f || WorldGen.genRand.NextFloat() < jDistance * 0.2f)
                                             {
                                                 return;
                                             }

                                             if (Main.tile[i1, j1].HasTile)
                                             {
                                                 FastPlaceTile(i1, j1, TileID.FleshBlock);
                                             }
                                         }
                                     );
                        }

                        OrbitStructureMap.AddProtectedStructure(new Rectangle(x - 10, y - 10, x + 10, y + 10), padding: 1);
                    }
                }
            }

            //Let there be ores
            int protolithType = TileType<Protolith>();
            GenerateOre(TileType<Tiles.Ores.LithiumOre>(), 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), protolithType);
            GenerateOre(TileType<Tiles.Ores.AluminumOre>(), 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), protolithType);
            GenerateOre(TileID.Iron, 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), protolithType);
            GenerateOre(TileID.Gold, 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), protolithType);
            GenerateOre(TileID.Cobalt, 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), protolithType);
            GenerateOre(TileID.Titanium, 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), protolithType);
            GenerateOre(TileID.Meteorite, 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), protolithType);

        }
        private Structure GetACommonStructure()
        {
            int random = WorldGen.genRand.Next(0, 7);
            Structure structure = random switch
            {
                1 => new SpaceJunk1(),
                2 => new SpaceJunk2(),
                3 => new SpaceJunk3(),
                4 => new SpaceJunk4(),
                5 => new SpaceLoot1(),
                6 => new SpaceLoot2(),
                _ => new SpaceLoot3(),
            };
            return structure;
        }
        private Structure GetARareStructure()
        {
            int random = WorldGen.genRand.Next(0, 1);//We'll add more to this list
            Structure structure = random switch
            {
                _ => new RareStructure1(),
            };
            return structure;
        }
        [Task]
        private void Structures(GenerationProgress progress)
        {

            for (int x = 50; x < (int)Main.maxTilesX - 50; x++)
            {
                for (int y = 50; y < Main.maxTilesY - 50; y++)
                {
                    if (WorldGen.genRand.NextBool(50000) && Math.Abs(Main.spawnTileX - x) > 200)
                    {
                        Structure structure = GetACommonStructure();
                        if (OrbitStructureMap.CanPlace(new Rectangle(x - 10, y - 10, structure.Size.X + 10, structure.Size.Y + 10)))
                        {
                            structure.Place(new(x, y), OrbitStructureMap);
                        }
                    }
                }

            }

            int MaximumRares = WorldGen.genRand.Next(0, 3);
            int placed = 0;
            for (int x = 50; x < (int)Main.maxTilesX * 0.4f; x++)
            {
                for (int y = 50; y < Main.maxTilesY - 50; y++)
                {
                    if (WorldGen.genRand.NextBool(100000) && placed < MaximumRares && Math.Abs(Main.spawnTileX - x) > 200)
                    {
                        if (WorldGen.genRand.NextBool(3))
                        {
                            Structure structure = GetARareStructure();
                            if (OrbitStructureMap.CanPlace(new Rectangle(x - 10, y - 10, structure.Size.X + 10, structure.Size.Y + 10)))
                            {
                                structure.Place(new(x, y), OrbitStructureMap);
                                placed++;
                            }
                        }
                        else
                        {
                            if (WorldGen.genRand.NextBool(2))
                            {
                                if (OrbitStructureMap.CanPlace(new Rectangle(x - 40, y - 40, 40, 40)))
                                {
                                    OrbitStructureMap.AddProtectedStructure(new Rectangle(x - 40, y - 40, x + 40, y + 40), padding: 5);
                                    Utility.BlobTileRunner(x, y, (int)TileType<Protolith>(), 5..12, 6..15, 20..30, 1f, 4, wallType: (ushort)WallType<ProtolithWall>());
                                    Utility.BlobTileRunner(x, y, (int)TileType<Protolith>(), 3..6, 1..5, 30..35, 1f, 4, wallType: (ushort)WallType<ProtolithWall>());
                                    Utility.BlobTileRunner(x, y, TileID.ShimmerBlock, 3..5, 1..2, 17..20, 1f, 4);
                                    Utility.BlobLiquidTileRunner(x, y, 3, 1..2, 0..1, 10..15, 1f, 4);
                                    ForEachInCircle(
                                        x,
                                        y,
                                        15,
                                        (i1, j1) =>
                                        {
                                            if (CoordinatesOutOfBounds(i1, j1))
                                            {
                                                return;
                                            }

                                            float iDistance = Math.Abs(x - i1) / (15 * 0.5f);
                                            float jDistance = Math.Abs(y - j1) / (15 * 0.5f);
                                            if (WorldGen.genRand.NextFloat() < iDistance * 0.2f || WorldGen.genRand.NextFloat() < jDistance * 0.2f)
                                            {
                                                return;
                                            }

                                            if (Main.tile[i1, j1].HasTile)
                                            {
                                                FastPlaceTile(i1, j1, TileID.ShimmerBlock);
                                            }
                                        }
                                    );
                                    ForEachInCircle(
                                        x,
                                        y,
                                        35,
                                        (i1, j1) =>
                                        {
                                            if (CoordinatesOutOfBounds(i1, j1))
                                            {
                                                return;
                                            }
                                            if (WorldGen.genRand.NextFloat() > 0.06f)
                                            {
                                                return;
                                            }

                                            if (Main.tile[i1, j1].HasTile)
                                            {
                                                FastPlaceTile(i1, j1, TileID.ShimmerBlock);
                                            }
                                        }
                                    );

                                    placed++;
                                }
                            }
                            else
                            {
                                if (OrbitStructureMap.CanPlace(new Rectangle(x - 40, y - 40, x + 40, y + 40)))
                                {
                                    OrbitStructureMap.AddProtectedStructure(new Rectangle(x - 40, y - 40, x + 40, y + 40), padding: 5);
                                    Utility.BlobTileRunner(x, y, (int)TileType<Cynthalith>(), 1..8, 1..5, 20..25, 1f, 4, wallType: (ushort)WallType<RegolithWall>());
                                    Utility.BlobTileRunner(x, y, (int)TileType<Regolith>(), 1..8, 1..53, 20..25, 1f, 4);

                                    placed++;
                                }
                            }
                        }
                    }
                }

            }
        }
        [Task]
        private void Loot(GenerationProgress progress)
        {

            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest chest = Main.chest[i];
                if (chest != null)
                {
                    if (Main.tile[chest.x, chest.y].TileType == TileType<IndustrialChest>())
                    {
                        ManageIndustrialChest(chest, i);
                    }
                }
            }
            for (int x = 1; x < (int)Main.maxTilesX; x++)
            {
                for (int y = 1; y < Main.maxTilesY; y++)
                {
                    if (!Main.tile[x, y].HasTile)
                    {
                        if (Main.tile[x, y].WallType == WallType<ProtolithWall>() || Main.tile[x, y].WallType == WallType<RegolithWall>())
                        {
                            Tile tile = Main.tile[x, y];
                            tile.WallType = 0;
                        }
                    }

                }
            }
        }
        public void ManageIndustrialChest(Chest chest, int index)
        {
            int slot = 0;
            int random;

            random = WorldGen.genRand.Next(1, 3);
            switch (random)
            {
                case 1:
                    chest.item[slot].SetDefaults(ItemID.GreaterHealingPotion);
                    chest.item[slot++].stack = WorldGen.genRand.Next(5, 16);
                    break;

                case 2:
                    chest.item[slot].SetDefaults(ItemID.GreaterManaPotion);
                    chest.item[slot++].stack = WorldGen.genRand.Next(50, 90);
                    break;
            }

            for (int i = 0; i < 3; i++)
            {
                random = WorldGen.genRand.Next(1, 8);
                switch (random)
                {
                    case 1:
                        chest.item[slot].SetDefaults(ItemType<Plastic>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                    case 2:
                        chest.item[slot].SetDefaults(ItemType<NickelOre>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                    case 3:
                        chest.item[slot].SetDefaults(ItemID.TitaniumOre);
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                    case 4:
                        chest.item[slot].SetDefaults(ItemType<Items.Ores.LithiumOre>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                    case 5:
                        chest.item[slot].SetDefaults(ItemType<Items.Ores.AluminumOre>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                    case 6:
                        chest.item[slot].SetDefaults(ItemType<SteelBar>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                    case 7:
                        chest.item[slot].SetDefaults(ItemType<RocketFuelCanister>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                }
            }


            for (int i = 0; i < 2; i++)
            {
                random = WorldGen.genRand.Next(1, 3);
                switch (random)
                {
                    case 1:
                        chest.item[slot].SetDefaults(ItemType<Items.Blocks.IndustrialPlating>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                        break;
                    case 2:
                        chest.item[slot].SetDefaults(ItemType<Items.Blocks.LexanGlass>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                        break;

                }
            }

        }
    }
}
