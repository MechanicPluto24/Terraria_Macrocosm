using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Macrocosm.Content.Trails
{
    public class ImbriumMeteorTrail : VertexTrail
    {
        public override MiscShaderData TrailShader => GameShaders.Misc["MagicMissile"];

        public override Color TrailColors(float progressOnStrip)
        {
            Color baseColor = new(0, 217, 102, 255);
            float lerpValue = Utils.GetLerpValue(0f, 0.5f, progressOnStrip, clamped: true);
            Color result = (new Color(0, 217, 102) * (2f * (progressOnStrip))).WithOpacity(0f);
            return result;
        }

        public override float TrailWidths(float progressOnStrip)
        {
            float lerpValue = Utils.GetLerpValue(0f, 0.06f + 1.115f * 0.01f, progressOnStrip, clamped: true);
            lerpValue = 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(100f, 50f, progressOnStrip);
        }
    }
}
