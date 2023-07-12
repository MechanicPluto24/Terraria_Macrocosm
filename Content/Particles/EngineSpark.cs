using Microsoft.Xna.Framework;
using Terraria;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Rockets;

namespace Macrocosm.Content.Particles
{
    public class EngineSpark : Particle
	{
		public override int SpawnTimeLeft => 10;
		public override string TexturePath => Macrocosm.EmptyTexPath;
		public override ParticleDrawLayer DrawLayer => ParticleDrawLayer.AfterProjectiles;

		public Color ColorOnSpawn;
		public Color ColorOnDespawn;

		private float Opacity;

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
			Texture2D glow = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "SimpleGlow").Value;

			var state = spriteBatch.SaveState();
			spriteBatch.End();
			spriteBatch.Begin(BlendState.Additive, state);

			Color color = Color.Lerp(ColorOnSpawn, ColorOnDespawn, (float)TimeLeft / SpawnTimeLeft);
			spriteBatch.Draw(glow, Center - screenPosition, null, color.NewAlpha(Opacity), Rotation, glow.Size() / 2, 0.0375f * ScaleV, SpriteEffects.None, 0f);
			
			spriteBatch.End();
			spriteBatch.Begin(state);
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
			ScaleV = new Vector2(Math.Clamp(speed, 0, 5), Math.Clamp(speed, 0, 1)) * 0.11f * origScale;

 			Opacity = 1f - Utility.InverseLerp(1f, 0f, (float)TimeLeft / SpawnTimeLeft, clamped: true);

			Velocity *= 0.71f;

			Lighting.AddLight(Center, new Vector3(1f, 1f, 1f) * Scale * 0.02f);

			if (ScaleV.Y < 0.1f)
				Kill();

 		}
	}
}
