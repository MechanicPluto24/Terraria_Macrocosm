using System;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.GenShapes
{
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