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
        private static MiscShaderData shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
            .UseProjectionMatrix(doUse: true)
            .UseImage0("Images/Extra_195")
            .UseImage1("Images/Extra_195")
            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FadeInTrail"));

        public override MiscShaderData TrailShader => shader;

        public override float Saturation => 0f;

        public override Color TrailColors(float progressOnStrip)
        {
            Color result = Color.Lerp(new Color(255, 101, 0), new Color(255, 255, 0) * progressOnStrip, progressOnStrip).WithAlpha(127);
            return result;
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return 4f;
        }
    }
}
