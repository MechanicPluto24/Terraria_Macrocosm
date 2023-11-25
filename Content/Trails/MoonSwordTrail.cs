using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
    public class MoonSwordTrail : VertexTrail
    {
        private static MiscShaderData shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
                        .UseProjectionMatrix(doUse: true)
                        .UseImage0("Images/Extra_194")
                        .UseImage1("Images/Extra_194")
                        .UseImage2("Images/Extra_194");

        public override MiscShaderData TrailShader => shader;

        public override Color TrailColors(float progressOnStrip)
        {
            shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
                        .UseProjectionMatrix(doUse: true)
                        .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FadeOutMask"))
                        .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FadeOutMask"))
                        .UseImage2("Images/Extra_193");

            float lerp = Utility.InverseLerp(0, 0.01f, progressOnStrip);
            Color result = Color.Lerp(Color.Lerp(Color.Black.WithAlpha(0), new Color(50, 255, 200, 0), lerp), new Color(91, 248, 158, 255), progressOnStrip);
            return result;
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return MathHelper.Lerp(70, 60, progressOnStrip);
        }
    }
}
