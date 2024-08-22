using Macrocosm.Common.Drawing.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
    public class StarTrail : VertexTrail
    {
        public override MiscShaderData TrailShader => new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
            .UseProjectionMatrix(doUse: true)
            .UseSaturation(Saturation)
            .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeOutTrail"))
            .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "RocketExhaustTrail1"))
            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "RocketExhaustTrail2"));

        public override float Saturation => -1f;

        public override Color TrailColors(float progressOnStrip)
        {
            return Color.Lerp(Color.Transparent, new Color(100, 100, 255, 0) * 0.8f, progressOnStrip * 1 / progressOnStrip);
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return 40;
        }
    }
}
