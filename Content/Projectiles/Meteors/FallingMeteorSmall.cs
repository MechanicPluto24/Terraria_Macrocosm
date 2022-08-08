using Macrocosm.Common.Utility;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Miscellaneous;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Meteors
{
	public class FallingMeteorSmall : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.damage = 500;
			Projectile.penetrate = 1;
			Projectile.tileCollide = true;
		}

		public override void Kill(int timeLeft)
		{
			// handled by server 
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				#region Spawn Items

				if (Main.rand.NextBool(3))
				{
					//int type = Utils.SelectRandom<int>(Main.rand, SomeGeode, SomeOtherGeode); -- maybe WeigthedRandom?
					int type = ModContent.ItemType<MoonGeode>();
					Vector2 position = new Vector2(Projectile.position.X, Projectile.position.Y - Projectile.height);
					int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), position, new Vector2(Projectile.width, Projectile.height), type);
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
				}

				#endregion

				#region Screenshake effect

				// let the server do it for every player

				float maxDist = 90f * 16f; // 90 tiles max distance (least screenshake) 
				float maxScreenshake = 40f; // max screenshake (up to 100) for distance = 0

				for (int i = 0; i < 255; i++)
				{

					Player player = Main.player[i];

					if (player.active)
					{
						float distance = Vector2.Distance(player.Center, Projectile.Center);

						if (distance < maxDist)
						{
							player.GetModPlayer<MacrocosmPlayer>().ScreenShakeIntensity = maxScreenshake - distance / maxDist * maxScreenshake;
						}

					}
				}

				#endregion
			}

			// handled by clients 
			if (Main.netMode != NetmodeID.Server)
			{
				#region Dusts

				for (int i = 0; i < Main.rand.Next(60, 80); i++)
				{
					Dust dust = Dust.NewDustDirect(
						new Vector2(Projectile.position.X, Projectile.position.Y + 1.5f * Projectile.height),
						Projectile.width,
						Projectile.height,
						ModContent.DustType<RegolithDust>(),
						Main.rand.NextFloat(-1f, 1f),
						Main.rand.NextFloat(0f, -5f),
						Scale: Main.rand.NextFloat(1.5f, 2f)
					);

					dust.noGravity = false;
				}

				#endregion

				#region Gores

				#endregion

				#region Sounds

				#endregion
			}
		}

		override public void AI()
		{
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
		}
	}
}
