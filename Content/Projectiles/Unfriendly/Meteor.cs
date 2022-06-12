using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Unfriendly {
	//Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
	public class Meteor : ModProjectile {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Flaming Meteor");
		}

		public override void SetDefaults() {
			Projectile.width = Projectile.height = 22;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit) {
			target.AddBuff(BuffID.OnFire, 360, true);
			target.AddBuff(BuffID.Burning, 90, true);
		}

		public override void AI() {
			Projectile.velocity.Y += 0.068f;
			if(Projectile.velocity.Y > 16f)
				Projectile.velocity.Y = 16f;

			if(Projectile.velocity != Vector2.Zero)
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

			if(Main.rand.NextFloat() < 0.3f) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FlameBurst);
				dust.velocity = new Vector2(0f, Main.rand.NextFloat(0.2f, 1.5f));
			}
		}

		public override bool PreDraw(ref Color lightColor) {
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, 
				null, Color.White, Projectile.rotation, Projectile.Size / 2f, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
