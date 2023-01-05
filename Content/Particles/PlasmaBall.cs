using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Content.Particles
{
    public class PlasmaBall : Particle
	{
		public override int SpawnTimeLeft => 95;
		public override int TrailCacheLenght => 7;

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
			DrawSimpleTrail(Vector2.Zero, 4f, 1f, new Color(104, 255, 255), new Color(104, 255, 255, 0));

			// draw circular glow
			Texture2D glow = ModContent.Request<Texture2D>("Macrocosm/Content/Particles/PlasmaBallEffect").Value;
			var state = spriteBatch.SaveState();
			spriteBatch.End();
			spriteBatch.Begin(BlendState.Additive, state);
			spriteBatch.Draw(glow, Center - screenPosition, null, new Color(89, 151, 193), Rotation, glow.Size() / 2, 0.0375f * Scale, SpriteEffects.None, 0f);
			spriteBatch.Restore(state);
			
			spriteBatch.Draw(Texture, Center - screenPosition, null, Color.White, Rotation, Size / 2, Scale, SpriteEffects.None, 0f);
 		}

		public override void AI()
		{
			Lighting.AddLight(Center, new Vector3(0.407f, 1f, 1f) * Scale * 0.5f);

			float decelerationFactor = ((float)SpawnTimeLeft - TimeLeft) / SpawnTimeLeft;
			Velocity *= MathHelper.Lerp(0.9f, 0.85f, decelerationFactor);

			if(Scale >= 0)
				Scale -= 0.008f;
 		}
	}
}
