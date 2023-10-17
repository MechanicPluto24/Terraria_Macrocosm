using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.GenActions
{
    // By GroxTheGreat
    public class SetMapBrightness : GenAction
    {
        public byte brightness;

        public SetMapBrightness(byte brightness)
        {
            this.brightness = brightness;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY) return false;
            Main.Map.UpdateLighting(x, y, Math.Max(Main.Map[x, y].Light, brightness));
            return UnitApply(origin, x, y, args);
        }
    }
}