using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Macrocosm.Content.Trails
{
    public class FlamingMeteorTrail : VertexTrail
    {
        private static MiscShaderData shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
                            .UseProjectionMatrix(doUse: true)
                            .UseImage0("Images/Extra_194")
                            .UseImage1("Images/Extra_194")
                            .UseImage2("Images/Extra_194");

        public override MiscShaderData TrailShader => shader;

        public override Color TrailColors(float progressOnStrip)
        {
            if (progressOnStrip < 0.1f)
                return Color.Black.WithAlpha(0);

            Color baseColor = Color.Lerp(new Color(224, 115, 20, 255), new Color(0, 217, 102, 255), (0.5f + MathF.Sin(Main.GlobalTimeWrappedHourly % 1)) / 2);
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progressOnStrip, clamped: true);
            Color result = Color.Lerp(Color.Lerp(Color.White, new Color(224, 115, 20, 255), 1.115f * 0.5f), baseColor, lerpValue) * (1f - Utils.GetLerpValue(0.5f, 0.98f, progressOnStrip));
            result.A /= 8;
            return result;
        }

        public override float TrailWidths(float progressOnStrip)
        {
            float lerpValue = Utils.GetLerpValue(0f, 0.06f + 1.115f * 0.01f, progressOnStrip, clamped: true);
            return MathHelper.Lerp(120f, 160f, progressOnStrip) * lerpValue;
        }
    }
}
