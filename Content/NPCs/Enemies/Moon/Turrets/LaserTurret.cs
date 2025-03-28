using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon.Turrets
{
    public class LaserTurret : ModNPC
    {
        private static Asset<Texture2D> beam;
        private static Asset<Texture2D> turretTexture;

        public override string Texture => "Macrocosm/Content/NPCs/Enemies/Moon/Turrets/Turret_Base";

        public int AI_Timer
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public Vector2 turretHeight = new(0, -22);

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

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;

        public override void AI()
        {
            NPC.TargetClosest(faceTarget: true);
            Player player = Main.player[NPC.target];

            if (NPC.ai[0] == 0f)
            {
                Vector2 turningVector = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                if (AI_Timer < 170)
                    NPC.rotation = (new Vector2(5, 0).RotatedBy(NPC.rotation) + (turningVector * 0.3f)).ToRotation();

                bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                if (NPC.HasPlayerTarget && clearLineOfSight)
                {
                    if ((NPC.direction >= 0 && player.Center.X > NPC.Center.X) || (NPC.direction < 0 && player.Center.X <= NPC.Center.X))
                        AI_Timer++;

                    if (AI_Timer == 200)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + turretHeight, new Vector2(1f, 0).RotatedBy(NPC.rotation), ModContent.ProjectileType<LaserTurretProjectile>(), 100, 1f, Main.myPlayer, ai0: NPC.whoAmI);
                        NPC.ai[0] = 1f;
                        AI_Timer = 0;
                    }
                }
                else
                {
                    AI_Timer = 0;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
        }

        private void DrawBeam(SpriteBatch spriteBatch)
        {
            Vector2 beamStart = NPC.Center + turretHeight;

            Vector2 beamEnd = NPC.Center + turretHeight + (new Vector2(1, 0).RotatedBy(NPC.rotation) * Utility.CastLength(NPC.Center + turretHeight, new Vector2(1, 0).RotatedBy(NPC.rotation), 2000f, false));

            if (beamStart == beamEnd)
                return;

            beamStart -= Main.screenPosition;
            beamEnd -= Main.screenPosition;

            float rotation = (beamEnd - beamStart).ToRotation() + MathHelper.PiOver2;
            beam ??= ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Beam4");
            Vector2 scale = new Vector2(45f, Vector2.Distance(beamStart, beamEnd)) / beam.Size();
            Vector2 origin = new(beam.Width() * 0.5f, beam.Height());

            spriteBatch.Draw(beam.Value, beamStart, null, new Color(170, 0, 0, 0), rotation, origin, scale, SpriteEffects.None, 0f);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            turretTexture ??= ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Enemies/Moon/Turrets/Turret_Laser_Top");
            if (AI_Timer > 140)
                DrawBeam(spriteBatch);

            spriteBatch.Draw(turretTexture.Value, NPC.Center + turretHeight - Main.screenPosition, null, drawColor, NPC.rotation, turretTexture.Size() / 2, NPC.scale, NPC.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<PrintedCircuitBoard>(), 5));
            loot.Add(ItemDropRule.Common(ModContent.ItemType<Motor>(), 5));
            loot.Add(ItemDropRule.Common(ModContent.ItemType<Gear>(), 5));
            loot.Add(ItemDropRule.Common(ModContent.ItemType<Battery>(), 5));
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