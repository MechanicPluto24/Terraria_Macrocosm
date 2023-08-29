using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
	public class TintableExplosion : Particle
	{
		public override int TrailCacheLenght => 15;

		public Color DrawColor;
		public int NumberOfInnerReplicas;
		public float ReplicaScalingFactor;

		public override int FrameNumber => 7;
		public override int FrameSpeed => 4;
		public override bool DespawnOnAnimationComplete => true;


		private bool rotateClockwise = false;

		public override void OnSpawn()
		{
			rotateClockwise = Main.rand.NextBool();
  		}

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{

			spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), DrawColor, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);

			var state = spriteBatch.SaveState();
			spriteBatch.End();
			spriteBatch.Begin(BlendState.Additive, state);

			for (int i = 1; i < NumberOfInnerReplicas; i++)
			{
				float progress = (float)i / NumberOfInnerReplicas;
				float invProgress = 1f - progress;
				float scale = Scale * ((ReplicaScalingFactor) + (1f - ReplicaScalingFactor) * invProgress);
				Color color = DrawColor.NewAlpha(((float)TimeLeft / SpawnTimeLeft) * 0.5f);
				spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), color, Rotation, Size * 0.5f, scale, SpriteEffects.None, 0f);
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
