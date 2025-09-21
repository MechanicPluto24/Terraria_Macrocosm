using Microsoft.Xna.Framework;
using ModLiquidLib.ID;
using ModLiquidLib.ModLoader;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Liquids;

public class Oil : ModLiquid
{
    public override void SetStaticDefaults()
    {
        LiquidFallLength = 3;
        VisualViscosity = 160;

        DefaultOpacity = 0.85f;
        SlopeOpacity = 1f;
        LiquidfallOpacityMultiplier = 0.5f;

        WaterRippleMultiplier = 0.3f;
        FallDelay = 2;
        SplashSound = SoundID.SplashWeak;

        AddMapEntry(Color.Black, CreateMapEntryName());
    }

    public override int ChooseWaterfallStyle(int i, int j) => ModContent.GetInstance<OilFall>().Slot;
}
