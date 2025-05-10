using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
    public class TrailScepterTrail : VertexTrail
    {
        public override MiscShaderData TrailShader => new MiscShaderData(Utility.VanillaVertexShader, "MagicMissile")
            .UseProjectionMatrix(doUse: true)
            .UseSaturation(Saturation)
            .UseImage0(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "FadeOutTrail"))
            .UseImage1(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "RocketExhaustTrail1"))
            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Spark7"));

        public override float Saturation => -2f;

        public override Color TrailColors(float progressOnStrip)
        {
            return Color.Lerp(new Color(255, 255, 255, 0) * 0.5f, new Color(0, 217, 102, 0) * 1f, 0.8f);
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return MathHelper.Lerp(18f, 7f, progressOnStrip);
        }
    }
}
