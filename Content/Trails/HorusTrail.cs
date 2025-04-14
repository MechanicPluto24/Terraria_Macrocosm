using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
    public class HorusTrail : VertexTrail
    {
        public override MiscShaderData TrailShader => new MiscShaderData(shader: Utility.VanillaVertexShader, "MagicMissile")
            .UseProjectionMatrix(doUse: true)
            .UseSaturation(Saturation)
            .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeOutTrail"))
            .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "RocketExhaustTrail1"))
            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "RocketExhaustTrail2"));

        public override float Saturation => -2f;

        public override Color TrailColors(float progressOnStrip)
        {
            return Color.Lerp(new Color(127, 200, 155, 0) * 0.8f, new Color(255, 180, 131, 0) * 1f, progressOnStrip * 2f / progressOnStrip);
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return MathHelper.Lerp(40f, 20f, progressOnStrip);
        }
    }
}
