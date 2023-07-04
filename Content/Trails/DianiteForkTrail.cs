using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
	public class DianiteForkTrail : VertexTrail
	{
		public override MiscShaderData TrailShader  {
			get
			{
				var shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile").UseProjectionMatrix(doUse: true);
				shader.UseImage0("Images/Extra_" + (short)195);
				shader.UseImage1("Images/Extra_" + (short)195);
				shader.UseImage2(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/FadeOutTrail"));
				return shader;
			}
		}
		public override float Saturation => 0f;

		public override Color TrailColors(float progressOnStrip)
		{
			if (progressOnStrip < 0.11f)
				return new Color(0, 0, 0, 0);

			Color result = Color.Lerp(new Color(255, 101, 0) , new Color(255, 255, 0) * progressOnStrip, progressOnStrip).NewAlpha(255);

			return result;
		}

		public override float TrailWidths(float progressOnStrip)
		{
			float multiplier = 1f;

			if(Owner is Projectile proj)
			{
				multiplier = proj.velocity.Length();
			}

			return MathHelper.Lerp(5f + 0.1f * multiplier, 0.2f * multiplier, progressOnStrip) ;
		}
	}
}
