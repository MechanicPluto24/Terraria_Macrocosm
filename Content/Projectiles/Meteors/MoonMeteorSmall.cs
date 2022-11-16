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
	public class MoonMeteorSmall : BaseMeteor
	{
		public MoonMeteorSmall()
		{
			Width = 32;
			Height = 32;
			Damage = 500;

			DisplayName = "Small Meteor";

			ScreenshakeMaxDist = 90f * 16f;
			ScreenshakeIntensity = 50f;

			RotationMultiplier = 0.01f;
			BlastRadiusMultiplier = 2;

			DustType = ModContent.DustType<RegolithDust>();
			ImpactDustCount = Main.rand.Next(60, 80);
			ImpactDustSpeed = new Vector2(1f, 5f);
			ImpactDustScaleMin = 1f;
			ImpactDustScaleMax = 1.2f;
			AiDustChanceDenominator = 4;

			GoreType = ModContent.GoreType<RegolithDebris>();
			GoreCount = Main.rand.Next(2, 4);
			GoreVelocity = new Vector2(0.5f, 0.6f);
		}

		public override void SpawnItems()
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
