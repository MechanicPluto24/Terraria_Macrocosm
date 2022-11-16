using Macrocosm.Common.Utility;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Gores;
using Macrocosm.Content.Items.Miscellaneous;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Meteors
{
	public class MoonMeteorLarge : BaseMeteor
	{
		public MoonMeteorLarge()
		{
			Width = 64;
			Height = 64;
			Damage = 1500;

			DisplayName = "Large Meteor";

			ScreenshakeMaxDist = 140f * 16f;
			ScreenshakeIntensity = 100f;

			RotationMultiplier = 0.01f;
			BlastRadiusMultiplier = 3.5f;

			DustType = ModContent.DustType<RegolithDust>();
			ImpactDustCount = Main.rand.Next(140, 160);
			ImpactDustSpeed = new Vector2(3f, 10f);
			ImpactDustScaleMin = 1f;
			ImpactDustScaleMax = 1.6f;
			AiDustChanceDenominator = 2;

			GoreType = ModContent.GoreType<RegolithDebris>();
			GoreCount = Main.rand.Next(6, 8);
			GoreVelocity = new Vector2(0.5f, 0.8f);
		}

		public override void SpawnItems()
		{
			// can spawn up to two geodes
			for (int i = 0; i < 2; i++)
			{
				if (Main.rand.NextBool(3))
				{
					int type = ModContent.ItemType<MeteoricChunk>();
					Vector2 position = new Vector2(Projectile.position.X, Projectile.position.Y - Projectile.height);
					int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), position, new Vector2(Projectile.width, Projectile.height), type);
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
				}
			}
		}
	}
}
