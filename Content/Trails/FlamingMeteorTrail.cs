using Macrocosm.Common.Drawing.Trails;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Macrocosm.Content.Trails
{
	public class FlamingMeteorTrail : VertexTrail
	{
		public override MiscShaderData TrailShader => GameShaders.Misc["RainbowRod"];

		public override Color TrailColors(float progressOnStrip)
		{
			//float lerpValue = Utils.GetLerpValue(0f, 0.5f, progressOnStrip, clamped: true);
			//Color result = Color.Lerp(Color.Lerp(Color.White, new Color(0, 217, 102, 255), 1.115f * 0.5f), , lerpValue) * (1f - Utils.GetLerpValue(0.3f, 0.98f, progressOnStrip));
			//result.A /= 8;

			Color baseColor = Color.Lerp(new Color(224, 115, 20, 255), new Color(0, 217, 102, 255), (0.5f + MathF.Sin(Main.GlobalTimeWrappedHourly % 1)) / 2);
			float lerpValue = Utils.GetLerpValue(0f, 0.5f, progressOnStrip, clamped: true);
			Color result = Color.Lerp(Color.Lerp(Color.White, new Color(224, 115, 20, 255), 1.115f * 0.5f), baseColor, lerpValue) * (1f - Utils.GetLerpValue(0.5f, 0.98f, progressOnStrip));
			result.A /= 8;
			return result;
		}

		public override float TrailWidths(float progressOnStrip)
		{
			//float lerpValue = Utils.GetLerpValue(0f, 0.06f + 1.115f * 0.01f, progressOnStrip, clamped: true);
			//lerpValue = 1f - (1f - lerpValue) * (1f - lerpValue);
			//float a = MathHelper.Lerp(1f, 100f, progressOnStrip) * lerpValue;

			float lerpValue = Utils.GetLerpValue(0f, 0.06f + 1.115f * 0.01f, progressOnStrip, clamped: true);
			lerpValue = 1f - (1f - lerpValue) * (1f - lerpValue);
			return MathHelper.Lerp(1f, 240f, progressOnStrip) * lerpValue;
		}
	}
}
