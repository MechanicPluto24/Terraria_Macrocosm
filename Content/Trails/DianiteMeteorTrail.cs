using Macrocosm.Common.Drawing.Trails;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Macrocosm.Content.Trails
{
    public class DianiteMeteorTrail : VertexTrail
    {
        public override MiscShaderData TrailShader => GameShaders.Misc["FlameLash"];
        public override float Saturation => -1.4f;

        public override int StartIndex => 0;

        public override Color TrailColors(float progressOnStrip)
        {
            float lerpValue = Utils.GetLerpValue(0f, 0.5f, progressOnStrip, clamped: true);
            Color result = Color.Lerp(Color.Lerp(Color.White, new Color(255, 197, 155, 255), 1.115f * 0.5f), new Color(127, 127, 0, 0), lerpValue) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            result.A /= 8;
            return result;
        }

        public override float TrailWidths(float progressOnStrip)
        {
            if (Owner is Projectile projectile)
            {
                float lerpValue = Utils.GetLerpValue(0f, 0.06f + 1.115f * 0.01f, progressOnStrip, clamped: true);
                lerpValue = 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(projectile.width * 1f, projectile.width * 0.2f, progressOnStrip) * lerpValue;
            }

            return 1f;
        }
    }
}
