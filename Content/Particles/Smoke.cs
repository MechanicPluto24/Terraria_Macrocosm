using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Macrocosm.Content.Particles
{
	public class Smoke : Particle
	{
		public override int FrameNumber => 3;
		public override bool SetRandomFrameOnSpawn => true;

		[NetSync] public bool FadeIn = false;
		[NetSync] public bool FadeOut = true;

		public Color DrawColor = Color.White;

		public int TargetAlpha = 255; // target opacity

		public float Deceleration = 0.98f;

		public int FadeInSpeed = 1;
		public int FadeOutSpeed = 4;

		public float ScaleDownSpeed = 0.005f;

		private float alpha = 255;
		private bool fadedIn = false;

		private bool collide1;
		public bool Collide1 { get => collide1; set => collide1 = value; }

		bool Collide = false;


		public override ParticleDrawLayer DrawLayer => ParticleDrawLayer.BeforeNPCs;


		public override void OnSpawn()
		{
			if(FadeIn && FadeOut)
			{
				alpha = 0;
			}
			else
			{
				if (FadeIn)
				{
					alpha = 0;
				}
				else if (FadeOut)
				{
					alpha = 255;
				}
			}

		}

		private bool bounced;

		public override void AI()
        {
			Velocity *= Deceleration;
			Scale -= ScaleDownSpeed;

			if(FadeIn && FadeOut)
			{
				if (!fadedIn)
				{	
					if(alpha >= TargetAlpha)
					{
						fadedIn = true;
					}
					else
					{
						alpha += FadeInSpeed;
					}
				}
				else if(alpha > 0)
				{
					alpha -= FadeOutSpeed;
				}
			}
            else
            {
				if (FadeIn)
				{
					if(alpha < TargetAlpha)
						alpha += FadeInSpeed;
				}
                else if(FadeOut)
                {
                     if(alpha > 0)
						alpha -= FadeOutSpeed;
                }
            }

			if (Collide1)
			{
				var originalVelocity = Velocity;

				var collisionVelocity = Collision.TileCollision(Position, Velocity, 1, 1);

				if(originalVelocity != collisionVelocity && !bounced) 
				{
					bounced = true;
					Velocity = collisionVelocity;
					Velocity.Y = -MathF.Abs(Velocity.Y);
					Velocity.Y *= Main.rand.NextFloat(0.1f, 0.25f);
					Velocity.X *= Main.rand.NextFloat(5f, 10f);
				}
			}

			alpha = (int)MathHelper.Clamp(alpha, 0, 255);

            if (Scale < 0.1 || (fadedIn && alpha <= 0))
 				Kill();
        }

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
			spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), Color.Lerp(DrawColor, lightColor, 0.5f).WithAlpha(DrawColor.A) * ((float)alpha/255f), Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
		}

	}
}
