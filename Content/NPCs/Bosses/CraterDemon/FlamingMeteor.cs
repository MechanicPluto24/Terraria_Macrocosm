using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
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

            NPCID.Sets.ProjectileNPC[Type] = true;

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.TrailCacheLength[Type] = 20;
            NPCID.Sets.TrailingMode[Type] = 3;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 28;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.lifeMax = 400;
            NPC.timeLeft = 600;
            NPC.damage = 50;
            NPC.knockBackResist = 0.1f;
            NPC.chaseable = false;
            trail = new(new Color(242, 142, 35, 65), 30);
            trail2 = new(new Color(30, 255, 105, 0), 10);
        }

        private float flashTimer;
        private float maxFlashTimer = 5;
        private bool spawned;
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;
        private Vector2 spawnPosition;
        private FlamingMeteorTrail trail;
        private FlamingMeteorTrail trail2;

        public override void AI()
        {
            if (!spawned)
            {
                NPC.velocity = (-Vector2.UnitY).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(12f, 16f);
                spawnPosition = NPC.position;
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                spawned = true;
            }

            NPC.velocity.Y += 0.2f;
            if (NPC.velocity.Y > 24f)
                NPC.velocity.Y = 24f;

            if (NPC.velocity != Vector2.Zero)
                NPC.rotation = NPC.velocity.ToRotation();

            Lighting.AddLight(NPC.Center, new Color(242, 142, 35).ToVector3());

            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, -NPC.velocity.X * 0.2f, -NPC.velocity.Y * 0.2f, 127, new Color(255, 255, 255), Main.rand.NextFloat(1.1f, 1.4f));
                dust.noGravity = true;
                dust.noLight = true;
            }

            flashTimer++;
            NPC.netUpdate = true;
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
                Particle.Create<TintableExplosion>(p =>
                {
                    p.Position = NPC.Center;
                    p.Color = (new Color(242, 142, 35, 65));
                    p.Scale = new(1.5f);
                    p.NumberOfInnerReplicas = 8;
                    p.ReplicaScalingFactor = 0.4f;
                });

                Particle.Create<TintableExplosion>(p =>
               {
                   p.Position = NPC.Center;
                   p.Color = (new Color(30, 255, 105, 65));
                   p.Scale = new(1.2f);
                   p.NumberOfInnerReplicas = 6;
                   p.ReplicaScalingFactor = 0.2f;
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
            Rectangle sourceRect = NPC.frame;
            Vector2 origin = NPC.Size / 2f + new Vector2(6, 32);

            Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, sourceRect, (Color.White.WithAlpha(65) * NPC.Opacity), NPC.rotation - MathHelper.PiOver2, origin, NPC.scale, SpriteEffects.None, 0);

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            if (flashTimer < maxFlashTimer)
            {
                Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Flare2").Value;
                float progress = flashTimer / maxFlashTimer;
                float scale = NPC.scale * progress * 0.85f;
                Vector2 position = spawnPosition + NPC.Size / 2f;
                Main.EntitySpriteDraw(flare, position - screenPos, null, new Color(242, 142, 35), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);
            }

            trail?.Draw(NPC, NPC.Size / 2f);
            trail2?.Draw(NPC, NPC.Size / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }
    }
}
