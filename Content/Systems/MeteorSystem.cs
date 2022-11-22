using Macrocosm.Content.Projectiles.Meteors;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Content.Systems
{
	class MeteorSystem : ModSystem
	{
		public enum MeteorType
		{
			Small,
			Medium,
			Large
		}
		public static float MeteorBoostMoon = 1f;

		private double timePass = 0.0;

		public override void PostUpdateWorld()
		{
			if (SubworldSystem.IsActive<Moon>())
				UpdateMeteorsMoon();
		}

		private void UpdateMeteorsMoon()
		{
			// handled by server 
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return;

			timePass += Main.desiredWorldEventsUpdateRate;

			int closestPlayer = 0;

			for (int l = 1; l <= (int)timePass; l++)
			{
				// Dependent on world size
				float frequency = Main.maxTilesX / 4200f;

				frequency *= MeteorBoostMoon;

				if (!((float)Main.rand.Next(8000) < frequency))
					continue;

				Vector2 position = new((Main.rand.Next(Main.maxTilesX - 50) + 100) * 16, Main.rand.Next((int)((double)Main.maxTilesY * 0.05)) * 16);

				// 3/4 chance to spawn close to a player 
				if (!Main.rand.NextBool(4))
				{
					closestPlayer = Player.FindClosest(position, 1, 1);
					if ((double)Main.player[closestPlayer].position.Y < Main.worldSurface * 16.0 && Main.player[closestPlayer].afkCounter < 3600)
					{
						int offset = Main.rand.Next(1, 640);
						position.X = Main.player[closestPlayer].position.X + (float)Main.rand.Next(-offset, offset + 1);
					}
				}

				if (!Collision.SolidCollision(position, 16, 16))
				{
					float speedX = Main.rand.Next(-100, 101);
					float speedY = Main.rand.Next(200) + 100;
					float mult = 8 / (float)Math.Sqrt(speedX * speedX + speedY * speedY);
					speedX *= mult;
					speedY *= mult;

					WeightedRandom<int> choice = new(Main.rand);
					choice.Add(ModContent.ProjectileType<MoonMeteorSmall>(), 50.0);
					choice.Add(ModContent.ProjectileType<MoonMeteorMedium>(), 33.0);
					choice.Add(ModContent.ProjectileType<MoonMeteorLarge>(), 12.0);

					choice.Add(ModContent.ProjectileType<SolarMeteor>(), 2.0);
					choice.Add(ModContent.ProjectileType<NebulaMeteor>(), 2.0);
					choice.Add(ModContent.ProjectileType<StardustMeteor>(), 2.0);
					choice.Add(ModContent.ProjectileType<VortexMeteor>(), 2.0);

					var source = Main.player[closestPlayer].GetSource_Misc("FallingStar");

					int type = choice;
					int damage;

					if (type == ModContent.ProjectileType<MoonMeteorSmall>())
						damage = 500;
					else if (type == ModContent.ProjectileType<MoonMeteorMedium>())
						damage = 1000;
					else if (type == ModContent.ProjectileType<MoonMeteorLarge>())
						damage = 1500;
					else
						damage = 2000;

					Projectile.NewProjectile(source, position.X, position.Y, speedX, speedY, type, damage, 0f, 255); break;
				
				}
			}

			timePass %= 1.0;
		}

	}
}
