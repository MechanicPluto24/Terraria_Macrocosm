using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Macrocosm.Content.Trails
{
	public class DianiteForkTrail : VertexTrail
	{
		public override MiscShaderData TrailShader => GameShaders.Misc["LightDisc"];
		public override float Saturation => -1f; 

		public override Color TrailColors(float progressOnStrip)
		{
			if (progressOnStrip < 0.01f)
				return new Color(0, 0, 0, 0);

			Color result = Color.Lerp(new Color(255, 101, 0) , new Color(255, 255, 0) * progressOnStrip, progressOnStrip).NewAlpha(127);

			return result;
		}
		public override float TrailWidths(float progressOnStrip)
		{
			return MathHelper.Lerp(4f,1f, progressOnStrip);
		}
	}
}
