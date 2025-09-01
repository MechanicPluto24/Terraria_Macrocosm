using Macrocosm.Common.Drawing.Trails;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Macrocosm.Content.Trails
{
    public class MissileTrail : VertexTrail
    {
        public override MiscShaderData TrailShader => GameShaders.Misc["RainbowRod"];
        public override float Saturation => -1.9f;

        public override int StartIndex => 0;

        public override Color TrailColors(float progressOnStrip)
        {
            float lerpValue = Utils.GetLerpValue(0.4f, 0.55f, progressOnStrip, clamped: true);
            Color result = Color.Lerp(new Color(247, 246, 194, 255), new Color(255, 68, 1, 127), lerpValue) * Utils.GetLerpValue(0.3f, 0.98f, progressOnStrip);
            //result.A /= 8;
            return result;
        }
        public override float TrailWidths(float progressOnStrip)
        {
            return MathHelper.Lerp(85f, 64f, progressOnStrip);
        }
    }
}
