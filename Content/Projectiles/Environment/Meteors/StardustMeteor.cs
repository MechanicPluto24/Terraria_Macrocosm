using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Items.GrabBags;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Meteors
{
    public class StardustMeteor : BaseMeteor
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 64;
            Projectile.height = 64;

            ScreenshakeMaxDist = 140f * 16f;
            ScreenshakeIntensity = 100f;

            RotationMultiplier = 0.01f;
            BlastRadius = 224;
        }

        public override void MeteorAI()
        {
            float DustScaleMin = 1f;
            float DustScaleMax = 1.6f;

            if (Main.rand.NextBool(1))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    Main.rand.NextBool() ? DustID.YellowStarDust : DustID.DungeonWater,
                    0f,
                    0f,
                    Scale: Main.rand.NextFloat(DustScaleMin, DustScaleMax)
                );

                dust.noGravity = true;
            }
        }

        public override void ImpactEffects()
        {
            int ImpactDustCount = Main.rand.Next(140, 160) * 2;
            Vector2 ImpactDustSpeed = new Vector2(3f, 10f);
            float DustScaleMin = 1f;
            float DustScaleMax = 1.6f;

            for (int i = 0; i < ImpactDustCount; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    i % 2 == 0 ? DustID.YellowStarDust : DustID.DungeonWater,
                    Main.rand.NextFloat(-ImpactDustSpeed.X, ImpactDustSpeed.X),
                    Main.rand.NextFloat(0f, -ImpactDustSpeed.Y),
                    Scale: Main.rand.NextFloat(DustScaleMin, DustScaleMax)
                );

                dust.noGravity = true;
            }

            for (int i = 0; i < Main.rand.Next(30, 50); i++)
            {
                Vector2 position = Projectile.Center + new Vector2(30).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat();
                Vector2 velocity = new Vector2(20).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat();
                Particle.Create(ParticleOrchestraType.StardustPunch, position, velocity);
            }
        }

        public override void SpawnItems()
        {
            int type = ModContent.ItemType<StardustChunk>();
            int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Center,  type);
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
        }
    }
}
