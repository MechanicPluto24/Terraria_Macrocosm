using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics;
using Terraria;
using Macrocosm.Content.Liquids.WaterStyles;
using Terraria.ModLoader;

namespace Macrocosm.Content.Liquids;

public class Oil : ModLiquid
{
    public override void SetStaticDefaults()
    {
        LiquidFallLength = 3;
        VisualViscosity = 160;
        DefaultOpacity = 0.95f;
        AddMapEntry(Color.Black, CreateMapEntryName());
    }

    public override int ChooseWaterfallStyle(int i, int j) => ModContent.GetInstance<OilFall>().Slot;
}
