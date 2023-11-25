using Macrocosm.Common.Bases;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.MeteorChunks;
using Macrocosm.Content.Projectiles.Environment.Debris;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Meteors
{
    public class MoonMeteorLarge : BaseMeteor
    {
        public MoonMeteorLarge()
        {
            Width = 64;
            Height = 64;
            Damage = 1500;

            ScreenshakeMaxDist = 140f * 16f;
            ScreenshakeIntensity = 100f;

            RotationMultiplier = 0.01f;
            BlastRadiusMultiplier = 3.5f;

            DustType = ModContent.DustType<RegolithDust>();
            ImpactDustCount = Main.rand.Next(140, 160);
            ImpactDustSpeed = new Vector2(3f, 10f);
            DustScaleMin = 1f;
            DustScaleMax = 1.6f;
            AI_DustChanceDenominator = 2;

            DebrisType = ModContent.ProjectileType<RegolithDebris>();
            DebrisCount = Main.rand.Next(6, 8);
            DebrisVelocity = new Vector2(0.5f, 0.8f);
        }

        public override void SpawnItems()
        {
            // can spawn up to two geodes
            for (int i = 0; i < 2; i++)
            {
                if (Main.rand.NextBool(3))
                {
                    int type = ModContent.ItemType<MeteoricChunk>();
                    Vector2 position = new Vector2(Projectile.position.X + Width / 2, Projectile.position.Y - Height);
                    int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), position, new Vector2(Projectile.width, Projectile.height), type);
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
                }
            }
        }
    }
}
