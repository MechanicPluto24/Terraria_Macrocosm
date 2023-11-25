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
            BlastRadiusMultiplier = 3.5f;

            DustType = ModContent.DustType<RegolithDust>();
            ImpactDustCount = Main.rand.Next(100, 120);
            ImpactDustSpeed = new Vector2(2f, 7.5f);
            DustScaleMin = 1f;
            DustScaleMax = 1.4f;
            AI_DustChanceDenominator = 3;

            DebrisType = ModContent.ProjectileType<RegolithDebris>();
            DebrisCount = Main.rand.Next(4, 6);
            DebrisVelocity = new Vector2(0.5f, 0.7f);
        }

        public override void SpawnItems()
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
