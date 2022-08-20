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
	public class MoonMeteorMedium : BaseMeteor
	{
		public MoonMeteorMedium()
		{
			Width = 48;
			Height = 48;
			Damage = 1000;

			ScreenshakeMaxDist = 110f * 16f;
			ScreenshakeIntensity = 75f;

			RotationMultiplier = 0.01f;
			BlastRadiusMultiplier = 2;

			DustType = ModContent.DustType<RegolithDust>();
			ImpactDustCount = Main.rand.Next(100, 120);
			ImpactDustSpeed = new Vector2(2f, 7.5f);
			ImpactDustScaleMin = 1f;
			ImpactDustScaleMax = 1.4f;
			AiDustChanceDenominator = 3;

			GoreType = ModContent.GoreType<RegolithDebris>();
			GoreCount = Main.rand.Next(4, 6);
			GoreVelocity = new Vector2(0.5f, 0.7f);
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
