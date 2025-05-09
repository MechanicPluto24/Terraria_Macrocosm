using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
    public class DianiteForkTrail : VertexTrail
    {
        private static MiscShaderData shader = new MiscShaderData(Utility.VanillaVertexShader, "MagicMissile")
            .UseProjectionMatrix(doUse: true)
            .UseImage0("Images/Extra_195")
            .UseImage1("Images/Extra_195")
            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "FadeInTrail"));

        public override MiscShaderData TrailShader => shader;

        public override float Saturation => 0f;

        public override Color TrailColors(float progressOnStrip)
        {
            Color result = Color.Lerp(new Color(248, 137, 0).WithLuminance(0.5f), Color.Yellow, progressOnStrip);
            return result;
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return MathHelper.Lerp(4f, 6f, progressOnStrip);
        }
    }
}
