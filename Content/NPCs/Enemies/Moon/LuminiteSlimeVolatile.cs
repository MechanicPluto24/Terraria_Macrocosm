using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Hostile;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class LuminiteSlimeVolatile : LuminiteSlime
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 650;
            NPC.damage = 80;
            NPC.defense = 70;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.SpawnTileY > Main.rockLayer && spawnInfo.SpawnTileType == ModContent.TileType<Protolith>()) ? 0.04f : 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            SpawnDusts(4);

            if (NPC.life <= 0)
                SpawnDusts(45);
        }

        public override bool PreAI()
        {
            Utility.AISlime(NPC, ref NPC.ai, false, false, 140, 5, -8, 6, -12);

            if (NPC.velocity.Y < 0f)
                NPC.velocity.Y += 0.25f;

            return true;
        }

        public override void AI()
        {
            base.AI();
        }

        public override void OnKill()
        {
            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, default, ModContent.ProjectileType<LuminiteExplosion>(), (int)(NPC.damage * 0.35f), 3, Main.myPlayer);

            for (int i = 0; i < 5; i++)
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, (-Vector2.UnitY).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(5f, 10f), ModContent.ProjectileType<LuminiteShard>(), (int)(NPC.damage * 0.25f), 1f, Main.myPlayer, ai1: -1, ai2: 1);

            SpawnDusts(20);
        }

        protected override void ProjectileAttack()
        {
        }
    }
}