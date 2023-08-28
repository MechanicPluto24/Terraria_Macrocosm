using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
	internal class MissileTrail : VertexTrail
	{
		public override MiscShaderData TrailShader => GameShaders.Misc["RainbowRod"];
		public override float Saturation => -2f;

		public override int StartIndex => 0;

		public override Color TrailColors(float progressOnStrip)
		{
			float lerpValue = Utils.GetLerpValue(0.4f, 0.55f, progressOnStrip, clamped: true) ;
			Color result = Color.Lerp(new Color(247, 246, 194), new Color(255, 68, 1, 255), lerpValue) * Utils.GetLerpValue(0.27f, 0.98f, progressOnStrip);
			result.A /= 8;
			return result;
		}
		public override float TrailWidths(float progressOnStrip)
		{
			float lerpValue = Utils.GetLerpValue(0.02f, 0.2f, progressOnStrip, clamped: true);
			lerpValue = 1f - (1f - lerpValue) * (1f - lerpValue);
			return MathHelper.Lerp(75f, 35f, progressOnStrip);
		}
	}
}
