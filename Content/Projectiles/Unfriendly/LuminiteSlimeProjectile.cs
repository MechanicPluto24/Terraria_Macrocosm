using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utility;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.NPCs.Enemies.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Unfriendly
{
 	public class LuminiteSlimeProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Luminite Slime");
			ProjectileID.Sets.TrailCacheLength[Type] = 5;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public ref float AI_Timer => ref Projectile.ai[0]; 

		public int TargetPlayer
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.DrawTrail(Vector2.Zero, 4f, 1f, new Color(98, 211, 168, 255) * lightColor.GetLuminance(), new Color(98, 211, 168, 1));
			return true;
		}

		public override void AI()
		{
			float timeToShoot = 40;
			float baseShootSpeed = 12f;
			float shootDeviation = 0.5f;

			AI_Timer++;
			if (AI_Timer == timeToShoot)
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
					return;

				float aimAngle = (Main.player[TargetPlayer].Center - Projectile.Center).ToRotation();
				float shootSpeed = baseShootSpeed + Main.rand.NextFloat(-shootDeviation, shootDeviation);
				Projectile.velocity = MathUtils.PolarVector(shootSpeed, aimAngle);

				Projectile.netUpdate = true;
			}
 		}
	}
}
