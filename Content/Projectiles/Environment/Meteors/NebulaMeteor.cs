using Macrocosm.Common.Bases;
using Macrocosm.Content.Items.MeteorChunks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Meteors
{
    public class NebulaMeteor : BaseMeteor
    {
        public NebulaMeteor()
        {
            Width = 52;
            Height = 44;
            Damage = 1500;

            ScreenshakeMaxDist = 140f * 16f;
            ScreenshakeIntensity = 100f;

            RotationMultiplier = 0.01f;
            BlastRadiusMultiplier = 3.5f;

            DustType = 71; // Nebula Blaze dust
            ImpactDustCount = Main.rand.Next(140, 160);
            ImpactDustSpeed = new Vector2(3f, 10f);
            DustScaleMin = 1f;
            DustScaleMax = 1.6f;
            AI_DustChanceDenominator = 1;
        }

        public override void SpawnItems()
        {
            int type = ModContent.ItemType<NebulaChunk>();
            Vector2 position = new Vector2(Projectile.position.X + Width / 2, Projectile.position.Y - Height);
            int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), position, new Vector2(Projectile.width, Projectile.height), type);
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
        }

        //public override void AI_SpawnDusts()
        //{
        //	int dustType = Main.rand.Next(71, 74); // Nebula Dusts: 71, 72, 73
        //	AI_SpawnDusts(dustType);
        //}
    }
}
