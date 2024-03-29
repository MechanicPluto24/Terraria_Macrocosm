using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.NPCs.Global;
using Macrocosm.Content.Projectiles.Hostile;
using Macrocosm.Content.Tiles.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class LuminiteSlime : ModNPC, IMoonEnemy
    {
        public static Color EffectColor => new Color(92, 228, 162);
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 36;
            NPC.height = 22;
            NPC.damage = 80;
            NPC.defense = 64;
            NPC.lifeMax = 550;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            AnimationType = NPCID.BlueSlime;

            SpawnModBiomes = [ModContent.GetInstance<UndergroundMoonBiome>().Type];
        }

        protected readonly float attackTime = 280f;
        public float AI_AttackTimer;

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.SpawnTileY > Main.rockLayer && spawnInfo.SpawnTileType == ModContent.TileType<Protolith>()) ? 0.1f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<SpaceDust>()));
            loot.Add(ItemDropRule.Common(ItemID.LunarOre, 1, 3, 13));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            SpawnDusts(3);

            if (NPC.life <= 0)
                SpawnDusts(15);
        }

        public override bool PreAI()
        {
            Utility.AISlime(NPC, ref NPC.ai, false, false, 175, 3, -8, 4, -10);

            if (NPC.velocity.Y < 0f)
                NPC.velocity.Y += 0.15f;

            return true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(25))
                SpawnDusts();

            if (!NPC.HasPlayerTarget)
                return;

            if (AI_AttackTimer++ > attackTime && NPC.velocity == Vector2.Zero)
            {
                Player target = Main.player[NPC.target];
                bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);

                if (clearLineOfSight && target.active && !target.dead)
                {
                    // successful attack, reset counter 
                    ProjectileAttack();
                    AI_AttackTimer = 0f;
                }
            }
        }

        protected virtual void ProjectileAttack()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.rand.Next(3, 7); i++)
                {
                    Vector2 projVelocity = Utility.PolarVector(2.6f, Main.rand.NextFloat(-MathHelper.Pi + MathHelper.PiOver4, -MathHelper.PiOver4));
                    Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<LuminiteShard>(), Utility.TrueDamage((int)(NPC.damage * 1.35f)), 1f, Main.myPlayer, ai1: NPC.target);
                    proj.netUpdate = true;
                }
            }

            SpawnDusts(5);
        }

        protected virtual void SpawnDusts(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<LuminiteDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
            }
        }
    }
}