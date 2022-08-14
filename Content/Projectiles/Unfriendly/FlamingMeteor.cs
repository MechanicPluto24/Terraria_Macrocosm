using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Unfriendly
{
	//Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
	public class FlamingMeteor : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flaming Meteor");
			Main.projFrames[Type] = 6;
		}

		public override void SetDefaults()
		{
			Projectile.width = 28;
			Projectile.height = 28;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 360, true);
			target.AddBuff(BuffID.Burning, 90, true);
		}

		public override void AI()
		{
			Projectile.velocity.Y += 0.068f;
			if (Projectile.velocity.Y > 16f)
				Projectile.velocity.Y = 16f;

			if (Projectile.velocity != Vector2.Zero)
				Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

			if (Main.rand.NextFloat() < 0.35f)
			{
				int num5 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 128, default, 3f);
				Main.dust[num5].noGravity = true;
				Main.dust[num5].velocity.X *= 2f;
				Main.dust[num5].velocity.Y *= 1.5f;

				if (num5 % 2 == 0)
					Main.dust[num5].color = new Color(0, 255, 100);
			}


			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Type]; // 6 frames @ 4 ticks/frame
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{

			Texture2D tex = TextureAssets.Projectile[Type].Value;

			Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);

			Vector2 origin = Projectile.Size / 2f + new Vector2(6, 32);

			Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition,
				sourceRect, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
		}
	}
}
