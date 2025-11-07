using Microsoft.Xna.Framework;
using ModLiquidLib.ID;
using ModLiquidLib.ModLoader;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Liquids;

public class Oil : ModLiquid
{
    public override void SetStaticDefaults()
    {
        LiquidRenderer.WATERFALL_LENGTH[Type] = 3;
        LiquidRenderer.VISCOSITY_MASK[Type] = 160;
        LiquidRenderer.DEFAULT_OPACITY[Type] = 0.85f;
        SlopeOpacity = 1f;
        LiquidfallOpacityMultiplier = 0.5f;

        WaterRippleMultiplier = 0.3f;
        FallDelay = 2;
        SplashSound = SoundID.SplashWeak;

        AddMapEntry(Color.Black, CreateMapEntryName());
    }

    public override int ChooseWaterfallStyle(int i, int j) => ModContent.GetInstance<OilFall>().Slot;
}
