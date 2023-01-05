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

namespace Macrocosm.Content.Particles
{
	public class ChandriumSparkle : Particle
	{
		public override string TexturePath => Macrocosm.EmptyTexPath;

		public override int TrailCacheLenght => 15;

		[NetSync] public int owner;

		public Player OwnerPlayer => Main.player[owner];

		public override int SpawnTimeLeft => 5 * 60;

		public override void OnSpawn()
		{
  		}

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
			spriteBatch.Draw(TextureAssets.Extra[89].Value, Position - screenPosition, null, new Color(177, 107, 219, 127), 0f + Rotation, TextureAssets.Extra[89].Size() / 2f, Scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureAssets.Extra[89].Value, Position - screenPosition, null, new Color(177, 107, 219, 127), MathHelper.PiOver2 + Rotation, TextureAssets.Extra[89].Size() / 2f, Scale, SpriteEffects.None, 0f);
			Lighting.AddLight(Position, new Vector3(0.607f, 0.258f, 0.847f));
		}

		public override void AI()
		{	
			float speed;
			float inertia;
			int whipIdx = -1;

			Rotation += Velocity.X * 0.02f;
			Scale = 0.7f;
 
			int chandriumWhipType = ModContent.ProjectileType<ChandriumWhipProjectile>();
			for(int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile proj = Main.projectile[i];
				if(proj.active && proj.type == chandriumWhipType && proj.owner == OwnerPlayer.whoAmI)
				{
					whipIdx = i;
					break;
				}
			}

			bool whipActive = whipIdx >= 0;


			if (!OwnerPlayer.active || OwnerPlayer.dead)
			{
				Kill();
				return;
			}
 
			Vector2 vectorToIdlePosition = OwnerPlayer.Center - Center;
			float distanceToIdlePosition = vectorToIdlePosition.Length();

			if (distanceToIdlePosition > 40f)
			{
				speed = 26f;
				inertia = 80f;
			}
			else
			{
				speed = 12f;
				inertia = 60f;
			}

			if (distanceToIdlePosition > 20f)
			{
				vectorToIdlePosition.Normalize();
				vectorToIdlePosition *= speed;
				Velocity = (Velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
			}
			else if (Velocity == Vector2.Zero)
			{
				Velocity.X = -2f;
				Velocity.Y = -0.5f;
			}
 		}
		
		public override void OnKill()
		{
			for (int i = 0; i < 10; i++)
			{
				Dust dust = Dust.NewDustDirect(Position, 32, 32, ModContent.DustType<ChandriumSparkDust>(), Velocity.X);
				dust.velocity = (Velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(1f, 1.5f)).RotatedByRandom(MathHelper.TwoPi);
				dust.noLight = false;
				dust.noGravity = true;
			}
		}

	}
}
