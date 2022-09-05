﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
	public abstract class PortalDust : ModDust
	{
		public override string Texture => "Macrocosm/Content/Dusts/PortalDust";

		public override void OnSpawn(Dust dust)
		{
			dust.noLight = true; 
		}
	}

	public class PortalLightGreenDust : PortalDust
	{

		public override void OnSpawn(Dust dust)
		{
			base.OnSpawn(dust);
			dust.frame = new Rectangle(0, Main.rand.Next(0, 2) * 10, 10, 10);
		}

		public override bool Update(Dust dust)
		{
			if (!dust.noGravity)
				dust.velocity.Y += 0.05f;

			if (dust.customData != null && dust.customData is NPC npc)
			{
				dust.position += npc.position - npc.oldPos[1];
			}
			else if (dust.customData != null && dust.customData is Player player)
			{
				dust.position += player.position - player.oldPosition;
			}
			else if (dust.customData != null && dust.customData is Vector2)
			{
				Vector2 vector = (Vector2)dust.customData - dust.position;
				if (vector != Vector2.Zero)
					vector.Normalize();

				dust.velocity = (dust.velocity * 4f + vector * dust.velocity.Length()) / 5f;
			}

			dust.position += dust.velocity;
			dust.rotation += 0.1f * (dust.dustIndex % 2 == 0 ? -1 : 1);
			dust.scale -= 0.08f;

			float lightMultiplier = 0.35f * dust.scale;

			Lighting.AddLight(dust.position, 0.61f * lightMultiplier, 0.26f * lightMultiplier, 0.85f * lightMultiplier);

			if (dust.scale <= 0f)
			{
				dust.active = false;
			}

			return false;
		}
	}

	public class PortalLightOrangeDust : PortalDust
	{
		public override void OnSpawn(Dust dust)
		{
			base.OnSpawn(dust);
			dust.frame = new Rectangle(10, Main.rand.Next(0, 2) * 10, 10, 10);
		}

		public override bool Update(Dust dust)
		{
			if (dust.customData != null && dust.customData is NPC npc)
			{
				dust.position += npc.position - npc.oldPos[1];
			}
			else if (dust.customData != null && dust.customData is Player player)
			{
				dust.position += player.position - player.oldPosition;
			}
			else if (dust.customData != null && dust.customData is Vector2)
			{
				Vector2 vector = (Vector2)dust.customData - dust.position;
				if (vector != Vector2.Zero)
					vector.Normalize();

				dust.velocity = (dust.velocity * 4f + vector * dust.velocity.Length()) / 5f;
			}

			dust.position += dust.velocity;
			dust.rotation += 0.1f * (dust.dustIndex % 2 == 0 ? -1 : 1);
			dust.scale -= 0.08f;

			float lightMultiplier = 0.35f * dust.scale;

			Lighting.AddLight(dust.position, 0.61f * lightMultiplier, 0.26f * lightMultiplier, 0.85f * lightMultiplier);

			if (dust.scale <= 0f)
			{
				dust.active = false;
			}

			return false;
		}
	}

	public class PortalDarkGreenDust : PortalDust
	{

		public override void OnSpawn(Dust dust)
		{
			base.OnSpawn(dust);
			dust.frame = new Rectangle(20, Main.rand.Next(0, 2) * 10, 10, 10);
		}
		public override bool Update(Dust dust)
		{
			if (dust.customData != null && dust.customData is Projectile projectile)
			{
				if (projectile.active)
					dust.position += projectile.position - projectile.oldPosition;
			}
			else if (dust.customData != null && dust.customData is Vector2)
			{
				Vector2 vector = (Vector2)dust.customData - dust.position;
				if (vector != Vector2.Zero)
					vector.Normalize();

				dust.velocity = (dust.velocity * 4f + vector * dust.velocity.Length()) / 5f;
			}

			dust.position += dust.velocity;
			dust.rotation += 0.35f * (dust.dustIndex % 2 == 0 ? -1 : 1);
			dust.scale -= 0.1f;

			float lightMultiplier = 0.35f * dust.scale;

			Lighting.AddLight(dust.position, 0.61f * lightMultiplier, 0.26f * lightMultiplier, 0.85f * lightMultiplier);

			if (dust.scale <= 0.01f)
			{
				dust.active = false;
			}

			return false;
		}
	}

	public class PortalDarkOrangeDust : PortalDust
	{
		public override void OnSpawn(Dust dust)
		{
			base.OnSpawn(dust);
			dust.frame = new Rectangle(30, Main.rand.Next(0, 2) * 10, 10, 10);
		}

		public override bool Update(Dust dust)
		{
			if (dust.customData != null && dust.customData is Projectile projectile)
			{
				if (projectile.active)
					dust.position += projectile.position - projectile.oldPosition;
			}
			else if (dust.customData != null && dust.customData is Vector2)
			{
				//Vector2 vector = (Vector2)dust.customData - dust.position;
				//if (vector != Vector2.Zero)
				//	vector.Normalize();
				//
				//dust.velocity = (dust.velocity * 4f + vector * dust.velocity.Length()) / 5f;

				dust.position += (Vector2)dust.customData;
			}

			dust.position += dust.velocity;
			dust.rotation += 0.35f * (dust.dustIndex % 2 == 0 ? -1 : 1);
			dust.scale -= 0.1f;

			float lightMultiplier = 0.35f * dust.scale;

			Lighting.AddLight(dust.position, 0.61f * lightMultiplier, 0.26f * lightMultiplier, 0.85f * lightMultiplier);

			if (dust.scale <= 0.1f)
			{
				dust.active = false;
			}

			return false;
		}
	}
}