using Macrocosm.Common.Utils;
using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Ambient;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.Tiles.Ores;
using Macrocosm.Content.Tiles.Furniture.Luminite;
using Macrocosm.Content.Tiles.Walls;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Items.Refined;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Items.Consumables.Throwable;
using Macrocosm.Content.Items.Weapons.Melee;
using Macrocosm.Content.Items.Accessories;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Items.Weapons.Summon;
using Macrocosm.Content.WorldGeneration.Structures;
using Macrocosm.Content.WorldGeneration.Structures.LunarOutposts;
using Macrocosm.Content.WorldGeneration.Structures.Shrines;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.WorldBuilding;
using static Macrocosm.Common.Utils.Utility;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Subworlds
{
    public partial class Moon
    {
        public int CynthalithlithLayerHeight => 65;
        public int RegolithLayerHeight => 200;
        private float SurfaceWidthFrequency => 0.003f;
        private float SurfaceHeightFrequency => 20f;
        private float TerrainPercentage => 0.8f;
        private int SpawnFlatArea => 50;
        private int GroundY => (int)(Main.maxTilesY * (1f - TerrainPercentage));
        private float SurfaceEquation(float x) => MathF.Sin(2f * x) + MathF.Sin(MathHelper.Pi * x) + 0.4f * MathF.Cos(10f * x);
        private int SurfaceHeight(int i)
        {
            float x = i * SurfaceWidthFrequency + gen_StartYOffset;
            float normalHeight = SurfaceEquation(x) * SurfaceHeightFrequency + GroundY;

            int spawnX = Main.spawnTileX;

            int flatWidth = SpawnFlatArea * 2; 
            int blendWidth = SpawnFlatArea; 

            int flatStart = spawnX - flatWidth / 2;
            int flatEnd = spawnX + flatWidth / 2;

            int blendStart = flatStart - blendWidth;
            int blendEnd = flatEnd + blendWidth;

            float flatHeight = SurfaceEquation(spawnX * SurfaceWidthFrequency + gen_StartYOffset) * SurfaceHeightFrequency + GroundY;

            if (i >= flatStart && i <= flatEnd)
            {
                return (int)flatHeight;
            }
            else if (i >= blendStart && i < flatStart)
            {
                float t = (i - blendStart) / (float)(flatStart - blendStart);
                float height = MathHelper.Lerp(normalHeight, flatHeight, t);
                return (int)height;
            }
            else if (i > flatEnd && i <= blendEnd)
            {
                float t = (i - flatEnd) / (float)(blendEnd - flatEnd);
                float height = MathHelper.Lerp(flatHeight, normalHeight, t);
                return (int)height;
            }
            else
            {
                return (int)normalHeight;
            }
        }

        private bool gen_IsIrradiationRight;
        private float gen_StartYOffset;

        private Point gen_LuminiteShrinePosition;
        private Point gen_HeavenforgeShrinePosition;
        private Point gen_MercuryShrinePosition;
        private Point gen_LunarRustShrinePosition;
        private Point gen_StarRoyaleShrinePosition;
        private Point gen_AstraShrinePosition;
        private Point gen_CryocoreShrinePosition;
        private Point gen_DarkCelestialShrinePosition;
        private Point gen_CosmicEmberShrinePosition;

        private StructureSH DetermineLunarHouse()
        {
            int i = WorldGen.genRand.Next(0, 9);
            return i switch
            {
                0 => new LunarHouse1(),
                1 => new LunarHouse2(),
                2 => new LunarHouse3(),
                3 => new LunarHouse4(),
                4 => new LunarHouse5(),
                5 => new LunarHouse6(),
                6 => new LunarHouse7(),
                7 => new LunarHouse8(),
                8 => new LunarHouse9(),
                _ => null,
            };
        }

        [Task]
        private void PrepareTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.TerrainPass");

            gen_IsIrradiationRight = WorldGen.genRand.NextBool();
        }

        [Task]
        private void TerrainTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.TerrainPass");

            ushort protolithType = (ushort)TileType<Protolith>();

            Main.worldSurface = GroundY + SurfaceHeightFrequency * 2;
            Main.rockLayer = GroundY + RegolithLayerHeight;

            gen_StartYOffset = WorldGen.genRand.NextFloat() * 2.3f;

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int startJ = SurfaceHeight(i);
                for (int j = startJ; j < Main.maxTilesY; j++)
                {
                    progress.Set((float)(j + i * Main.maxTilesY) / (Main.maxTilesX * Main.maxTilesY));
                    FastPlaceTile(i, j, protolithType);
                }
            }

            int regolithWall = WallType<RegolithWall>();
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                progress.Set(0.5d * i / Main.maxTilesX);
                int wallPlaceStart = SurfaceHeight(i) + 15;
                for (int j = wallPlaceStart; j < wallPlaceStart + RegolithLayerHeight; j++)
                {
                    FastPlaceWall(i, j, regolithWall);
                }
            }
        }

        [Task]
        private void CraterTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CraterPass");

            void CarveCraters(Range minMaxCount, Range minMaxRadius)
            {
                int count = WorldGen.genRand.Next(minMaxCount);
                for (int x = 0; x < count; x++)
                {
                    int i = (int)((x + WorldGen.genRand.NextFloat() * 0.9f) * (Main.maxTilesX / count));

                    if (Math.Abs(i - (Main.maxTilesX / 2)) < Math.Max(SpawnFlatArea, minMaxRadius.End.Value))
                        continue;

                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        if (Main.tile[i, j].HasTile)
                        {
                            int radius = WorldGen.genRand.Next(minMaxRadius);
                            ForEachInCircle(
                                i,
                                j - (int)(radius * 0.6f),
                                radius,
                                (i1, j1) =>
                                {
                                    FastRemoveWall(i1, j1);
                                    float iDistance = (float)Math.Abs(i - i1) / radius;
                                    float jDistance = (float)Math.Abs(j - j1) / radius;
                                    FastRemoveTile(i1, j1);
                                }
                            );

                            break;
                        }
                    }
                }
            }

            CarveCraters(2..5, 100..150);
            CarveCraters(5..6, 30..40);
            CarveCraters(25..36, 7..15);
        }

        [Task]
        private void CaveTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            double smallCaveFreq = 0.0009;
            double largeCaveFreq = 0.00013;

            int airTileType = -1;

            List<Point> smallCaves = new();
            List<Point> largeCaves = new();

            // generate small caves in the protolith layer 
            for (int smallCaveSpot = 0; smallCaveSpot < (int)((double)(Main.maxTilesX * Main.maxTilesY) * smallCaveFreq); smallCaveSpot++)
            {
                float percentDone = (float)((double)smallCaveSpot / ((double)(Main.maxTilesX * Main.maxTilesY) * smallCaveFreq));
                progress.Set(percentDone * 0.5f);

                int tileX = WorldGen.genRand.Next(0, Main.maxTilesX);
                int tileY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);

                // really small holes 
                WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 20), airTileType);

                tileX = WorldGen.genRand.Next(0, Main.maxTilesX);
                tileY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);
                while (((double)tileY < Main.rockLayer) || ((double)tileX > (double)Main.maxTilesX * 0.45 && (double)tileX < (double)Main.maxTilesX * 0.55 && (double)tileY < Main.rockLayer))
                {
                    tileX = WorldGen.genRand.Next(0, Main.maxTilesX);
                    tileY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);
                }

                // small caves 
                smallCaves.Add(new Point(tileX, tileY));
                WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(7, 30), airTileType);
            }

            //generate large caves 
            for (int largeCaveSpot = 0; largeCaveSpot < (int)((double)(Main.maxTilesX * Main.maxTilesY) * largeCaveFreq); largeCaveSpot++)
            {
                float percentDone = (float)((double)largeCaveSpot / ((double)(Main.maxTilesX * Main.maxTilesY) * largeCaveFreq));
                progress.Set(0.5f + percentDone * 0.5f);

                int tileX = WorldGen.genRand.Next(0, Main.maxTilesX);
                int tileY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);
                largeCaves.Add(new Point(tileX, tileY));
                WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(5, 26), WorldGen.genRand.Next(50, 350), airTileType);
            }
        }

        [Task]
        private void SurfaceTunnelTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            float verticalTunnelSpawnChance = 0.005f;
            int verticalTunnelSpread = 230;
            int verticalTunnelLength = RegolithLayerHeight + 170;
            int verticalTunnelSize = 11;

            int skipI = 0;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                progress.Set((float)i / Main.maxTilesX);

                if (skipI > 0)
                {
                    skipI--;
                    continue;
                }

                if (Math.Abs(i - (Main.maxTilesX / 2)) < SpawnFlatArea)
                    continue;

                if (WorldGen.genRand.NextFloat() < verticalTunnelSpawnChance)
                {
                    skipI = verticalTunnelSpread;
                    int surfaceHeight = SurfaceHeight(i);
                    float eqOffset = WorldGen.genRand.NextFloat() * 10.25f;
                    float tunnelLength = verticalTunnelLength * WorldGen.genRand.NextFloat(0.45f, 1.2f);
                    float tunnelSize = verticalTunnelSize * WorldGen.genRand.NextFloat(0.6f, 1f);
                    for (int j = 0; j < tunnelLength; j += (int)(tunnelSize * 0.66f))
                    {
                        int radius = (int)(((SurfaceEquation(j * 0.01f + eqOffset * 2f) + 1f) * 0.1f + 0.8f) * tunnelSize);

                        int iPos = i + (int)(SurfaceEquation(j * 0.005f + eqOffset) * tunnelSize * 3.5f);
                        int jPos = surfaceHeight + j;
                        ForEachInCircle(
                            iPos,
                            jPos,
                            radius,
                            FastRemoveTile
                        );
                    }
                }
            }
        }

        [Task]
        private void WallTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.WallPass");
            int protolithWall = WallType<ProtolithWall>();

            int wallCount = 100;
            for (int z = 0; z < wallCount; z++)
            {
                progress.Set(0.5d + 0.5d * z / wallCount);
                Point point = new();

                do
                {
                    int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                    point = new Point(x, WorldGen.genRand.Next(SurfaceHeight(x) + 15 + RegolithLayerHeight, Main.maxTilesY));
                }
                while (
                    Main.tile[point.X, point.Y].HasTile ||
                    Main.tile[point.X, point.Y].WallType == protolithWall
                );

                if (ConnectedTiles(point.X, point.Y, tile => !tile.HasTile, out var coordinates, 9999))
                {
                    foreach ((int i, int j) in coordinates)
                    {
                        FastPlaceWall(i, j, protolithWall);
                    }
                }
            }
        }
        [Task]
        private void PrepareLuminiteShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            Structure shrine = new LuminiteShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 3.5 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 3.5 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.45f), (int)(Main.maxTilesY * 0.55f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 30);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y - shrine.Size.Y / 2), new CustomShapes.ChasmSideways(12, 8, 80, 2, 0, dir: true), new Actions.ClearTile());
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y - shrine.Size.Y / 2), new CustomShapes.ChasmSideways(12, 8, 80, 2, 0, dir: false), new Actions.ClearTile());

            gen_LuminiteShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void PrepareHeavenforgeShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            StructureSH shrine = new HeavenforgeShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 0 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 0 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 30);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(12, 8, 80, 2, 0, dir: true), new Actions.ClearTile());
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(12, 8, 80, 2, 0, dir: false), new Actions.ClearTile());

            gen_HeavenforgeShrinePosition = new(x + shrine.Size.X / 2, y + shrine.Size.Y / 2);
        }

        [Task]
        private void PrepareMercuryShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            StructureSH shrine = new MercuryShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 1 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 1 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 30);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y), new CustomShapes.Chasm(30, 1, 10, 2, 0, dir: false), Actions.Chain(new Actions.ClearTile(), new Actions.ClearWall()));
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y), new CustomShapes.Chasm(30, 10, 140, 2, 0, dir: true), Actions.Chain(new Actions.ClearTile(), new Actions.ClearWall()));

            gen_MercuryShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void PrepareLunarRustShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            StructureSH shrine = new LunarRustShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 2 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 2 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 30);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y), new CustomShapes.Chasm(65, 1, 10, 2, 0, dir: false), Actions.Chain(new Actions.ClearTile(), new Actions.ClearWall()));
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y), new CustomShapes.Chasm(65, 10, 180, 2, 0, dir: true), Actions.Chain(new Actions.ClearTile(), new Actions.ClearWall()));

            gen_LunarRustShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void PrepareStarRoyaleShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            StructureSH shrine = new StarRoyaleShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 3 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 3 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 30);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(30, 10, 100, 3, 0, dir: true), new Actions.ClearTile());
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(30, 10, 100, 3, 0, dir: false), new Actions.ClearTile());
            WorldGen.TileRunner(x + shrine.Size.X / 2, y + shrine.Size.Y / 2, shrine.Size.X * 1.2, 2, -1);

            gen_StarRoyaleShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void PrepareCryocoreShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            StructureSH shrine = new CryocoreShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 4 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 4 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 30);

            //BlobTileRunner(, -1, 1..2, 1..10, (shrine.Size.X-1)..(), 1, 2);
            WorldGen.TileRunner(x + shrine.Size.X / 2, y + shrine.Size.Y / 2 - 1, shrine.Size.X * 1.2, 1, -1);

            gen_CryocoreShrinePosition = new(x + shrine.Size.X / 2, y + shrine.Size.Y / 2 - 1);

        }

        [Task]
        private void PrepareAstraShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            StructureSH shrine = new AstraShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 5 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 5 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 30);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(18, 16, 50, 2, 0, dir: true), new Actions.ClearTile());
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(18, 16, 50, 2, 0, dir: false), new Actions.ClearTile());
            WorldGen.TileRunner(x + shrine.Size.X / 2, y + shrine.Size.Y / 2 - 1, shrine.Size.X * 1.25f, 1, -1);

            gen_AstraShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void PrepareDarkCelestialShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            StructureSH shrine = new DarkCelestialShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 6 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 6 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 30);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(30, 10, 100, 3, 0, dir: true), new Actions.ClearTile());
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(30, 10, 100, 3, 0, dir: false), new Actions.ClearTile());
            WorldGen.TileRunner(x + shrine.Size.X / 2, y + shrine.Size.Y / 2, shrine.Size.X * 1.2, 3, -1);

            gen_DarkCelestialShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void PrepareCosmicEmberShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            StructureSH shrine = new CosmicEmberShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 7 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 7 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 30);

            WorldGen.TileRunner(x + shrine.Size.X / 2, y + shrine.Size.Y / 2 - 1, shrine.Size.X * 2f, 1, -1);

            gen_CosmicEmberShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void RegolithTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            float randomOffset = WorldGen.genRand.NextFloat() * 4.23f;
            ushort regolithType = (ushort)TileType<Regolith>();

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int offset = (int)(SurfaceEquation(i * 0.02f + randomOffset) * 9f);
                int surfaceHeight = SurfaceHeight(i);
                for (int j = surfaceHeight; j < surfaceHeight + RegolithLayerHeight; j++)
                {
                    if (!Main.tile[i, j].HasTile)
                    {
                        continue;
                    }

                    if (j == surfaceHeight + RegolithLayerHeight - 1 && j % 2 == 0)
                    {
                        int veinLength = WorldGen.genRand.Next(10) switch
                        {
                            > 7 => 26,
                            _ => 8,
                        };
                        float veinEqOffset = WorldGen.genRand.NextFloat() * 12.2f;
                        int jOffset = 0;
                        while (jOffset < veinLength)
                        {
                            ForEachInCircle(
                                i + (int)(SurfaceEquation(jOffset * 0.1f + veinEqOffset) * 2f),
                                j + jOffset,
                                (float)jOffset / veinLength > 0.6f ? 1 : 2,
                                (i1, j1) =>
                                {
                                    if (CoordinatesOutOfBounds(i1, j1) || !Main.tile[i1, j1].HasTile)
                                    {
                                        return;
                                    }

                                    FastPlaceTile(i1, j1, regolithType);
                                }
                            );

                            jOffset += 1;
                        }
                    }

                    FastPlaceTile(i, j, regolithType);
                }
            }
        }

        [Task]
        private void CynthalithTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.Cynthalith");

            float randomOffset = WorldGen.genRand.NextFloat() * 4.23f;
            ushort cynthalithType = (ushort)TileType<Cynthalith>();

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int offset = (int)(SurfaceEquation(i * 0.02f + randomOffset) * 3f);
                int surfaceHeight = SurfaceHeight(i);
                for (int j = surfaceHeight + CynthalithlithLayerHeight; j < surfaceHeight + RegolithLayerHeight + 30; j++)
                {
                    if (!Main.tile[i, j].HasTile)
                    {
                        continue;
                    }
                    FastPlaceTile(i, j, cynthalithType);

                    float Distance = Math.Abs(surfaceHeight+ CynthalithlithLayerHeight+80  - j) / ((surfaceHeight + RegolithLayerHeight+ 30)-(surfaceHeight + CynthalithlithLayerHeight)*0.5f);
                    if (WorldGen.genRand.NextFloat() > Distance*0.16f)
                    {
                        continue;
                    }
                    WorldGen.TileRunner(i, j, WorldGen.genRand.Next(10,25), WorldGen.genRand.Next(10, 60), cynthalithType);
                }
            }

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int offset = (int)(SurfaceEquation(i * 0.02f + randomOffset) * 3f);
                int surfaceHeight = SurfaceHeight(i);
                for (int j = surfaceHeight + CynthalithlithLayerHeight; j < surfaceHeight + RegolithLayerHeight + 30; j++)
                {
                    if (!Main.tile[i, j].HasTile)
                    {
                        continue;
                    }   
                    if(WorldGen.genRand.NextBool(300))
                    {
                        WorldGen.TileRunner(i, j, WorldGen.genRand.Next(6,12), WorldGen.genRand.Next(6,12), (ushort)TileType<Regolith>());
                    }
                   
                }
            }

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int offset = (int)(SurfaceEquation(i * 0.02f + randomOffset) * 3f);
                int surfaceHeight = SurfaceHeight(i);
                for (int j = surfaceHeight + CynthalithlithLayerHeight+80; j < surfaceHeight + RegolithLayerHeight + 30; j++)
                {
                    if (!Main.tile[i, j].HasTile)
                    {
                        continue;
                    }   
                    if(WorldGen.genRand.NextBool(300))
                    {
                        WorldGen.TileRunner(i, j, WorldGen.genRand.Next(6,12), WorldGen.genRand.Next(6,12), (ushort)TileType<Protolith>());
                    }
                   
                }
            }

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int offset = (int)(SurfaceEquation(i * 0.02f + randomOffset) * 3f);
                int surfaceHeight = SurfaceHeight(i);
                for (int j = surfaceHeight + RegolithLayerHeight + 30; j < surfaceHeight + RegolithLayerHeight + 140; j++)
                {
                    if (!Main.tile[i, j].HasTile)
                    {
                        continue;
                    }   
                    if(WorldGen.genRand.NextBool(300))
                    {
                        WorldGen.TileRunner(i, j, WorldGen.genRand.Next(6,12), WorldGen.genRand.Next(6,12), (ushort)TileType<Cynthalith>());
                    }
                   
                }
            }
        }

        private static int IrradiationEquation(int x, float a, int b)
        {
            static int Equation(int x) => (int)(((10 * Math.Sin(x / 20)) * (Math.Cos(x / 5)) + ((int)(2 * Math.Sin(MathHelper.Pi * x / 80)) ^ 2)) / 1.4);
            return (int)(((a * Equation(x)) + ((x ^ 2) / a) + (Math.Abs(b * x))) / 15);
        }

        //[Task]
        private void IrradiationPass(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.IrradiationPass");
            //So many variables, but hey, this is how I do world gen. It does make it consice.
            ushort rrradiationRockType = (ushort)TileType<IrradiatedRock>();
            ushort irradiationWallType = (ushort)WallType<IrradiatedRockWall>();
            int irradiationCenter = gen_IsIrradiationRight ? (int)(Main.maxTilesX / 2) + WorldGen.genRand.Next(500, 600) : (int)(Main.maxTilesX / 2) - WorldGen.genRand.Next(500, 600);
            int irradiationHeight = 400;
            int irradiationWidth = WorldGen.genRand.Next(240, 260);

            for (int radius, x = 0; x < irradiationHeight; x += 1 + (int)(radius * 0.1f))
            {

                radius = (int)((float)irradiationWidth * (1.5f - (float)x / irradiationHeight));
                int iOffset = irradiationCenter + WorldGen.genRand.NextDirection(10..50);
                int jOffset = (int)Main.worldSurface + x + WorldGen.genRand.NextDirection(20..30);
                ForEachInCircle(
                    iOffset,
                    jOffset,
                    radius,
                    (i1, j1) =>
                    {
                        if (CoordinatesOutOfBounds(i1, j1))
                        {
                            return;
                        }

                        float iDistance = Math.Abs(iOffset - i1) / (radius * 0.5f);
                        float jDistance = Math.Abs(jOffset - j1) / (radius * 0.5f);
                        if (WorldGen.genRand.NextFloat() < iDistance * 0.2f || WorldGen.genRand.NextFloat() < jDistance * 0.2f)
                        {
                            return;
                        }

                        if (Main.tile[i1, j1].WallType != WallID.None)
                        {
                            FastPlaceWall(i1, j1, irradiationWallType);
                        }

                        if (Main.tile[i1, j1].HasTile)
                        {
                            FastPlaceTile(i1, j1, rrradiationRockType);
                        }
                    }
                );
            }

            for (int radius, x = 0; x < (irradiationHeight / 1.5); x += 1 + (int)(radius * 0.1f))
            {

                radius = (int)((float)irradiationWidth * (0.3f - (((float)x / (irradiationHeight)) * 0.3f)));
                int iOffset = irradiationCenter + WorldGen.genRand.NextDirection(10..20);
                int jOffset = (int)Main.worldSurface + x + WorldGen.genRand.NextDirection(20..30);
                ForEachInCircle(
                    iOffset,
                    jOffset,
                    radius,
                    (i1, j1) =>
                    {
                        if (CoordinatesOutOfBounds(i1, j1))
                        {
                            return;
                        }

                        float iDistance = Math.Abs(iOffset - i1) / (radius * 0.5f);
                        float jDistance = Math.Abs(jOffset - j1) / (radius * 0.5f);

                        if (Main.tile[i1, j1].WallType != WallID.None || j1 > CynthalithlithLayerHeight + SurfaceHeight(i1))
                        {
                            FastPlaceWall(i1, j1, irradiationWallType);
                        }

                        if (Main.tile[i1, j1].HasTile)
                        {
                            FastRemoveTile(i1, j1);
                        }
                    }
                );
            }

            int radius2 = (int)((float)irradiationWidth * 0.5f);
            int iOffset2 = irradiationCenter + WorldGen.genRand.NextDirection(2..4);
            int jOffset2 = (int)Main.worldSurface + (int)(irradiationHeight / 1.5) - WorldGen.genRand.NextDirection(20..30);

            //Right facing tunnels
            for (int i = 0; i < WorldGen.genRand.Next(2, 5); i++)
            {
                float a = WorldGen.genRand.NextFloat(1.0f, 20.0f);
                int b = WorldGen.genRand.Next(0, 22);
                int count = 0; 
                for (int j = 0; j < WorldGen.genRand.Next(100, 150); j++)
                {
                    ForEachInCircle(
                            iOffset2 + count,
                            jOffset2 + IrradiationEquation(count, a, b),
                            WorldGen.genRand.Next(5, 8),
                            (i1, j1) =>
                            {
                                if (CoordinatesOutOfBounds(i1, j1))
                                {
                                    return;
                                }

                                float iDistance = Math.Abs(iOffset2 - i1) / (radius2 * 0.5f);
                                float jDistance = Math.Abs(jOffset2 - j1) / (radius2 * 0.5f);
                                FastRemoveTile(i1, j1);
                                FastPlaceWall(i1, j1, irradiationWallType);
                            }
                        );
                    count++;
                }

            }

            for (int i = 0; i < WorldGen.genRand.Next(2, 5); i++)
            {

                float a = WorldGen.genRand.NextFloat(1.0f, 20.0f);
                int b = WorldGen.genRand.Next(0, 22);
                int count = 0; 
                for (int j = 0; j < WorldGen.genRand.Next(100, 150); j++)
                {
                    ForEachInCircle(
                            iOffset2 + count,
                            jOffset2 + IrradiationEquation(count, a, b),
                            WorldGen.genRand.Next(5, 8),
                            (i1, j1) =>
                            {
                                if (CoordinatesOutOfBounds(i1, j1))
                                {
                                    return;
                                }

                                float iDistance = Math.Abs(iOffset2 - i1) / (radius2 * 0.5f);
                                float jDistance = Math.Abs(jOffset2 - j1) / (radius2 * 0.5f);

                                FastRemoveTile(i1, j1);
                                FastPlaceWall(i1, j1, irradiationWallType);
                            }
                        );
                    count--;
                }

            }
        }

        [Task]
        private void OreTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.OrePass");

            int protolithType = TileType<Protolith>();
            GenerateOre(TileType<ArtemiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
            GenerateOre(TileType<ChandriumOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
            GenerateOre(TileType<DianiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
            GenerateOre(TileType<SeleniteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);

            GenerateOre(TileID.LunarOre, 0.00005, WorldGen.genRand.Next(9, 15), WorldGen.genRand.Next(9, 15), protolithType);
        }

        //[Task]
        private void QuartzTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.OrePass");

            int tries = 0;
            int geodes = WorldGen.genRand.Next(10, 14);
            int quartzType = TileType<QuartzBlock>();
            while (geodes > 1 && tries < 100)
            {
                while (geodes > 1 && tries < 100)
                {
                    int i = WorldGen.genRand.Next(70, Main.maxTilesX - 70);
                    int j = WorldGen.genRand.Next((int)(Main.maxTilesY / 3), Main.maxTilesY - 70);
                    Tile tile = Main.tile[i, j];
                    if (tile.HasTile)
                    {
                        int iOffset2 = i;
                        int jOffset2 = j;
                        int radius = WorldGen.genRand.Next(40, 46);

                        int radius2 = WorldGen.genRand.Next(30, 36);
                        ForEachInCircle(
                            iOffset2,
                            jOffset2,
                            radius2,
                            (i1, j1) =>
                            {
                                if (CoordinatesOutOfBounds(i1, j1))
                                {
                                    return;
                                }

                                float iDistance = Math.Abs(iOffset2 - i) / (radius2 * 0.5f);
                                float jDistance = Math.Abs(jOffset2 - j) / (radius2 * 0.5f);

                                if (Main.tile[i1, j1].HasTile)
                                {
                                    FastRemoveTile(i1, j1);
                                }

                            }
                        );

                        ForEachInCircle(
                            iOffset2,
                            jOffset2,
                            radius,
                            (i1, j1) =>
                            {
                                if (CoordinatesOutOfBounds(i1, j1))
                                {
                                    return;
                                }
                                
                                float iDistance = Math.Abs(iOffset2 - i) / (radius * 0.5f);
                                float jDistance = Math.Abs(jOffset2 - j) / (radius * 0.5f);

                                if (WorldGen.genRand.NextFloat() < 0.96f)
                                {
                                    return;
                                }
                                if (!Main.tile[i1, j1].HasTile)
                                {
                                    return;
                                }
                                
                               TileRunnerButItDoesntIgnoreAir(i1, j1, WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 10), (ushort)quartzType);
                                

            
                            }
                        );
                        ForEachInCircle(
                            iOffset2,
                            jOffset2,
                            radius,
                            (i1, j1) =>
                            {
                                if (CoordinatesOutOfBounds(i1, j1))
                                {
                                    return;
                                }
                                
                                float iDistance = Math.Abs(iOffset2 - i) / (radius * 0.5f);
                                float jDistance = Math.Abs(jOffset2 - j) / (radius * 0.5f);

                                if (WorldGen.genRand.NextFloat() < 0.2f)
                                {
                                    return;
                                }
                              

                                if (Main.tile[i1, j1].HasTile)
                                {
                                    FastPlaceTile(i1, j1, (ushort)quartzType);
                                }
                            }
                        );
                        geodes--;
                    }
                    else
                    {
                        tries++;
                    }
                }

            }
        }

        [Task]
        private void SmoothTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.SmoothPass");
            SmoothWorld(progress);
        }

        [Task]
        private void PlaceLuminiteShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new LuminiteShrine();
            bool solidDown = WorldUtils.Find(gen_LuminiteShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(solidGround.X - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y * 0.8f)), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
            gen_LuminiteShrinePosition =new Point(fallbackX, fallbackY); 
        }

        [Task]
        private void PlaceHeavenforgeShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            StructureSH shrine = new HeavenforgeShrine();
            bool solidDown = WorldUtils.Find(gen_HeavenforgeShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(solidGround.X - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y * 0.8f)), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
            gen_HeavenforgeShrinePosition=new Point(fallbackX, fallbackY); 
        }

        [Task]
        private void PlaceMercuryShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            StructureSH shrine = new MercuryShrine();
            bool solidUp = WorldUtils.Find(gen_MercuryShrinePosition, Searches.Chain(new Searches.Up(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidUp && shrine.Place(new Point16(solidGround.X + shrine.Size.X / 2 - 1, solidGround.Y - 10), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
            gen_MercuryShrinePosition=new Point(fallbackX, fallbackY); 
        }

        [Task]
        private void PlaceLunarRustShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            StructureSH shrine = new LunarRustShrine();
            bool solidUp = WorldUtils.Find(gen_LunarRustShrinePosition, Searches.Chain(new Searches.Up(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidUp && shrine.Place(new Point16(gen_LunarRustShrinePosition.X + shrine.Size.X / 4, solidGround.Y - 10), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
            gen_LunarRustShrinePosition=new Point(fallbackX, fallbackY); 
        }

        [Task]
        private void PlaceStarRoyaleShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            StructureSH shrine = new StarRoyaleShrine();
            bool solidDown = WorldUtils.Find(gen_StarRoyaleShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(gen_StarRoyaleShrinePosition.X - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y * 1.1f)), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
            gen_StarRoyaleShrinePosition=new Point(fallbackX, fallbackY); 
        }

        [Task]
        private void PlaceCryocoreShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            StructureSH shrine = new CryocoreShrine();
            bool solidDown = WorldUtils.Find(gen_CryocoreShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(gen_CryocoreShrinePosition.X - shrine.Size.X / 2, solidGround.Y - shrine.Size.Y - (int)(shrine.Size.Y * 0.2f)), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
            gen_CryocoreShrinePosition=new Point(fallbackX, fallbackY); 
        }

        [Task]
        private void PlaceAstraShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            StructureSH shrine = new AstraShrine();
            bool solidDown = WorldUtils.Find(gen_AstraShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(gen_AstraShrinePosition.X - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y)), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
            gen_AstraShrinePosition=new Point(fallbackX, fallbackY); 
        }

        [Task]
        private void PlaceDarkCelestialShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            StructureSH shrine = new DarkCelestialShrine();
            bool solidDown = WorldUtils.Find(gen_DarkCelestialShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(gen_DarkCelestialShrinePosition.X - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y * 0.9f)), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
            gen_DarkCelestialShrinePosition=new Point(fallbackX, fallbackY); 
        }

        [Task]
        private void PlaceCosmicEmberShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            StructureSH shrine = new CosmicEmberShrine();
            bool solidDown = WorldUtils.Find(gen_CosmicEmberShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(gen_CosmicEmberShrinePosition.X - shrine.Size.X / 2, solidGround.Y - shrine.Size.Y), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
            gen_CosmicEmberShrinePosition=new Point(fallbackX, fallbackY); 
        }


        private bool IsVaildForHousePlacment(int x, int y)
        {
            Point housePoint = new(x,y);
            Vector2 housePosition =housePoint.ToWorldCoordinates();

            if(Vector2.Distance(housePosition, gen_LuminiteShrinePosition.ToWorldCoordinates()) < 1500f)
                return false;
            if(Vector2.Distance(housePosition, gen_HeavenforgeShrinePosition.ToWorldCoordinates()) < 1500f)
                return false;
            if(Vector2.Distance(housePosition, gen_MercuryShrinePosition.ToWorldCoordinates()) < 1500f)
                return false;
            if(Vector2.Distance(housePosition, gen_LunarRustShrinePosition.ToWorldCoordinates()) < 1500f)
                return false;
            if(Vector2.Distance(housePosition, gen_StarRoyaleShrinePosition.ToWorldCoordinates()) < 1500f)
                return false;
            if(Vector2.Distance(housePosition, gen_CryocoreShrinePosition.ToWorldCoordinates()) < 1500f)
                return false;
            if(Vector2.Distance(housePosition, gen_AstraShrinePosition.ToWorldCoordinates()) < 1500f)
                return false;
            if(Vector2.Distance(housePosition, gen_DarkCelestialShrinePosition.ToWorldCoordinates()) < 1500f)
                return false;
            if(Vector2.Distance(housePosition, gen_CosmicEmberShrinePosition.ToWorldCoordinates()) < 1500f)
                return false;
            return true;
        }


        [Task]
        private void RoomsTasks(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            int tries = 10000;
            int count = WorldGen.genRand.Next(10, 30);
            for (int i = 0; i < count; i++)
            {
                if (tries <= 0)
                    break;

                progress.Set((double)i / count);
                int tileX = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
                int tileY = WorldGen.genRand.Next((int)(SurfaceHeight(tileX) + RegolithLayerHeight + 20.0), Main.maxTilesY - 230);

                var builder = DetermineLunarHouse();
                if(IsVaildForHousePlacment(tileX, tileY)){
                if (!builder.Place(new(tileX, tileY), StructureMap))
                {
                    tries--;
                    i--;
                }
                }
                
            }
        }

        [Task]
        private void CheeseHouse(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.Horror");
            StructureSH cheeseHouse = new CheeseHouse();
            cheeseHouse.Place(new Point16(420, 1000), StructureMap);
        }

        [Task]
        private void AmbientTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.AmbientPass");

            float smallRockSpawnChance = 0.1f;
            float mediumRockSpawnChance = 0.05f;
            float largeRockSpawnChance = 0.01f;
            float altarChance = 0.01f;
            float kyaniteNestChance = 0.0045f;
            float chestChance = 0.0065f;

            ushort regolithType = (ushort)TileType<Regolith>();
            ushort protolithType = (ushort)TileType<Protolith>();
            ushort irradiatedRockType = (ushort)TileType<IrradiatedRock>();
            ushort quartzType = (ushort)TileType<QuartzBlock>();

            for (int i = 0; i < Main.maxTilesX - 1; i++)
            {
                progress.Set((float)i / Main.maxTilesX);

                for (int j = 1; j < Main.maxTilesY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (WorldGen.genRand.NextFloat() < chestChance && tile.HasTile && Main.tileSolid[tile.TileType] && !tile.IsActuated)
                    {
                        if (tile.TileType == protolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<LuminiteChest>(), mute: true);
                    }

                    if (WorldGen.genRand.NextFloat() < smallRockSpawnChance && tile.HasTile && Main.tileSolid[tile.TileType] && !tile.IsActuated)
                    {
                        if (tile.TileType == regolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<RegolithRockSmallNatural>(), style: WorldGen.genRand.Next(10), mute: true);

                        if (tile.TileType == protolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<ProtolithRockSmallNatural>(), style: WorldGen.genRand.Next(10), mute: true);
                    }

                    if (WorldGen.genRand.NextFloat() < mediumRockSpawnChance && CheckEmptyAboveWithSolidToTheRight(i, j, 2, 1))
                    {
                        if (tile.TileType == regolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<RegolithRockMediumNatural>(), style: WorldGen.genRand.Next(6), mute: true);

                        if (tile.TileType == protolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<ProtolithRockMediumNatural>(), style: WorldGen.genRand.Next(6), mute: true);
                    }

                    if (WorldGen.genRand.NextFloat() < largeRockSpawnChance && CheckEmptyAboveWithSolidToTheRight(i, j, 3, 2))
                    {
                        if (tile.TileType == regolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<RegolithRockLargeNatural>(), style: WorldGen.genRand.Next(5), mute: true);

                        if (tile.TileType == protolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<ProtolithRockLargeNatural>(), style: WorldGen.genRand.Next(5), mute: true);
                    }

                    if (WorldGen.genRand.NextFloat() < altarChance && CheckEmptyAboveWithSolidToTheRight(i, j, 3, 2))
                    {
                        if (tile.TileType == protolithType || tile.TileType == irradiatedRockType)
                        {
                            WorldGen.PlaceTile(i, j - 1, TileType<IrradiatedAltar>(), mute: true);
                            int iOffset2 = i;
                            int jOffset2 = j;
                            int radius = WorldGen.genRand.Next(10, 16);
                            ForEachInCircle(
                                    iOffset2,
                                    jOffset2,
                                    radius,
                                    (i1, j1) =>
                                    {
                                        if (Main.tile[i, j - 1].TileType != TileType<IrradiatedAltar>())
                                        {
                                            return;
                                        }
                                        if (CoordinatesOutOfBounds(i1, j1))
                                        {
                                            return;
                                        }

                                        float iDistance = Math.Abs(iOffset2 - i) / (radius * 0.5f);
                                        float jDistance = Math.Abs(jOffset2 - j) / (radius * 0.5f);

                                        if (WorldGen.genRand.NextFloat() < 0.5f)
                                        {
                                            return;
                                        }

                                        if (Main.tile[i1, j1].HasTile && TileID.Sets.CanBeClearedDuringOreRunner[Main.tile[i1, j1].TileType] && !(Main.tile[i1, j1].TileType == TileType<IrradiatedAltar>()))
                                        {
                                            FastPlaceTile(i1, j1, (ushort)TileType<IrradiatedRock>());
                                        }
                                    }
                                );
                        }
                    }

                    if (WorldGen.genRand.NextFloat() < kyaniteNestChance && CheckEmptyAboveWithSolidToTheRight(i, j, 4, 3))
                    {
                        if (tile.TileType == protolithType)
                        {
                            WorldGen.PlaceTile(i, j - 1, TileType<KyaniteNest>(), mute: true);
                        }
                    }
                }
            }
        }

        [Task]
        private void MonolithTask(GenerationProgress progress)
        {
            int tries = 0;
            bool placed = false;
            while (tries < 1000)
            {
                int tileX = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
                int tileY = WorldGen.genRand.Next((int)(SurfaceHeight(tileX) + RegolithLayerHeight + 20.0), Main.maxTilesY - 230);
                if (CheckEmptyAboveWithSolidToTheRight(tileX, tileY, 4, 8))
                {
                    if (WorldGen.PlaceTile(tileX, tileY - 1, TileType<Monolith>(), mute: true))
                    {
                        placed = true;
                        break;
                    }
                }
            }

            if (!placed)
            {
                int tileX = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
                int tileY = WorldGen.genRand.Next((int)(SurfaceHeight(tileX) + RegolithLayerHeight + 20.0), Main.maxTilesY - 230);
                WorldGen.PlaceTile(tileX, tileY - 1, TileType<Monolith>(), mute: true, forced: true);
            }
        }


        [Task]
        private void SpawnTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.SpawnPass");
            int spawnTileX = Main.maxTilesX / 2;
            Main.spawnTileX = spawnTileX;
            for (int tileY = 0; tileY < Main.maxTilesY; tileY++)
            {
                if (Main.tile[spawnTileX, tileY].HasTile)
                {
                    Main.spawnTileY = tileY - 2;
                    break;
                }
            }

            Main.dayTime = true;
            Main.time = 0;
        }
        //This will be MUCH easier when uncon does the shrine redesgins and uses the luminite chests
        private void ManageChest(Chest chest,int type){
            int slot =0;
            switch (type)
            {
                case 1:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Procellarum>());
                    chest.item[slot++].stack = 1;
                    break;
                case 2:
                    chest.item[slot].SetDefaults(ModContent.ItemType<ManisolBlades>());
                    chest.item[slot++].stack = 1;
                    break;
                case 3:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Ilmenite>());
                    chest.item[slot++].stack = 1;
                    break;
                case 4:
                    chest.item[slot].SetDefaults(ModContent.ItemType<StarDestroyer>());
                    chest.item[slot++].stack = 1;
                    break;
                case 5:
                    chest.item[slot].SetDefaults(ModContent.ItemType<FrigorianGaze>());
                    chest.item[slot++].stack = 1;
                    break;
                case 6:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Micronova>());
                    chest.item[slot++].stack = 1;
                    break;
                case 7:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Totality>());
                    chest.item[slot++].stack = 1;
                    break;
                case 8:
                    chest.item[slot].SetDefaults(ModContent.ItemType<GreatstaffOfHorus>());
                    chest.item[slot++].stack = 1;
                    break; 
            }
            if (type == 1 || type == 2){
            chest.item[slot].SetDefaults(ModContent.ItemType<ArtemiteBar>());
            chest.item[slot++].stack = WorldGen.genRand.Next(10,15);
            chest.item[slot].SetDefaults(ItemID.FragmentSolar);
            chest.item[slot++].stack = WorldGen.genRand.Next(10,15);
            }

            if (type == 3 || type == 4){
            chest.item[slot].SetDefaults(ModContent.ItemType<SeleniteBar>());
            chest.item[slot++].stack = WorldGen.genRand.Next(10,15);
            chest.item[slot].SetDefaults(ItemID.FragmentVortex);
            chest.item[slot++].stack = WorldGen.genRand.Next(10,15);
            }

            if (type == 5 || type == 6){
            chest.item[slot].SetDefaults(ModContent.ItemType<DianiteBar>());
            chest.item[slot++].stack = WorldGen.genRand.Next(10,15);
            chest.item[slot].SetDefaults(ItemID.FragmentNebula);
            chest.item[slot++].stack = WorldGen.genRand.Next(10,15);
            }

            if (type == 7 || type == 8){
            chest.item[slot].SetDefaults(ModContent.ItemType<ChandriumBar>());
            chest.item[slot++].stack = WorldGen.genRand.Next(10,15);
            chest.item[slot].SetDefaults(ItemID.FragmentStardust);
            chest.item[slot++].stack = WorldGen.genRand.Next(10,15);
            }
            chest.item[slot].SetDefaults(ModContent.ItemType<LunarCrystal>());
            chest.item[slot++].stack = WorldGen.genRand.Next(30,60);
            chest.item[slot].SetDefaults(ModContent.ItemType<Moonstone>());
            chest.item[slot++].stack = WorldGen.genRand.Next(30,60);

            Chest.Lock(chest.x,chest.y);
        }
        public void ManageLuminiteChest(Chest chest){
            int slot =0;
            int random1 = WorldGen.genRand.Next(1,11);
            switch (random1)
            {
                case 1:
                    chest.item[slot].SetDefaults(ModContent.ItemType<RyuguStaff>());
                    chest.item[slot++].stack = 1;
                    break;
                case 2:
                    chest.item[slot].SetDefaults(ModContent.ItemType<ArmstrongGauntlets>());
                    chest.item[slot++].stack = 1;
                    break;
                case 3:
                    chest.item[slot].SetDefaults(ModContent.ItemType<WornLunarianDagger>());
                    chest.item[slot++].stack = 1;
                    break;
                case 4:
                    chest.item[slot].SetDefaults(ModContent.ItemType<RocheChakram>());
                    chest.item[slot++].stack = 1;
                    break;
                case 5:
                    chest.item[slot].SetDefaults(ModContent.ItemType<ArcaneBarnacle>());
                    chest.item[slot++].stack = 1;
                    break;
                case 6:
                    chest.item[slot].SetDefaults(ModContent.ItemType<MomentumLash>());
                    chest.item[slot++].stack = 1;
                    break;
                case 7:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Consumables.BossSummons.CraterDemonSummon>());
                    chest.item[slot++].stack = 1;
                    break;
                case 8:
                    chest.item[slot].SetDefaults(ModContent.ItemType<TempestuousBand>());
                    chest.item[slot++].stack = 1;
                    break;
                case 9:
                    chest.item[slot].SetDefaults(ModContent.ItemType<ThaumaturgicWard>());
                    chest.item[slot++].stack = 1;
                    break;
                case 10:
                    chest.item[slot].SetDefaults(ModContent.ItemType<CrescentMoon>());
                    chest.item[slot++].stack = 1;
                    break;  
            }
            
            int random3 = WorldGen.genRand.Next(1,6);
            switch (random3)
            {
                case 1:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Ores.ArtemiteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;
                case 2:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Ores.ChandriumOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;
                case 3:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Ores.DianiteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;
                case 4:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Ores.SeleniteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;
                case 5:
                    chest.item[slot].SetDefaults(ItemID.LunarOre);
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;
            }
            
            chest.item[slot].SetDefaults(ModContent.ItemType<LunarCrystal>());
                chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);
            chest.item[slot].SetDefaults(ModContent.ItemType<Moonstone>());
                chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);
        }


        public void ManageIndustrialChest(Chest chest){
            int slot =0;
            int random1 = WorldGen.genRand.Next(1,9);
            switch (random1)
            {
                case 1:
                    chest.item[slot].SetDefaults(ModContent.ItemType<ClawWrench>());
                    chest.item[slot++].stack = 1;
                    break;
                case 2:
                    chest.item[slot].SetDefaults(ModContent.ItemType<WaveGunRed>());
                    chest.item[slot++].stack = 1;
                    break;
                case 3:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Copernicus>());
                    chest.item[slot++].stack = 1;
                    break;
                case 4:
                    chest.item[slot].SetDefaults(ModContent.ItemType<HummingbirdDroneRemote>());
                    chest.item[slot++].stack = 1;
                    break; 
                case 5:
                    chest.item[slot].SetDefaults(ModContent.ItemType<OsmiumBoots>());
                    chest.item[slot++].stack = 1;
                    break;
                case 6:
                    chest.item[slot].SetDefaults(ModContent.ItemType<StalwartTowerShield>());
                    chest.item[slot++].stack = 1;
                    break;
                case 7:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Tools.Sledgehammer>());
                    chest.item[slot++].stack = 1;
                    break;
                case 8:
                    chest.item[slot].SetDefaults(ModContent.ItemType<LaserSight>());
                    chest.item[slot++].stack = 1;
                    break;
                
            }
            for(int i =0; i<3; i++){
                int random2 = WorldGen.genRand.Next(1,10);
            switch (random2)
            {
                case 1:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Plastic>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 2:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Ores.NickelOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 3:
                    chest.item[slot].SetDefaults(ItemID.LunarOre);
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 4:
                    chest.item[slot].SetDefaults(ModContent.ItemType<SpaceDust>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 5:
                    chest.item[slot].SetDefaults(ModContent.ItemType<AlienResidue>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 6:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Ores.LithiumOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 7:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Ores.AluminumOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break;
                case 8:
                    chest.item[slot].SetDefaults(ModContent.ItemType<SteelBar>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break; 
                case 9:
                    chest.item[slot].SetDefaults(ModContent.ItemType<RocketFuelCanister>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                    break; 
            }
            }
            int random3 = WorldGen.genRand.Next(1,5);
            switch (random3)
            {
                case 1:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Ores.ArtemiteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;
                case 2:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Ores.ChandriumOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;
                case 3:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Ores.DianiteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;
                case 4:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Ores.SeleniteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;
                
            }
            for(int i =0; i<2; i++){
            int random4 = WorldGen.genRand.Next(1,5);
            switch (random4)
            {
                case 1:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Blocks.IndustrialPlating>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                    break;
                case 2:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Blocks.Terrain.Protolith>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                    break;
                case 3:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Blocks.Terrain.Regolith>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                    break;
                case 4:
                    chest.item[slot].SetDefaults(ModContent.ItemType<Items.Blocks.Terrain.Cynthalith>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                    break;
                
            }
            }
            chest.item[slot].SetDefaults(ModContent.ItemType<LunarCrystal>());
                chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);
            chest.item[slot].SetDefaults(ModContent.ItemType<Moonstone>());
                chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);
        }

        [Task]
        private void ChestLootTask(GenerationProgress progress)
        {
            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest chest = Main.chest[i];
                if (chest != null){

                if(Main.tile[chest.x,chest.y].TileType==TileType<Tiles.Furniture.Industrial.IndustrialChest>())
                {
                    ManageIndustrialChest(chest);
                }
                if(Main.tile[chest.x,chest.y].TileType==TileType<Tiles.Furniture.Luminite.LuminiteChest>()&&Main.tile[chest.x, chest.y].TileFrameX ==0 * 36)
                {
                    ManageLuminiteChest(chest);
                }


                

                if(Main.tile[chest.x,chest.y].TileType==TileType<Tiles.Furniture.Luminite.LuminiteChest>()&&Main.tile[chest.x, chest.y].TileFrameX ==1 * 36)//Heavenforge
                {
                    ManageChest(chest,1);
    
                }
                if(Main.tile[chest.x,chest.y].TileType==TileType<Tiles.Furniture.Luminite.LuminiteChest>()&&Main.tile[chest.x, chest.y].TileFrameX ==5 * 36)//Mercury
                {
                    ManageChest(chest,2);
     
                }
                if(Main.tile[chest.x,chest.y].TileType==TileType<Tiles.Furniture.Luminite.LuminiteChest>()&&Main.tile[chest.x, chest.y].TileFrameX ==2 * 36)//LunarRust
                {
                    ManageChest(chest,3);
         
                }
                if(Main.tile[chest.x,chest.y].TileType==TileType<Tiles.Furniture.Luminite.LuminiteChest>()&&Main.tile[chest.x, chest.y].TileFrameX ==6 * 36)//Starroalye
                {
                    ManageChest(chest,4);
        
                }
                if(Main.tile[chest.x,chest.y].TileType==TileType<Tiles.Furniture.Luminite.LuminiteChest>()&&Main.tile[chest.x, chest.y].TileFrameX ==7 * 36)//Cryocore
                {
                    ManageChest(chest,5);
    
                }
                if(Main.tile[chest.x,chest.y].TileType==TileType<Tiles.Furniture.Luminite.LuminiteChest>()&&Main.tile[chest.x, chest.y].TileFrameX ==3 * 36)//Astra
                {
                    ManageChest(chest,6);
     
                }
                if(Main.tile[chest.x,chest.y].TileType==TileType<Tiles.Furniture.Luminite.LuminiteChest>()&&Main.tile[chest.x, chest.y].TileFrameX ==4 * 36)//Dark Celestial
                {
                    ManageChest(chest,7);
         
                }
                if(Main.tile[chest.x,chest.y].TileType==TileType<Tiles.Furniture.Luminite.LuminiteChest>()&&Main.tile[chest.x, chest.y].TileFrameX ==8 * 36)//Cosmic ember
                {
                    ManageChest(chest,8);
        
                }

                }
            }
        }
    }
}