using Macrocosm.Content.Projectiles.Meteors;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Content
{
	class MeteorSystem : ModSystem
	{
		public enum MeteorType
		{
			Small,
			Medium,
			Large
		}
		public static float MeteorBoostMoon { get => meteorBoost; set => meteorBoost = value; }

		private static float meteorBoost = 1f;
		private double timePass = 0.0;

		public override void PostUpdateWorld()
		{
			if (SubworldSystem.IsActive<Moon>())
			{
				UpdateMeteorsMoon();
			}
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

				if (!((float)Main.rand.Next(8000) < 10f * frequency))
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

					WeightedRandom<MeteorType> choice = new(Main.rand);
					choice.Add(MeteorType.Small, 6);
					choice.Add(MeteorType.Medium, 3);
					choice.Add(MeteorType.Large, 1);

					var source = Projectile.GetSource_NaturalSpawn();

					switch ((MeteorType)choice)
					{
						case MeteorType.Small: Projectile.NewProjectile(source, position.X, position.Y, speedX, speedY, ModContent.ProjectileType<FallingMeteorSmall>(), 500, 0f, closestPlayer); break;
						case MeteorType.Medium: Projectile.NewProjectile(source, position.X, position.Y, speedX, speedY, ModContent.ProjectileType<FallingMeteorMedium>(), 750, 0f, closestPlayer); break;
						case MeteorType.Large: Projectile.NewProjectile(source, position.X, position.Y, speedX, speedY, ModContent.ProjectileType<FallingMeteorLarge>(), 1000, 0f, closestPlayer); break;
					}
				}
			}

			timePass %= 1.0;
		}

	}
}
