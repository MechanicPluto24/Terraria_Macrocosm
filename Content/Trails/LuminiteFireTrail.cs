using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
    public class LuminiteFireTrail : VertexTrail
    {
        private static readonly MiscShaderData shader = new MiscShaderData(Utility.VanillaVertexShader, "MagicMissile")
                        .UseProjectionMatrix(doUse: true)
                        .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeOutMask"))
                        .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeOutMask"))
                        .UseImage2("Images/Extra_193");

        public override MiscShaderData TrailShader => shader;

        public override Color TrailColors(float progressOnStrip)
        {
            float lerp = Utility.InverseLerp(0, 0.01f, progressOnStrip);
            Color result = Color.Lerp(Color.Lerp(Color.Black.WithAlpha(0), new Color(213, 155, 148, 0), lerp), new Color(94, 229, 163, 255), progressOnStrip);
            return result;
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return MathHelper.Lerp(70, 40, progressOnStrip);
        }
    }
}
