using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Macrocosm.Content.Trails
{
	public class MissileTrail : VertexTrail
	{
		public override MiscShaderData TrailShader => GameShaders.Misc["RainbowRod"];
		public override float Saturation => -2f; 

		public override Color TrailColors(float progressOnStrip)
		{
			if (progressOnStrip < 0.28f)
				return Color.Black.NewAlpha(0f);

			float lerpValue = Utils.GetLerpValue(0.15f, 0.55f, progressOnStrip, clamped: true);
			Color result = Color.Lerp(Color.White * 1f, new Color(255, 68, 1, 255), lerpValue) * Utils.GetLerpValue(0.16f, 0.98f, progressOnStrip);
			result.A /= 8;
			return result;
		}
		public override float TrailWidths(float progressOnStrip)
		{
			float lerpValue = Utils.GetLerpValue(0.02f, 0.2f, progressOnStrip, clamped: true);
			lerpValue = 1f - (1f - lerpValue) * (1f - lerpValue);
			return MathHelper.Lerp(16f, 120f, progressOnStrip) * lerpValue;
		}
	}
}
