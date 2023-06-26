using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Macrocosm.Common.Drawing.Particles;
using Terraria.ModLoader;
using Macrocosm.Content.Projectiles.Hostile;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.Particles
{
	public class TintableExplosion : Particle
	{
		public override int TrailCacheLenght => 15;

		public Projectile Owner { get; set; }

		public Color DrawColor { get; set; }

		public override int SpawnTimeLeft => numFrames * frameSpeed - 1;

		// TODO: move these to base type
		private int numFrames = 7;
		private int currentFrame = 0;
		private int frameCnt = 0;
		private int frameSpeed = 6;

		private bool rotateClockwise = false;

		public override void OnSpawn()
		{
			rotateClockwise = Main.rand.NextBool();
  		}

		public override Rectangle? GetFrame()
		{
			frameCnt++;
			if (frameCnt == frameSpeed)
			{
				frameCnt = 0;
				currentFrame++;

				if (currentFrame >= numFrames)
					currentFrame = 0;
			}

			int frameHeight = Texture.Height / numFrames;
			return new Rectangle(0, frameHeight * currentFrame, Texture.Width, frameHeight);
		}

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
			var state = spriteBatch.SaveState();

			spriteBatch.End();
			spriteBatch.Begin(BlendState.Additive, state);

			spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), DrawColor, Rotation, new Vector2(Size.X, Size.Y/numFrames) * 0.5f, ScaleV, SpriteEffects.None, 0f);
			Lighting.AddLight(Position, DrawColor.ToVector3());

			spriteBatch.End();
			spriteBatch.Begin(state);

		}

		public override void AI()
		{
			if (rotateClockwise)
				Rotation += 0.005f;
			else
				Rotation -= 0.005f;
 		}
		
		public override void OnKill()
		{
		}
	}
}
