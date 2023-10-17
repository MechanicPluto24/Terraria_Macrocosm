using System;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.GenShapes
{    
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
}