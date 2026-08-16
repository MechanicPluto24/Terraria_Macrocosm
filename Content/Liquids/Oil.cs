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
    //TODO: The liquid needs to merge to stone during the world gen so that the game doesn't take an infinite amount of time to settle during world gen
    // However, it needs to not merge afterwards
    public override bool PreLiquidMerge(int liquidX, int liquidY, int tileX, int tileY, int otherLiquid)
	{
		return true; //Keep this like so unless we want the liquids to merge with something specific.
	}
    
    public override int LiquidMerge(int i, int j, int otherLiquid)
		{
            return TileID.Stone;
        }
    public override int ChooseWaterfallStyle(int i, int j) => ModContent.GetInstance<OilFall>().Slot;
}
