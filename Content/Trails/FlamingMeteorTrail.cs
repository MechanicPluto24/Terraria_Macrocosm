using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
	public class FlamingMeteorTrail : VertexTrail
	{
		private static readonly MiscShaderData shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
                            .UseProjectionMatrix(doUse: true)
                            .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FadeOutTrail"))
                            .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "RocketExhaustTrail1"))
                            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "RocketExhaustTrail1"));


        public override MiscShaderData TrailShader => shader;

		public override Color TrailColors(float progressOnStrip)
		{
            return Color.Lerp(new Color(242, 142, 35, 0) * 0.1f, new Color(242, 142, 35, 0) * 0.8f, progressOnStrip * 1/progressOnStrip);
		}

		public override float TrailWidths(float progressOnStrip)
		{
			return 45;
		}
	}
}
