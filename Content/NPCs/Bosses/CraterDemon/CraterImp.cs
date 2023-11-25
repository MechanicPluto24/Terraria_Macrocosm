using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.NPCs.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Bosses.CraterDemon
{
    public class CraterImp : ModNPC, IMoonEnemy
    {
        public ref float AI_Timer => ref NPC.ai[0];
        public ref float AI_Attack => ref NPC.ai[1];
        public ref float AI_AttackProgress => ref NPC.ai[2];
        public int ParentBoss => (int)NPC.ai[3];

        public const int Despawning = -3;
        public const int Spawning = -2;
        public const int Wait = -1;
        public const int FloatTowardPlayer = 0;
        public const int ChargeAtPlayer = 1;
        public const int Chomp = 2;
        public const int FadeOut = 3;

        private int targetFrame;
        private bool spawned;
        private float targetAlpha;

        private int chargeTicks;

        public const int WaitTime = 4 * 60;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCID.Sets.ImmuneToRegularBuffs[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new()
            {
                Position = new Vector2(0f, 4f),
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 56;
            NPC.lifeMax = 6000;
            NPC.defense = 60;
            NPC.damage = 80;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.aiStyle = -1;

            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<CraterDemon>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = targetFrame * frameHeight;

            if (NPC.IsABestiaryIconDummy)
                CycleAnimation();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.expertMode)
                target.AddBuff(ModContent.BuffType<SuitBreach>(), 80);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                // This is required because we have NPC.alpha = 255, in the bestiary it would look transparent
                return NPC.GetBestiaryEntryColor();
            }
            return drawColor * (1f - targetAlpha / 255f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 vector, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Bosses/CraterDemon/CraterImp_Glow").Value;

            Color color = GetAlpha(drawColor) ?? Color.White;

            SpriteEffects effect = (NPC.rotation > MathHelper.PiOver2 && NPC.rotation < 3 * MathHelper.PiOver2) || (NPC.rotation < -MathHelper.PiOver2 && NPC.rotation > -3 * MathHelper.PiOver2)
                ? SpriteEffects.FlipVertically
                : SpriteEffects.None;

            if (AI_Attack == ChargeAtPlayer && chargeTicks > 0)
            {
                int length = Math.Min(chargeTicks, NPC.oldPos.Length);
                for (int i = 0; i < length; i++)
                {
                    Vector2 drawPos = NPC.oldPos[i] - Main.screenPosition + NPC.Size / 2f;

                    Color trailColor = color * (((float)NPC.oldPos.Length - i) / NPC.oldPos.Length);
                    spriteBatch.Draw(texture, drawPos, NPC.frame, trailColor * 0.6f, NPC.rotation, NPC.Size / 2f, NPC.scale, effect, 0f);

                    // trailing glowmask (behind the trail and the npc)
                    Color glowColor = (Color)(GetAlpha(Color.White) * (((float)NPC.oldPos.Length - i) / NPC.oldPos.Length));
                    spriteBatch.Draw(glowmask, drawPos, NPC.frame, glowColor * 0.6f, NPC.rotation, NPC.Size / 2f, NPC.scale, effect, 0f);
                }
            }

            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, color, NPC.rotation, NPC.Size / 2f, NPC.scale, effect, 0);

            if (NPC.IsABestiaryIconDummy)
                return true;

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return;

            Texture2D glowmask = ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Bosses/CraterDemon/CraterImp_Glow").Value;

            SpriteEffects effect = (NPC.rotation > MathHelper.PiOver2 && NPC.rotation < 3 * MathHelper.PiOver2) || (NPC.rotation < -MathHelper.PiOver2 && NPC.rotation > -3 * MathHelper.PiOver2)
                ? SpriteEffects.FlipVertically
                : SpriteEffects.None;

            spriteBatch.Draw(glowmask, NPC.Center - Main.screenPosition, NPC.frame, (Color)GetAlpha(Color.White), NPC.rotation, NPC.Size / 2f, NPC.scale, effect, 0f);
        }

        public override void AI()
        {
            if (AI_Attack != Despawning && !Main.npc[ParentBoss].active || Main.npc[ParentBoss].type != ModContent.NPCType<CraterDemon>())
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
            }

            if (!spawned)
            {
                spawned = true;

                AI_Attack = Spawning;
                AI_Timer = 2 * 60;
                AI_AttackProgress = 0;
                targetAlpha = 255f;

                NPC.TargetClosest();

                NPC.spriteDirection = 1;
            }

            Player player = NPC.target >= 0 && NPC.target < Main.maxPlayers ? Main.player[NPC.target] : null;

            if (AI_Attack == Wait && !(NPC.target < 0 || NPC.target >= Main.maxPlayers || player.dead || !player.active))
            {
                //Chase the new player
                AI_Attack = FloatTowardPlayer;
                AI_Timer = WaitTime;
            }

            switch (AI_Attack)
            {
                case Despawning:

                    NPC.velocity *= 1f - 3f / 60f;
                    targetAlpha += 255f / (2 * 60);
                    SpawnDusts();

                    if (targetAlpha > 255)
                        NPC.active = false;

                    targetFrame = 0;
                    NPC.frameCounter = 0;
                    break;


                case Spawning:
                    targetAlpha -= 255f / (2 * 60);

                    SpawnDusts();

                    if (targetAlpha <= 0)
                    {
                        targetAlpha = 0;

                        AI_Attack = Wait;
                    }

                    targetFrame = 0;
                    NPC.frameCounter = 0;
                    break;

                case Wait:
                    //Player is dead/not connected?  Target a new one
                    if (NPC.target < 0 || NPC.target >= Main.maxPlayers || player.dead || !player.active)
                    {
                        NPC.velocity *= 1f - 5f / 60f;

                        if (Math.Abs(NPC.velocity.X) < 0.02f)
                            NPC.velocity.X = 0;
                        if (Math.Abs(NPC.velocity.Y) < 0.02f)
                            NPC.velocity.Y = 0;

                        NPC.TargetClosest();
                    }

                    CycleAnimation();
                    break;

                case FloatTowardPlayer:
                    MoveTowardTargetPlayer(player);

                    AdjustRotation(player);

                    if (AI_Timer <= 0 && targetFrame == 0)
                    {
                        AI_Attack = ChargeAtPlayer;
                        AI_Timer = (int)(1.25f * 60);
                        AI_AttackProgress = 0;
                    }
                    else
                        CycleAnimation();

                    break;

                case ChargeAtPlayer:
                    //Wait until mouth is open
                    if (AI_AttackProgress == 0)
                    {
                        chargeTicks = 0;

                        if (targetFrame != 2)
                        {
                            AI_Timer++;

                            CycleAnimation();

                            MoveTowardTargetPlayer(player);

                            AdjustRotation(player);
                        }
                        else
                        {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = 1.1f, Volume = 0.5f }, NPC.Center);

                            Vector2 dir = NPC.DirectionTo(player.Center);
                            NPC.rotation = dir.ToRotation();

                            NPC.velocity = dir * (Main.expertMode ? 25f : 15f);

                            AI_AttackProgress++;
                        }
                    }
                    else
                        chargeTicks++;

                    if (AI_Timer <= 0)
                    {
                        AI_Attack = Chomp;
                        AI_Timer = 20;
                        AI_AttackProgress = 0;
                    }
                    break;

                case Chomp:
                    NPC.velocity *= 1f - 5f / 60f;

                    if (targetFrame != 0)
                        CycleAnimation();
                    else if (AI_Timer <= 0)
                    {
                        AI_Attack = Wait;
                        AI_AttackProgress = 0;
                    }
                    break;
            }

            AI_Timer--;

            NPC.alpha = (int)targetAlpha;

            NPC.spriteDirection = NPC.velocity.X > 0
                ? 1
                : (NPC.velocity.X < 0
                    ? -1
                    : NPC.spriteDirection);
        }

        private void MoveTowardTargetPlayer(Player player)
        {
            const float inertia = 60f;
            float speedX = Main.expertMode ? 14f : 8f;
            float speedY = speedX * 0.5f;

            const float minDistance = 5 * 16;
            if (NPC.DistanceSQ(player.Center) >= minDistance * minDistance)
            {
                Vector2 direction = NPC.DirectionTo(player.Center) * speedX;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

                if (NPC.velocity.X < -speedX)
                    NPC.velocity.X = -speedX;
                else if (NPC.velocity.X > speedX)
                    NPC.velocity.X = speedX;

                if (NPC.velocity.Y < -speedY)
                    NPC.velocity.Y = -speedY;
                else if (NPC.velocity.Y > speedY)
                    NPC.velocity.Y = speedY;
            }
        }

        private void AdjustRotation(Player player)
        {
            float NPCRotation = NPC.rotation;
            float targetRotation = NPC.DirectionTo(player.Center).ToRotation();

            //Prevent spinning
            if (NPCRotation - targetRotation > MathHelper.Pi)
                targetRotation += MathHelper.TwoPi;
            else if (targetRotation - NPCRotation > MathHelper.Pi)
                NPCRotation += MathHelper.TwoPi;

            NPC.rotation = MathHelper.Lerp(NPCRotation, targetRotation, 4.3f / 60f);
        }

        private void CycleAnimation()
        {
            if (++NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                targetFrame = ++targetFrame % Main.npcFrameCount[NPC.type];
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            CraterDemon.SpawnDustsInner(NPC.position, NPC.width, NPC.height, ModContent.DustType<RegolithDust>());

            if (Main.dedServ)
                return;

            var entitySource = NPC.GetSource_Death();

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 15; i++)
                    CraterDemon.SpawnDustsInner(NPC.position, NPC.width, NPC.height, ModContent.DustType<RegolithDust>());

                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("CraterImpGoreFace").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("CraterImpGoreHead").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("CraterImpGoreJaw").Type);
            }
        }

        private void SpawnDusts()
        {
            for (int i = 0; i < 4; i++)
                CraterDemon.SpawnDustsInner(NPC.position, NPC.width, NPC.height, ModContent.DustType<RegolithDust>());
        }
    }
}
