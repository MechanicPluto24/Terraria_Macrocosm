using Microsoft.Xna.Framework;
using Terraria;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Drawing.Particles;

namespace Macrocosm.Content.Particles
{
    public class PortalSpark : Particle
	{
		public override int SpawnTimeLeft => 250;
		public override string TexturePath => Macrocosm.EmptyTexPath;
		public override ParticleDrawLayer DrawLayer => ParticleDrawLayer.AfterProjectiles;

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
			Texture2D glow = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/CurvedGlow").Value;
			var state = spriteBatch.SaveState();
			spriteBatch.End();
			spriteBatch.Begin(BlendState.Additive, state);
			spriteBatch.Draw(glow, Center - screenPosition, null, new Color(89, 151, 193), Rotation, glow.Size() / 2, 0.0375f * ScaleV, SpriteEffects.None, 0f);
			spriteBatch.End();
			spriteBatch.Begin(state);

			//spriteBatch.Draw(glow, Center - screenPosition, null, Color.White, Rotation, Size / 2, ScaleV, SpriteEffects.None, 0f);
 		}

		bool spawned = false;
		float origScale = 0f;
		public override void AI()
		{
			if (!spawned)
			{
				origScale = Scale;
				spawned = true;
			}
			float speed = Velocity.LengthSquared() * 0.4f;
			Rotation = Velocity.ToRotation();
			ScaleV = new Vector2(Math.Clamp(speed, 0, 15), Math.Clamp(speed, 0, 1)) * 0.09f * origScale;

			Velocity *= 0.91f;

			Lighting.AddLight(Center, new Vector3(1f, 1f, 1f) * Scale * 0.02f);

			if (ScaleV.Y < 0.1f)
				Kill();
 		}
	}
}
