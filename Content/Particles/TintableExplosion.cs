using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.Particles
{
	public class TintableExplosion : Particle
	{
		public override int TrailCacheLenght => 15;

		public Projectile Owner;
		public Color DrawColor;
		public int NumberOfInnerReplicas;
		public float ReplicaScalingFactor;
		public BlendState BlendState = BlendState.Additive;

		public override int FrameNumber => 7;
		public override int FrameSpeed => 6;
		public override bool DespawnOnAnimationComplete => true;


		private bool rotateClockwise = false;

		public override void OnSpawn()
		{
			rotateClockwise = Main.rand.NextBool();
  		}

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
			var state = spriteBatch.SaveState();

			spriteBatch.End();
			spriteBatch.Begin(BlendState, state);

			for(int i = 0; i < NumberOfInnerReplicas; i++)
			{
				float invProgress = 1f - (float)i / NumberOfInnerReplicas;
				float scale = Scale * ((1f - ReplicaScalingFactor) + ReplicaScalingFactor * invProgress);
				spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), DrawColor, Rotation, Size * 0.5f, scale, SpriteEffects.None, 0f);
			}

			Lighting.AddLight(Center, DrawColor.ToVector3());

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
