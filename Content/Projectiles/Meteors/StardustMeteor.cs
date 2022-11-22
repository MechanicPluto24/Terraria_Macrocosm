using Macrocosm.Common.Utility;
using Macrocosm.Content.Items.Chunks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Meteors
{
	public class StardustMeteor : BaseMeteor
	{
		public StardustMeteor()
		{
			Width = 52;
			Height = 44;
			Damage = 1500;

			DisplayName = "Stardust Meteor";

			ScreenshakeMaxDist = 140f * 16f;
			ScreenshakeIntensity = 100f;

			RotationMultiplier = 0.01f;
			BlastRadiusMultiplier = 3.5f;

			DustType = DustID.YellowStarDust;
			ImpactDustCount = Main.rand.Next(70, 80);
			ImpactDustSpeed = new Vector2(3f, 10f);
			DustScaleMin = 1f;
			DustScaleMax = 1.6f;
			AI_DustChanceDenominator = 1;
		}

		public override void SpawnItems()
		{
			int type = ModContent.ItemType<StardustChunk>();
			Vector2 position = new Vector2(Projectile.position.X + Width / 2, Projectile.position.Y - Height);
			int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), position, new Vector2(Projectile.width, Projectile.height), type);
			NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
 		}

		public override void AI_SpawnDusts()
		{
			int dustType = Main.rand.NextFromList(DustID.YellowStarDust, DustID.DungeonWater);
			AI_SpawnDusts(dustType);
		}

		public override void SpawnImpactDusts()
		{
 			SpawnImpactDusts(DustID.YellowStarDust);
			SpawnImpactDusts(DustID.DungeonWater, noGravity: true);

			ParticleOrchestraSettings settings;

			for(int i = 0; i < Main.rand.Next(30, 40); i++)
			{
				settings = new()
				{
					PositionInWorld = Projectile.Center + new Vector2(Width, Height).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(),
					MovementVector = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 10f
				};

				ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.StardustPunch, settings);
			}
 		}
	}
}
