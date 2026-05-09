using Macrocosm.Common.Systems;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Gores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Liquids.WaterStyles;

public class PollutionWaterStyle : ModWaterStyle
{
    private Asset<Texture2D> rainTexture;
    protected virtual ModWaterfallStyle WaterfallStyle => ModContent.GetInstance<PollutionWaterfallStyle>();
    protected virtual int SplashDustType => ModContent.DustType<PollutionWaterSplashDust>();
    protected virtual int DropletGoreType => ModContent.GoreType<PollutionWaterDropletGore>();

    public override void Load()
    {
        rainTexture = ModContent.Request<Texture2D>(Texture.Replace(nameof(PollutionWaterStyle), "PollutionRain"));
    }

    public static ModWaterStyle GetCurrentStyle()
    {
        float level = TileCounts.Instance.PollutionLevel;
        if (level >= 500f)
            return ModContent.GetInstance<PollutionWaterStyle5>();

        if (level >= 400f)
            return ModContent.GetInstance<PollutionWaterStyle4>();

        if (level >= 300f)
            return ModContent.GetInstance<PollutionWaterStyle3>();

        if (level >= 200f)
            return ModContent.GetInstance<PollutionWaterStyle2>();

        return ModContent.GetInstance<PollutionWaterStyle>();
    }

    public override int ChooseWaterfallStyle() => WaterfallStyle.Slot;

    public override int GetSplashDust() => SplashDustType;

    public override int GetDropletGore() => DropletGoreType;

    public override void LightColorMultiplier(ref float r, ref float g, ref float b)
    {
        r = 1f;
        g = 1f;
        b = 1f;
    }

    public override Color BiomeHairColor() => Color.DarkGray;

    public override byte GetRainVariant() => (byte)Main.rand.Next(3);

    public override Asset<Texture2D> GetRainTexture() => rainTexture;
}

public class PollutionWaterStyle2 : PollutionWaterStyle
{
    protected override ModWaterfallStyle WaterfallStyle => ModContent.GetInstance<PollutionWaterfallStyle2>();
    protected override int SplashDustType => ModContent.DustType<PollutionWaterSplashDust2>();
    protected override int DropletGoreType => ModContent.GoreType<PollutionWaterDropletGore2>();
}

public class PollutionWaterStyle3 : PollutionWaterStyle
{
    protected override ModWaterfallStyle WaterfallStyle => ModContent.GetInstance<PollutionWaterfallStyle3>();
    protected override int SplashDustType => ModContent.DustType<PollutionWaterSplashDust3>();
    protected override int DropletGoreType => ModContent.GoreType<PollutionWaterDropletGore3>();
}

public class PollutionWaterStyle4 : PollutionWaterStyle
{
    protected override ModWaterfallStyle WaterfallStyle => ModContent.GetInstance<PollutionWaterfallStyle4>();
    protected override int SplashDustType => ModContent.DustType<PollutionWaterSplashDust4>();
    protected override int DropletGoreType => ModContent.GoreType<PollutionWaterDropletGore4>();
}

public class PollutionWaterStyle5 : PollutionWaterStyle
{
    protected override ModWaterfallStyle WaterfallStyle => ModContent.GetInstance<PollutionWaterfallStyle5>();
    protected override int SplashDustType => ModContent.DustType<PollutionWaterSplashDust5>();
    protected override int DropletGoreType => ModContent.GoreType<PollutionWaterDropletGore5>();
}
