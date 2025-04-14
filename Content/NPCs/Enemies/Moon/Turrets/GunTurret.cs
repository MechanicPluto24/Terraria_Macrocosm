using Macrocosm.Common.Sets;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Tech;
using Macrocosm.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon.Turrets
{
    public class GunTurret : ModNPC
    {
        public override string Texture => base.Texture.Replace(Name, "TurretBase");
        public string GunTexture => base.Texture;
        private static Asset<Texture2D> gunTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = false;
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 40;
            NPC.damage = 40;
            NPC.defense = 50;
            NPC.lifeMax = 2000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
        }

        public int AI_Timer
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        public Vector2 TurretHeight { get; } = new(0, -18);
        public float TurretRotation { get; set; } = 0f;

        public override void AI()
        {
            NPC.TargetClosest(faceTarget: true);
            Player player = Main.player[NPC.target];
            Vector2 turningVector = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            TurretRotation = (new Vector2(5, 0).RotatedBy(TurretRotation) + (turningVector * 0.6f)).ToRotation();

            bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
            if (NPC.HasPlayerTarget && clearLineOfSight)
            {
                if ((NPC.direction >= 0 && player.Center.X > NPC.Center.X) || (NPC.direction < 0 && player.Center.X <= NPC.Center.X))
                    AI_Timer++;

                if (AI_Timer == 60)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + TurretHeight, new Vector2(25f, 0).RotatedBy(TurretRotation), ModContent.ProjectileType<ZombieSecurityBullet>(), 100, 1f, Main.myPlayer);
                }

                if (AI_Timer == 70)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + TurretHeight, new Vector2(25f, 0).RotatedBy(TurretRotation), ModContent.ProjectileType<ZombieSecurityBullet>(), 100, 1f, Main.myPlayer);
                }

                if (AI_Timer == 80)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + TurretHeight, new Vector2(25f, 0).RotatedBy(TurretRotation), ModContent.ProjectileType<ZombieSecurityBullet>(), 100, 1f, Main.myPlayer);
                    AI_Timer = 0;
                }
            }
            else
            {
                AI_Timer = 0;
            }

        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            gunTexture ??= ModContent.Request<Texture2D>(GunTexture);
            spriteBatch.Draw(gunTexture.Value, NPC.Center + TurretHeight - Main.screenPosition + new Vector2(NPC.direction * 5, 0), null, drawColor, TurretRotation, gunTexture.Size() / 2, NPC.scale, NPC.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<PrintedCircuitBoard>(), 2));
            loot.Add(ItemDropRule.Common(ModContent.ItemType<Motor>(), 2));
            loot.Add(ItemDropRule.Common(ModContent.ItemType<Gear>(), 2));
            loot.Add(ItemDropRule.Common(ModContent.ItemType<Battery>(), 2));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<IndustrialPlatingDust>());
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
    }
}