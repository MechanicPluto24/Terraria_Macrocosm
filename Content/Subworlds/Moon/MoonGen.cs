using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Macrocosm.Content.Tiles;
using SubworldLibrary;
using System;
using Terraria.ID;

namespace Macrocosm.Content.Subworlds.Moon
{
    public class MoonGen : List<GenPass>
    {
        private Subworld subworld;
        private double surfaceLayer = 200.0;
        private double rockLayerLow = 0.0;
        private double rockLayerHigh = 0.0;
        private void CraterPass(GenerationProgress progress)
        {
            progress.Message = "Sculpting the Moon...";
            for (int craterPass = 0; craterPass < 2; craterPass++)
            {
                int lastMaxTileX = 0;
                int lastXRadius = 0;
                int craterDenominatorChance = 20;
                switch (craterPass)
                {
                    case 0:
                        craterDenominatorChance = 5;
                        break;
                    case 1:
                        craterDenominatorChance = 20;
                        break;
                    default:
                        craterDenominatorChance = 30;
                        break;
                }


                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    progress.Set((i / (float)Main.maxTilesX - 1));
                    // The moon has craters... Therefore, we gotta make some flat ellipses in the world gen code!
                    if (WorldGen.genRand.Next(0, craterDenominatorChance) == 0 && i > lastMaxTileX + lastXRadius)
                    {
                        int craterJPosition = 0;
                        // Look for a Y position to put the crater
                        for (int lookupY = 0; lookupY < Main.maxTilesY; lookupY++)
                        {
                            if (Framing.GetTileSafely(i, lookupY).active())
                            {
                                craterJPosition = lookupY;
                                break;
                            }
                        }
                        // Create random-sized boxes in which our craters will be carved
                        int radiusX;
                        int radiusY;
                        // Two passes for two different sizes of craters
                        // (That's why the moon raggedy)
                        // We could add more to roughen up the terrain more, but the terrain looks fine with two
                        switch (craterPass)
                        {
                            // This is for the big, main craters
                            case 0:
                                radiusX = WorldGen.genRand.Next(8, 55);
                                radiusY = WorldGen.genRand.Next(4, 10);
                                break;
                            case 1:
                                radiusX = WorldGen.genRand.Next(8, 15);
                                radiusY = WorldGen.genRand.Next(2, 4);
                                break;
                            default:
                                radiusX = 0;
                                radiusY = 0;
                                break;
                        }

                        int minTileX = i - radiusX;
                        int maxTileX = i + radiusX;
                        int minTileY = craterJPosition - radiusY;
                        int maxTileY = craterJPosition + radiusY;

                        // Calculate diameter and center of ellipse based on the boundaries specified
                        int diameterX = Math.Abs(minTileX - maxTileX);
                        int diameterY = Math.Abs(minTileY - maxTileY);
                        float centerX = (minTileX + maxTileX - 1) / 2f;
                        float centerY = (minTileY + maxTileY - 1) / 2f;

                        // Make the crater
                        for (int craterTileX = minTileX; craterTileX < maxTileX; craterTileX++)
                        {
                            for (int craterTileY = minTileY; craterTileY < maxTileY; craterTileY++)
                            {
                                // This is the equation for the unit ellipse; we're dividing by squares of the diameters to scale along the axes
                                if
                                (
                                    (
                                        Math.Pow(craterTileX - centerX, 2) / Math.Pow(diameterX / 2, 2))
                                        + (Math.Pow(craterTileY - centerY, 2) / Math.Pow(diameterY / 2, 2)
                                    ) <= 1
                                )
                                {
                                    if (craterTileX < Main.maxTilesX && craterTileY < Main.maxTilesY && craterTileX >= 0 && craterTileY >= 0)
                                    {
                                        Main.tile[craterTileX, craterTileY].active(false);
                                    }
                                }
                            }
                            // We're going to remove a chunk of tiles 
                            // above the craters to prevent weird overhangs which clearly do not appear on the moon in real-life
                            // It'll extend from the halfway mark of the ellipse to twenty tiles above the minTileY
                            // Before: http://prntscr.com/toyj7u
                            // After: http://prntscr.com/toylfa
                            for (int craterTileY = minTileY - 20; craterTileY < maxTileY - diameterY / 2; craterTileY++)
                            {
                                if (craterTileX < Main.maxTilesX && craterTileY < Main.maxTilesY && craterTileX >= 0 && craterTileY >= 0)
                                {
                                    Main.tile[craterTileX, craterTileY].active(false);
                                }
                            }
                        }
                        lastMaxTileX = maxTileX;
                        lastXRadius = diameterX / 2;
                    }
                }
            }
        }
        private void OrePass(GenerationProgress progress)
        {
            void GenerateOre(int type, double percent, int strength, int steps)
            {
                for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * percent); k++)
                {
                    int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                    int y = WorldGen.genRand.Next(0, Main.maxTilesY);
                    if (Main.tile[x, y].active() && Main.tile[x, y].type == ModContent.TileType<Tiles.Protolith>())
                    {
                        WorldGen.TileRunner(x, y, strength, steps, type);
                    }
                }
            }
            progress.Message = "Mineralizing the Moon...";
            #region Generate ore veins
            GenerateOre(ModContent.TileType<ArtemiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9));
            GenerateOre(ModContent.TileType<ChandriumOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9));
            GenerateOre(ModContent.TileType<DianiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9));
            GenerateOre(ModContent.TileType<SeleniteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9));
            #endregion
        }
        private void BackgroundPass(GenerationProgress progress)
        {
            // This generates before lunar caves so that there are overhangs
            progress.Message = "Backgrounding the Moon...";
            for (int tileX = 1; tileX < Main.maxTilesX - 1; tileX++)
            {
                int wall = 2;
                float progressPercent = tileX / Main.maxTilesX;
                progress.Set(progressPercent);
                bool surroundedTile = false;
                for (int tileY = 2; tileY < Main.maxTilesY - 1; tileY++)
                {
                    if (Main.tile[tileX, tileY].active())
                        wall = ModContent.WallType<Walls.RegolithWall>();

                    if (surroundedTile)
                        Main.tile[tileX, tileY].wall = (ushort)wall;

                    if
                    (
                        Main.tile[tileX, tileY].active() // Current tile is active
                        && Main.tile[tileX - 1, tileY].active() // Left tile is active
                        && Main.tile[tileX + 1, tileY].active() // Right tile is active
                        && Main.tile[tileX, tileY + 1].active() // Bottom tile is active
                        && Main.tile[tileX - 1, tileY + 1].active() // Bottom-left tile is active
                        && Main.tile[tileX + 1, tileY + 1].active() // Bottom-right tile is active
                                                                    // The following will help to make the walls slightly lower than the terrain
                        && Main.tile[tileX, tileY - 2].active() // Top tile is active
                    )
                    {
                        surroundedTile = true; // Set the rest of the walls down the column
                    }
                }
            }
        }
        private void ScuffedSmoothPass(GenerationProgress progress)
        {
            progress.Message = "Smoothening the Moon...";
            // WARNING x WARNING x WARNING
            // Heavily nested code copied from decompiled code
            for (int tileX = 20; tileX < Main.maxTilesX - 20; tileX++)
            {
                float percentAcrossWorld = (float)tileX / (float)Main.maxTilesX;
                progress.Set(percentAcrossWorld);
                for (int tileY = 20; tileY < Main.maxTilesY - 20; tileY++)
                {
                    if (Main.tile[tileX, tileY].type != 48 && Main.tile[tileX, tileY].type != 137 && Main.tile[tileX, tileY].type != 232 && Main.tile[tileX, tileY].type != 191 && Main.tile[tileX, tileY].type != 151 && Main.tile[tileX, tileY].type != 274)
                    {
                        if (!Main.tile[tileX, tileY - 1].active())
                        {
                            if (WorldGen.SolidTile(tileX, tileY) && TileID.Sets.CanBeClearedDuringGeneration[Main.tile[tileX, tileY].type])
                            {
                                if (!Main.tile[tileX - 1, tileY].halfBrick() && !Main.tile[tileX + 1, tileY].halfBrick() && Main.tile[tileX - 1, tileY].slope() == 0 && Main.tile[tileX + 1, tileY].slope() == 0)
                                {
                                    if (WorldGen.SolidTile(tileX, tileY + 1))
                                    {
                                        if (!WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX - 1, tileY + 1].halfBrick() && WorldGen.SolidTile(tileX - 1, tileY + 1) && WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX + 1, tileY - 1].active())
                                        {
                                            if (WorldGen.genRand.Next(2) == 0)
                                                WorldGen.SlopeTile(tileX, tileY, 2);
                                            else
                                                WorldGen.PoundTile(tileX, tileY);
                                        }
                                        else if (!WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX + 1, tileY + 1].halfBrick() && WorldGen.SolidTile(tileX + 1, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX - 1, tileY - 1].active())
                                        {
                                            if (WorldGen.genRand.Next(2) == 0)
                                                WorldGen.SlopeTile(tileX, tileY, 1);
                                            else
                                                WorldGen.PoundTile(tileX, tileY);
                                        }
                                        else if (WorldGen.SolidTile(tileX + 1, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY + 1) && !Main.tile[tileX + 1, tileY].active() && !Main.tile[tileX - 1, tileY].active())
                                        {
                                            WorldGen.PoundTile(tileX, tileY);
                                        }

                                        if (WorldGen.SolidTile(tileX, tileY))
                                        {
                                            if (WorldGen.SolidTile(tileX - 1, tileY) && WorldGen.SolidTile(tileX + 1, tileY + 2) && !Main.tile[tileX + 1, tileY].active() && !Main.tile[tileX + 1, tileY + 1].active() && !Main.tile[tileX - 1, tileY - 1].active())
                                            {
                                                WorldGen.KillTile(tileX, tileY);
                                            }
                                            else if (WorldGen.SolidTile(tileX + 1, tileY) && WorldGen.SolidTile(tileX - 1, tileY + 2) && !Main.tile[tileX - 1, tileY].active() && !Main.tile[tileX - 1, tileY + 1].active() && !Main.tile[tileX + 1, tileY - 1].active())
                                            {
                                                WorldGen.KillTile(tileX, tileY);
                                            }
                                            else if (!Main.tile[tileX - 1, tileY + 1].active() && !Main.tile[tileX - 1, tileY].active() && WorldGen.SolidTile(tileX + 1, tileY) && WorldGen.SolidTile(tileX, tileY + 2))
                                            {
                                                if (WorldGen.genRand.Next(5) == 0)
                                                    WorldGen.KillTile(tileX, tileY);
                                                else if (WorldGen.genRand.Next(5) == 0)
                                                    WorldGen.PoundTile(tileX, tileY);
                                                else
                                                    WorldGen.SlopeTile(tileX, tileY, 2);
                                            }
                                            else if (!Main.tile[tileX + 1, tileY + 1].active() && !Main.tile[tileX + 1, tileY].active() && WorldGen.SolidTile(tileX - 1, tileY) && WorldGen.SolidTile(tileX, tileY + 2))
                                            {
                                                if (WorldGen.genRand.Next(5) == 0)
                                                    WorldGen.KillTile(tileX, tileY);
                                                else if (WorldGen.genRand.Next(5) == 0)
                                                    WorldGen.PoundTile(tileX, tileY);
                                                else
                                                    WorldGen.SlopeTile(tileX, tileY, 1);
                                            }
                                        }
                                    }

                                    if (WorldGen.SolidTile(tileX, tileY) && !Main.tile[tileX - 1, tileY].active() && !Main.tile[tileX + 1, tileY].active())
                                        WorldGen.KillTile(tileX, tileY);
                                }
                            }
                            else if (!Main.tile[tileX, tileY].active() && Main.tile[tileX, tileY + 1].type != 151 && Main.tile[tileX, tileY + 1].type != 274)
                            {
                                if (Main.tile[tileX + 1, tileY].type != 190 && Main.tile[tileX + 1, tileY].type != 48 && Main.tile[tileX + 1, tileY].type != 232 && WorldGen.SolidTile(tileX - 1, tileY + 1) && WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX - 1, tileY].active() && !Main.tile[tileX + 1, tileY - 1].active())
                                {
                                    WorldGen.PlaceTile(tileX, tileY, Main.tile[tileX, tileY + 1].type);
                                    if (WorldGen.genRand.Next(2) == 0)
                                        WorldGen.SlopeTile(tileX, tileY, 2);
                                    else
                                        WorldGen.PoundTile(tileX, tileY);
                                }

                                if (Main.tile[tileX - 1, tileY].type != 190 && Main.tile[tileX - 1, tileY].type != 48 && Main.tile[tileX - 1, tileY].type != 232 && WorldGen.SolidTile(tileX + 1, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX + 1, tileY].active() && !Main.tile[tileX - 1, tileY - 1].active())
                                {
                                    WorldGen.PlaceTile(tileX, tileY, Main.tile[tileX, tileY + 1].type);
                                    if (WorldGen.genRand.Next(2) == 0)
                                        WorldGen.SlopeTile(tileX, tileY, 1);
                                    else
                                        WorldGen.PoundTile(tileX, tileY);
                                }
                            }
                        }
                        else if (!Main.tile[tileX, tileY + 1].active() && WorldGen.genRand.Next(2) == 0 && WorldGen.SolidTile(tileX, tileY) && !Main.tile[tileX - 1, tileY].halfBrick() && !Main.tile[tileX + 1, tileY].halfBrick() && Main.tile[tileX - 1, tileY].slope() == 0 && Main.tile[tileX + 1, tileY].slope() == 0 && WorldGen.SolidTile(tileX, tileY - 1))
                        {
                            if (WorldGen.SolidTile(tileX - 1, tileY) && !WorldGen.SolidTile(tileX + 1, tileY) && WorldGen.SolidTile(tileX - 1, tileY - 1))
                                WorldGen.SlopeTile(tileX, tileY, 3);
                            else if (WorldGen.SolidTile(tileX + 1, tileY) && !WorldGen.SolidTile(tileX - 1, tileY) && WorldGen.SolidTile(tileX + 1, tileY - 1))
                                WorldGen.SlopeTile(tileX, tileY, 4);
                        }

                        if (TileID.Sets.Conversion.Sand[Main.tile[tileX, tileY].type])
                            Tile.SmoothSlope(tileX, tileY, applyToNeighbors: false);
                    }
                }
            }

            for (int tileX = 20; tileX < Main.maxTilesX - 20; tileX++)
            {
                for (int tileY = 20; tileY < Main.maxTilesY - 20; tileY++)
                {
                    if (WorldGen.genRand.Next(2) == 0 && !Main.tile[tileX, tileY - 1].active() && Main.tile[tileX, tileY].type != 137 && Main.tile[tileX, tileY].type != 48 && Main.tile[tileX, tileY].type != 232 && Main.tile[tileX, tileY].type != 191 && Main.tile[tileX, tileY].type != 151 && Main.tile[tileX, tileY].type != 274 && Main.tile[tileX, tileY].type != 75 && Main.tile[tileX, tileY].type != 76 && WorldGen.SolidTile(tileX, tileY) && Main.tile[tileX - 1, tileY].type != 137 && Main.tile[tileX + 1, tileY].type != 137)
                    {
                        if (WorldGen.SolidTile(tileX, tileY + 1) && WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX - 1, tileY].active())
                            WorldGen.SlopeTile(tileX, tileY, 2);

                        if (WorldGen.SolidTile(tileX, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX + 1, tileY].active())
                            WorldGen.SlopeTile(tileX, tileY, 1);
                    }

                    if (Main.tile[tileX, tileY].slope() == 1 && !WorldGen.SolidTile(tileX - 1, tileY))
                    {
                        WorldGen.SlopeTile(tileX, tileY);
                        WorldGen.PoundTile(tileX, tileY);
                    }

                    if (Main.tile[tileX, tileY].slope() == 2 && !WorldGen.SolidTile(tileX + 1, tileY))
                    {
                        WorldGen.SlopeTile(tileX, tileY);
                        WorldGen.PoundTile(tileX, tileY);
                    }
                }
            }
        }
        private void CavePass(GenerationProgress progress)
        {
            progress.Message = "Carving the Moon...";
            for (int currentCaveSpot = 0; currentCaveSpot < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013); currentCaveSpot++)
            {
                float percentDone = (float)((double)currentCaveSpot / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013));
                progress.Set(percentDone);
                if (rockLayerHigh <= (double)Main.maxTilesY)
                {
                    int airType = -1;
                    WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(6, 20), WorldGen.genRand.Next(50, 300), airType);
                }
            }
        }
        private void RegolithPass(GenerationProgress progress)
        {
            progress.Message = "Sending meteors to the Moon...";
            for (int tileX = 1; tileX < Main.maxTilesX - 1; tileX++)
            {
                float progressPercent = tileX / Main.maxTilesX;
                progress.Set(progressPercent / 2f);
                float regolithChance = 6;
                for (int tileY = 1; tileY < Main.maxTilesY; tileY++)
                {
                    if (Main.tile[tileX, tileY].active())
                    {
                        if (regolithChance > 0.1)
                        {
                            Main.tile[tileX, tileY].type = (ushort)ModContent.TileType<Tiles.Regolith>();
                        }
                        regolithChance -= 0.02f;
                        if (regolithChance <= 0) break;
                    }
                }
            }
            // Generate protolith veins
            for (int tileX = 1; tileX < Main.maxTilesX - 1; tileX++)
            {
                float progressPercent = tileX / Main.maxTilesX;
                progress.Set(0.5f + progressPercent / 2f);
                float regolithChance = 6;
                for (int tileY = 1; tileY < Main.maxTilesY; tileY++)
                {
                    if (Main.tile[tileX, tileY].active())
                    {
                        double veinChance = (6 - regolithChance) / 6f * 0.006;
                        if (WorldGen.genRand.NextFloat() < veinChance || veinChance == 0)
                        {
                            //WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next((int)(6 * veinChance / 0.005), (int)(20 * veinChance / 0.005)), WorldGen.genRand.Next(50, 300), ModContent.TileType<Tiles.Protolith>());
                            WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next((int)(6 * veinChance / 0.003), (int)(20 * veinChance / 0.003)), WorldGen.genRand.Next(5, 19), ModContent.TileType<Tiles.Protolith>());
                        }
                        regolithChance -= 0.02f;
                        if (regolithChance < 0) break;
                    }
                }
            }
        }
        private void GroundPass(GenerationProgress progress)
        {
            progress.Message = "Landing on the Moon...";
            Main.worldSurface = surfaceLayer + 20; // Hides the underground layer just out of bounds
            Main.rockLayer = surfaceLayer + 60; // Hides the cavern layer way out of bounds

            int surfaceHeight = (int)surfaceLayer; // If the moon's world size is variable, this probably should depend on that
            rockLayerLow = surfaceHeight;
            rockLayerHigh = surfaceHeight;
            #region Base ground
            // Generate base ground
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                // Here, we just focus on the progress along the x-axis
                progress.Set((i / (float)Main.maxTilesX - 1)); // Controls the progress bar, should only be set between 0f and 1f
                for (int j = surfaceHeight; j < subworld.height; j++)
                {
                    Main.tile[i, j].active(true);
                    Main.tile[i, j].type = (ushort)ModContent.TileType<Tiles.Protolith>();
                }

                if (WorldGen.genRand.Next(0, 10) == 0) // Not much deviation here
                {
                    surfaceHeight += WorldGen.genRand.Next(-1, 2);
                    if (WorldGen.genRand.Next(0, 10) == 0)
                    {
                        surfaceHeight += WorldGen.genRand.Next(-2, 3);
                    }
                }

                if (surfaceHeight < rockLayerLow)
                    rockLayerLow = surfaceHeight;

                if (surfaceHeight > rockLayerHigh)
                    rockLayerHigh = surfaceHeight;

            }
            #endregion
        }
        public MoonGen(Subworld sw)
        {
            subworld = sw;
            Add(new SubworldGenPass(GroundPass));
            Add(new SubworldGenPass(CraterPass));
            Add(new SubworldGenPass(BackgroundPass));
            Add(new SubworldGenPass(RegolithPass));
            Add(new SubworldGenPass(OrePass));
            Add(new SubworldGenPass(CavePass));
            Add(new SubworldGenPass(ScuffedSmoothPass));
        }
    }
}