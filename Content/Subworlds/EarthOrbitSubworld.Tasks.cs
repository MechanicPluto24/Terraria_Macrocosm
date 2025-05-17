using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Items.Refined;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.Tiles.Furniture.Industrial;
using Macrocosm.Content.WorldGeneration.Structures;
using Macrocosm.Content.WorldGeneration.Structures.Orbit.Earth;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.WorldBuilding;
using static Macrocosm.Common.Utils.Utility;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Subworlds
{
    public partial class EarthOrbitSubworld
    {
        private StructureMap gen_StructureMap;
        private int gen_spawnExclusionRadius;

        [Task]
        private void PrepareTask(GenerationProgress progress)
        {
            gen_StructureMap = new();
            gen_spawnExclusionRadius = 200;
        }

        [Task]
        private void SpawnTask(GenerationProgress progress)
        {
            Main.spawnTileX = Main.maxTilesX / 2;
            Main.spawnTileY = Main.maxTilesY / 2;
        }

        [Task]
        private void AsteroidTask(GenerationProgress progress)
        {
            // I really do not care if they overlap eachother. BUT they do need to protect the area they spawn in -- Clyder
            for (int x = 50; x < Main.maxTilesX - 50; x++)
            {
                for (int y = 50; y < Main.maxTilesY - 50; y++)
                {
                    // Don't spawn asteroids too close to the spawn area
                    if (Math.Abs(x - Main.spawnTileX) < gen_spawnExclusionRadius &&
                        Math.Abs(y - Main.spawnTileY) < gen_spawnExclusionRadius)
                        continue;

                    if (WorldGen.genRand.NextBool(80000))
                    {
                        //int wallType = VariantWall.WallType<AstrolithWall>(WallSafetyType.Natural);
                        int wallType = 0;
                        BlobTileRunner(x, y, (ushort)TileType<Astrolith>(), 0..8, 1..4, 4..6, 1f, 4, wallType: (ushort)wallType);

                        // Very small chance to create a flesh meteor
                        if (WorldGen.genRand.NextBool(20))
                        {
                            ForEachInCircle(
                                i: x,
                                j: y,
                                radius: 3,
                                (i1, j1) =>
                                {
                                    if (!WorldGen.InWorld(i1, j1))
                                        return;

                                    float iDistance = Math.Abs(x - i1) / (3 * 0.5f);
                                    float jDistance = Math.Abs(y - j1) / (3 * 0.5f);
                                    if (WorldGen.genRand.NextFloat() < iDistance * 0.2f || WorldGen.genRand.NextFloat() < jDistance * 0.2f)
                                        return;

                                    if (Main.tile[i1, j1].HasTile)
                                        FastPlaceTile(i1, j1, TileID.FleshBlock);
                                }
                            );
                        }

                        gen_StructureMap.AddProtectedStructure(new Rectangle(x - 10, y - 10, x + 10, y + 10), padding: 1);
                    }
                }
            }
        }

        [Task]
        private void OreTask(GenerationProgress progress)
        {
            GenerateOre(TileType<Tiles.Ores.LithiumOre>(), 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), TileType<Astrolith>());
            GenerateOre(TileType<Tiles.Ores.AluminumOre>(), 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), TileType<Astrolith>());
            GenerateOre(TileID.Iron, 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), TileType<Astrolith>());
            GenerateOre(TileID.Gold, 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), TileType<Astrolith>());
            GenerateOre(TileID.Cobalt, 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), TileType<Astrolith>());
            GenerateOre(TileID.Titanium, 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), TileType<Astrolith>());
            GenerateOre(TileID.Meteorite, 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), TileType<Astrolith>());
        }

        [Task(weight: 12.0)]
        private void SmoothTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.SmoothTask");
            SmoothWorld(progress);
        }

        [Task]
        private void WallCleanupTask(GenerationProgress progress)
        {
            for (int x = 1; x < Main.maxTilesX; x++)
            {
                for (int y = 1; y < Main.maxTilesY; y++)
                {
                    Tile tile = Main.tile[x, y];

                    if (!tile.HasTile || tile.BlockType != BlockType.Solid)
                    {
                        //if (tile.WallType == VariantWall.WallType<AstrolithWall>())
                        //    tile.WallType = 0;
                    }

                }
            }
        }

        [Task]
        private void SpaceStationTask(GenerationProgress progress)
        {
            Structure module = Structure.Get<BaseSpaceStationModule>();
            Point16 origin = new(Main.spawnTileX + module.Size.X / 2, Main.spawnTileY);
            module.Place(origin, gen_StructureMap, padding: gen_spawnExclusionRadius);
        }

        [Task]
        private void StructureTask(GenerationProgress progress)
        {
            // Common structures
            for (int x = 50; x < Main.maxTilesX - 50; x++)
            {
                for (int y = 50; y < Main.maxTilesY - 50; y++)
                {
                    if (WorldGen.genRand.NextBool(50000))
                    {
                        int random = WorldGen.genRand.Next(7);
                        Structure structure = random switch
                        {
                            1 => Structure.Get<SpaceJunk1>(),
                            2 => Structure.Get<SpaceJunk2>(),
                            3 => Structure.Get<SpaceJunk3>(),
                            4 => Structure.Get<SpaceJunk4>(),
                            5 => Structure.Get<SpaceLoot1>(),
                            6 => Structure.Get<SpaceLoot2>(),
                            _ => Structure.Get<SpaceLoot3>(),
                        };

                        if (gen_StructureMap.CanPlace(new Rectangle(x - 10, y - 10, structure.Size.X + 10, structure.Size.Y + 10)))
                        {
                            structure.Place(new(x, y), gen_StructureMap);
                        }
                    }
                }

            }

            // Rare structures
            int maximumRares = WorldGen.genRand.Next(3);
            int placed = 0;
            for (int x = 50; x < Main.maxTilesX * 0.4f; x++)
            {
                for (int y = 50; y < Main.maxTilesY - 50; y++)
                {
                    if (WorldGen.genRand.NextBool(100000) && placed < maximumRares && Math.Abs(Main.spawnTileX - x) > 200)
                    {
                        if (WorldGen.genRand.NextBool(3))
                        {
                            int random = WorldGen.genRand.Next(3); // We'll add more to this list
                            Structure structure = random switch
                            {
                                0 => Structure.Get<RareStructure1>(),
                                1 => Structure.Get<LCShip1>(),
                                _ => Structure.Get<LCShip2>(),
                            };

                            if (gen_StructureMap.CanPlace(new Rectangle(x - 10, y - 10, structure.Size.X + 10, structure.Size.Y + 10)))
                            {
                                structure.Place(new(x, y), gen_StructureMap);
                                placed++;
                            }
                        }
                        else
                        {
                            if (gen_StructureMap.CanPlace(new Rectangle(x - 40, y - 40, 40, 40)))
                            {
                                //int wallType = VariantWall.WallType<AstrolithWall>(WallSafetyType.Natural);
                                int wallType = 0;

                                gen_StructureMap.AddProtectedStructure(new Rectangle(x - 40, y - 40, x + 40, y + 40), padding: 5);
                                BlobTileRunner(x, y, TileType<Astrolith>(), 5..12, 6..15, 20..30, 1f, 4, wallType: (ushort)wallType);
                                BlobTileRunner(x, y, TileType<Astrolith>(), 3..6, 1..5, 30..35, 1f, 4, wallType: (ushort)wallType);

                                BlobTileRunner(x, y, TileID.ShimmerBlock, 3..5, 1..2, 17..20, 1f, 4);
                                BlobLiquidTileRunner(x, y, 3, 1..2, 0..1, 10..15, 1f, 4);

                                ForEachInCircle(
                                    x,
                                    y,
                                    15,
                                    (i1, j1) =>
                                    {
                                        if (!WorldGen.InWorld(i1, j1))
                                            return;

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
                                        if (!WorldGen.InWorld(i1, j1))
                                            return;

                                        if (WorldGen.genRand.NextFloat() > 0.06f)
                                            return;

                                        if (Main.tile[i1, j1].HasTile)
                                        {
                                            FastPlaceTile(i1, j1, TileID.ShimmerBlock);
                                        }
                                    }
                                );

                                placed++;
                            }
                        }
                    }
                }

            }
        }

        [Task]
        private void LootTask(GenerationProgress progress)
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
                        chest.item[slot].SetDefaults(ItemID.IronOre);
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
