using Macrocosm.Common.Drawing.Trails;
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
			float lerpValue = Utils.GetLerpValue(0.2f, 0.5f, progressOnStrip, clamped: true);
			Color result = Color.Lerp(new Color(255, 197, 155, 255), new Color(255, 68, 1, 255), lerpValue) * (1f - Utils.GetLerpValue(0f, 0.95f, progressOnStrip));
 			return result;
		}
		public override float TrailWidths(float progressOnStrip)
		{
			float lerpValue = Utils.GetLerpValue(0.1f, 0.5f, progressOnStrip, clamped: true);
			return MathHelper.Lerp(4f, 100f, progressOnStrip) * lerpValue;
		}
	}
}
