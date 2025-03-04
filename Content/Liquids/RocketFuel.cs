using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Macrocosm.Content.Liquids
{
    public class RocketFuel : ModLiquid
    {
        public override void SetStaticDefaults()
        {
            LiquidFallLength = 6;
            VisualViscosity = 40;
            DefaultOpacity = 0.75f;
            AddMapEntry(new Color(155, 59, 0), CreateMapEntryName());
        }

        public override int ChooseWaterfallStyle(int i, int j) => ModContent.GetInstance<RocketFuelFall>().Slot;
    }
}
