using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Unfriendly{
	//Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
	public class Meteor : ModProjectile{
		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Flaming Meteor");
		}

		public override void SetDefaults(){
			projectile.width = projectile.height = 22;
			projectile.hostile = true;
			projectile.friendly = false;
			projectile.tileCollide = false;
			projectile.timeLeft = 600;
			projectile.penetrate = -1;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit){
			target.AddBuff(BuffID.OnFire, 360, true);
			target.AddBuff(BuffID.Burning, 90, true);
		}

		public override void AI(){
			projectile.velocity.Y += 0.068f;
			if(projectile.velocity.Y > 16f)
				projectile.velocity.Y = 16f;

			if(projectile.velocity != Vector2.Zero)
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi;

			if(Main.rand.NextFloat() < 0.3f){
				Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Fire);
				dust.velocity = new Vector2(0f, Main.rand.NextFloat(0.2f, 1.5f));
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor){
			spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, projectile.Size / 2f, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}
