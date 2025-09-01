using System;
using Terraria;
using Terraria.Utilities;

namespace Macrocosm.Common.DataStructures
{
    public class CellularAutomaton
    {
        public Cell[][] Map { get; }
        public CellularAutomaton(int width, int height, int seed, float fill = 0.5f, int smoothing = 4)
        {
            UnifiedRandom random = new(seed);

            Map = new Cell[width][];
            for (int i = 0; i < width; i++)
            {
                Map[i] = new Cell[height];
                for (int j = 0; j < height; j++)
                {
                    if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                    {
                        continue;
                    }

                    Map[i][j] = random.NextFloat() < fill ? Cell.Terrain : Cell.Air;
                }
            }



            for (int x = 0; x < smoothing; x++)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        uint neighbourCount = 0;
                        if (i != 0)
                        {
                            neighbourCount += (uint)Map[i - 1][j];
                            if (j != 0)
                            {
                                neighbourCount += (uint)Map[i - 1][j - 1];
                            }

                            if (j != height - 1)
                            {
                                neighbourCount += (uint)Map[i - 1][j + 1];
                            }
                        }

                        if (j != 0)
                        {
                            neighbourCount += (uint)Map[i][j - 1];
                            if (i != 0)
                            {
                                neighbourCount += (uint)Map[i - 1][j - 1];
                            }

                            if (i != width - 1)
                            {
                                neighbourCount += (uint)Map[i + 1][j - 1];
                            }
                        }

                        if (i != width - 1)
                        {
                            neighbourCount += (uint)Map[i + 1][j];
                        }

                        if (j != height - 1)
                        {
                            neighbourCount += (uint)Map[i][j + 1];
                        }

                        if (neighbourCount > 4)
                        {
                            Map[i][j] = Cell.Terrain;
                        }
                        else if (neighbourCount < 4)
                        {
                            Map[i][j] = Cell.Air;
                        }
                    }
                }
            }
        }

        public void ForEachCell(Action<int, int, Cell> action)
        {
            for (int i = 0; i < Map.Length; i++)
            {
                for (int j = 0; j < Map[i].Length; j++)
                {
                    action(i, j, Map[i][j]);
                }
            }
        }
    }

    public enum Cell : uint
    {
        Air,
        Terrain
    }
}
