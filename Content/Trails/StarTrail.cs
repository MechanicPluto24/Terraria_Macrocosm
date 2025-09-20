using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails;

public class StarTrail : VertexTrail
{
    public Color Color { get; set; } = new Color(100, 100, 255, 0);

    public override MiscShaderData TrailShader => new MiscShaderData(Utility.VanillaVertexShader, "MagicMissile")
        .UseProjectionMatrix(doUse: true)
        .UseSaturation(Saturation)
        .UseImage0(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "FadeOutTrail"))
        .UseImage1(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "RocketExhaustTrail1"))
        .UseImage2(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "RocketExhaustTrail2"));

    public override float Saturation => -2f;

    public override Color TrailColors(float progressOnStrip)
    {
        return Color.Lerp(Color.Transparent, Color * 0.8f, progressOnStrip * 1 / progressOnStrip);
    }

    public override float TrailWidths(float progressOnStrip)
    {
        return 45;
    }
}
