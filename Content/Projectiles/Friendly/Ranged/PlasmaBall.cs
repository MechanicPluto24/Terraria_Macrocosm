using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;
using Macrocosm.Common.Drawing;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
 	public class PlasmaBall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 7;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public int TimeToLive
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public bool Stopped
		{
			get => Projectile.ai[1] > 0;
			set => Projectile.ai[1] = value ? 1f : 0f;
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
 			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 200;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.Slow, 100);
			Projectile.friendly = false;
		}

		public override Color? GetAlpha(Color lightColor)
			=> Color.White; 
		 
		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.DrawSimpleTrail(Vector2.Zero, 4f, 1f, new Color(104, 255, 255), new Color(104, 255, 255, 0));
			return true;
		}

		public override void AI()
		{
			if (TimeToLive <= 0)
				return;

			Lighting.AddLight(Projectile.Center, new Vector3(0.407f, 1f, 1f) * Projectile.scale * 0.5f);

			float decelerationFactor = ((float)TimeToLive - Projectile.timeLeft) / TimeToLive;
			Projectile.velocity *= MathHelper.Lerp(0.9f, 0.85f, decelerationFactor);

			Projectile.scale -= 0.005f;

			if (Projectile.scale < 0.6f)
				Projectile.active = false;

			Dust dust;
			if (Main.rand.NextBool(30))
			{
				dust = Dust.NewDustDirect(Projectile.Center, 1, 1, ModContent.DustType<PlasmaBallDust>(), Scale: Main.rand.NextFloat(0.3f, 0.5f));
				dust.customData = Main.projectile[Projectile.whoAmI];
			}
 		}
	}
}
