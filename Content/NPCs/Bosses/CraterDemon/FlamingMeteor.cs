using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Bosses.CraterDemon
{
    //Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
    public class FlamingMeteor : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
            NPCID.Sets.TrailCacheLength[Type] = 15;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 28;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.lifeMax = 400;
            NPC.timeLeft = 600;
            NPC.damage = 200;
            NPC.knockBackResist=0.1f;
        }

        private float flashTimer;
        private float maxFlashTimer = 5;
        private bool spawned;
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;
        private Vector2 spawnPosition;
        private FlamingMeteorTrail trail;

        public override void AI()
        {
            if (!spawned)
            {
                trail = new();
                NPC.velocity = (-Vector2.UnitY).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(12f, 16f);
                spawned = true;
            }

            NPC.velocity.Y += 0.2f;
            if (NPC.velocity.Y > 24f)
                NPC.velocity.Y = 24f;

            if (NPC.velocity != Vector2.Zero)
                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

            Lighting.AddLight(NPC.Center, new Color(242, 142, 35).ToVector3());

            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, -NPC.velocity.X * 0.2f, -NPC.velocity.Y * 0.2f, 127, new Color(255, 255, 255), Main.rand.NextFloat(1.1f, 1.4f));
                dust.noGravity = true;
                dust.noLight = true;
            }

            flashTimer++;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, Main.rand.NextFloat(-10f, 11f), Main.rand.NextFloat(-10f, 11f), 127, new Color(255, 255, 255), Main.rand.NextFloat(1.1f, 1.4f));
                dust.noGravity = true;
                dust.noLight = true;
            }

            //(-Vector2.UnitY).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(12f, 16f)
            if (NPC.life <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {

                int cd = NPC.FindFirstNPC(ModContent.NPCType<CraterDemon>());
                if (cd < 0)
                {
                    NPC.velocity = NPC.velocity.RotatedByRandom(MathHelper.Pi / 8);
                }
                else
                {
                    float speed = NPC.velocity.Length();
                    Vector2 aim = (Main.npc[cd].position - NPC.Center).SafeNormalize(default);
                    NPC.velocity = NPC.velocity.RotatedBy(aim.ToRotation());
                }

                //NPC.life = 1;
                //NPC.friendly = true;
                // NPC.dontTakeDamage = true;
                //NPC.netUpdate = true;
                var explosion = Particle.CreateParticle<TintableExplosion>(p =>
                {
                    p.Position = NPC.Center;
                    p.DrawColor = (new Color(200, 120, 60)).WithOpacity(0.8f);
                    p.Scale = 1.5f;
                    p.NumberOfInnerReplicas = 8;
                    p.ReplicaScalingFactor = 0.4f;
                });
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = -NPC.direction;
            int frameSpeed = 5;

            NPC.frameCounter++;

            if (NPC.frameCounter >= frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y >= Main.npcFrameCount[Type] * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        private SpriteBatchState state;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            NPCID.Sets.TrailCacheLength[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 3;

            Rectangle sourceRect = NPC.frame;
            Vector2 origin = NPC.Size / 2f + new Vector2(6, 32);

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);


            if (flashTimer < maxFlashTimer)
            {
                Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Flare2").Value;
                float progress = flashTimer / maxFlashTimer;
                float scale = NPC.scale * progress * 0.85f;
                Vector2 position = spawnPosition + NPC.Size / 2f;
                float opacity = 1f;
                Main.spriteBatch.Draw(flare, position - screenPos, null, new Color(242, 142, 35).WithOpacity(opacity), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
            else
            {
                trail.Draw(NPC.oldPos, NPC.oldRot, NPC.Size / 2f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, sourceRect, (Color.White * (1f - NPC.alpha / 255f)).WithAlpha(65), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
