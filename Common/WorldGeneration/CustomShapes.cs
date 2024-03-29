
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration
{
    public class CustomShapes
    {
        public class HollowRectangle : GenShape
        {
            private Rectangle area;


            /// <summary> 
            /// Create from XNA Rectangle.
            /// <paramref name="area"/> X and Y are offsets from place origin not absolute tile coords!
            /// </summary>
            public HollowRectangle(Rectangle area)
            {
                this.area = area;
            }

            /// <summary> Create with width and height, no offset from place origin. </summary>
            public HollowRectangle(int width, int height)
            {
                area = new Rectangle(0, 0, width, height);
            }

            /// <summary> <paramref name="area"/> X and Y are offsets from place origin not absolute tile coords! </summary>
            public void SetArea(Rectangle area)
            {
                this.area = area;
            }

            public override bool Perform(Point origin, GenAction action)
            {
                int minX = origin.X + area.Left;
                int maxX = origin.X + area.Right;
                int minY = origin.Y + area.Top;
                int maxY = origin.Y + area.Bottom;

                for (int i = minX; i < maxX; i++)
                {
                    for (int j = minY; j < maxY; j++)
                    {
                        bool isOnBoundary = i == minX || i == maxX - 1 || j == minY || j == maxY - 1;

                        if (isOnBoundary)
                        {
                            if (!UnitApply(action, origin, i, j) && _quitOnFail)
                                return false;
                        }
                    }
                }

                return true;
            }
        }

        // By GroxTheGreat
        public class ChasmSideways : GenShape
        {
            public int startheight = 20, endheight = 5, length = 60, variance, randomHeading;
            public float[] heightVariance;
            public bool dir = true;

            public ChasmSideways(int startheight, int endheight, int length, int variance, int randomHeading, float[] heightVariance = null, bool dir = true)
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
                        height = Math.Max(endheight, (int)(startheight * Utility.MultiLerp(m / (float)length, heightVariance)));
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

        // By GroxTheGreat
        public class Chasm : GenShape
        {
            public int startwidth = 20, endwidth = 5, depth = 60, variance, randomHeading;
            public float[] widthVariance;
            public bool dir = true;

            public Chasm(int startwidth, int endwidth, int depth, int variance, int randomHeading, float[] widthVariance = null, bool dir = true)
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
                        width = Math.Max(endwidth, (int)(startwidth * Utility.MultiLerp(m / (float)depth, widthVariance)));
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
    }
}
