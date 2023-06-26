using Macrocosm.Common.Drawing.Trails;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Macrocosm.Content.Trails
{
	public class MissileTrail : VertexTrail
	{
		public override MiscShaderData TrailShader => GameShaders.Misc["RainbowRod"];
		public override float Saturation => -3f; 

		public override Color TrailColors(float progressOnStrip)
		{
			float lerpValue = Utils.GetLerpValue(0f, 0.5f, progressOnStrip, clamped: true);
			Color result = Color.Lerp(Color.Lerp(Color.White, new Color(255, 197, 155, 255), 1.115f * 0.5f), new Color(255, 68, 1, 255), lerpValue) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
			result.A /= 8;
			return result;
		}

		public override float TrailWidths(float progressOnStrip)
		{
			float lerpValue = Utils.GetLerpValue(0f, 0.06f + 1.115f * 0.01f, progressOnStrip, clamped: true);
			lerpValue = 1f - (1f - lerpValue) * (1f - lerpValue);
			return MathHelper.Lerp(20f, 100f, progressOnStrip) * lerpValue;
		}
	}
}
