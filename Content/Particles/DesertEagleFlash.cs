using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Trails;
using Macrocosm.Content.Projectiles.Friendly.Summon;
using Macrocosm.Common.Netcode;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;
using static Terraria.ModLoader.PlayerDrawLayer;
using Terraria.ID;
using Macrocosm.Content.Buffs.GoodBuffs;
using System;
using Macrocosm.Content.Projectiles.Hostile;
using Macrocosm.Content.Projectiles.Friendly.Ranged;

namespace Macrocosm.Content.Particles
{
	public class DesertEagleFlash : Particle
	{
		public override int TrailCacheLenght => 15;

		public Projectile Owner { get; set; }

		public override int SpawnTimeLeft => numFrames * frameSpeed - 1;

		// TODO: move these to base type
		private int numFrames = 4;
		private int currentFrame = 0;
		private int frameCnt = 0;
		private int frameSpeed = 6;

		public override void OnSpawn()
		{
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
			spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), lightColor, Rotation, new Vector2(Size.X, Size.Y/numFrames) * 0.5f, ScaleV, SpriteEffects.None, 0f);
			Lighting.AddLight(Position, Color.White.ToVector3());
		}

		public override void AI()
		{
			if(!Owner.active && !(Owner.type == ModContent.ProjectileType<ZombieSecurityBullet>() || (Owner.type == ModContent.ProjectileType<Tycho50Bullet>())))
				Kill();

 		}
		
		public override void OnKill()
		{
	
		}

	}
}
