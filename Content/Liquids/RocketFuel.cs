using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using Terraria.GameContent.Liquid;
using Terraria.ModLoader;

namespace Macrocosm.Content.Liquids;

public class RocketFuel : ModLiquid
{
    public override void SetStaticDefaults()
    {
        LiquidRenderer.WATERFALL_LENGTH[Type] = 6;
        LiquidRenderer.VISCOSITY_MASK[Type] = 40;
        LiquidRenderer.DEFAULT_OPACITY[Type] = 0.75f;
        AddMapEntry(new Color(155, 59, 0), CreateMapEntryName());
    }

    public override int ChooseWaterfallStyle(int i, int j) => ModContent.GetInstance<RocketFuelFall>().Slot;
}
