using Macrocosm.Common.Bases;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Items.MeteorChunks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Meteors
{
    public class StardustMeteor : BaseMeteor
    {
        public StardustMeteor()
        {
            Width = 52;
            Height = 44;
            Damage = 1500;

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
            SpawnImpactDusts(DustID.YellowStarDust, noGravity: false);

            for (int i = 0; i < Main.rand.Next(30, 50); i++)
            {
                Vector2 position = Projectile.Center + new Vector2(Width, Height).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat();
                Vector2 velocity = new(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(0f, -20f));

                Particle.CreateParticle(ParticleOrchestraType.StardustPunch, position, velocity);
            }
        }
    }
}
