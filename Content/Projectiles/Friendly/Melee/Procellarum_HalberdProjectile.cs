using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Debuffs.Weapons;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class Procellarum_HalberdProjectile : HalberdProjectile
    {
        public enum ProcellarumState
        {
            Release,
            Travel,
            End
        }
        ProcellarumState chargeState;
        // Old Vals: 75, 200, 20, 39, 64-rot
        public override int BaseSpeed => 30;
        public override int HalberdSize => 140;
        public override int RotPointToBlade => 16;
        public override int RotationOffset => 31;
        public override int StartOffset => 54 - RotationOffset;

        public float cursorRotation;
        public Vector2 npcHitPostition;

        public const int MAX_CHARGE_STAGE = 3;
        public const int TICKS_PER_STAGE = 60;
        public const int RELEASE_TICKS = 20;
        public const int END_TICKS = 10;

        public int currentChargeStage;
        public int currentChargeTick;
        public int currentReleaseTick;
        public int currentEndTick;

        public List<NPC> MarkedNPCs = new List<NPC>();

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            if (Projectile.ai[0] == 2)
            {
                Projectile.friendly = false;
            }

            chargeState = ProcellarumState.Release;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1)
            {
                base.AI();
            }
            else
            {
                Player.heldProj = Projectile.whoAmI;
                float angleOffset = MathHelper.Pi * 3 / 4;
                switch (chargeState)
                {
                    case ProcellarumState.Release:
                        Projectile.timeLeft += 1;
                        currentChargeTick += 1;
                        Projectile.rotation = angleOffset;
                        float CursorRotation = (Main.MouseWorld - Player.MountedCenter - new Vector2(0, 6)).ToRotation();

                        if (CursorRotation >= -MathHelper.PiOver2 && CursorRotation < MathHelper.PiOver2)
                        {
                            Player.direction = 1;
                            DrawOriginOffsetX = -(HalberdSize / 2) + RotationOffset;
                            DrawOriginOffsetY = RotDiag - RotationOffset;
                            DrawOffsetX = RotDiag - RotationOffset;
                        }
                        else
                        {
                            Player.direction = -1;
                            DrawOriginOffsetX = (HalberdSize / 2) - RotationOffset;
                            DrawOriginOffsetY = RotDiag - RotationOffset;
                            DrawOffsetX = -HalberdSize + RotDiag + RotationOffset;
                            Projectile.rotation -= MathHelper.PiOver2;
                        }
                        Projectile.spriteDirection = Player.direction;
                        Projectile.Center = Player.MountedCenter + Utility.PolarVector(50, CursorRotation);
                        Projectile.rotation += (Projectile.Center - Player.MountedCenter - new Vector2(-4 * Player.direction, 6 * Player.gravDir)).ToRotation();

                        if (currentChargeTick >= TICKS_PER_STAGE && currentChargeStage < 3)
                        {
                            currentChargeTick = 0;
                            currentChargeStage += 1;
                            SoundEngine.PlaySound(SoundID.Item29 with
                            {
                                Pitch = 0.2f + 0.5f * (currentChargeStage / 3f),
                                Volume = 0.15f * (currentChargeStage / 3f),
                                SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
                            }, Projectile.position);
                            for (int i = 0; i < 15 * currentChargeStage; i++)
                                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LuminiteBrightDust>(), Utility.PolarVector(Main.rand.Next(3, 5), CursorRotation + (MathHelper.Pi / currentChargeStage * Main.rand.Next(0, 2 * currentChargeStage))));
                        }
                        if (!Main.mouseRight)
                        {
                            Projectile.timeLeft = 30;
                            chargeState = ProcellarumState.Travel;
                            Projectile.friendly = true;
                        }

                        if (Player.direction == 1)
                            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, CursorRotation - MathHelper.ToRadians(70));
                        else
                            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, CursorRotation - MathHelper.ToRadians(110));
                        break;
                    case ProcellarumState.Travel:
                        if (Projectile.spriteDirection == 1) Projectile.velocity = Utility.PolarVector(35, Projectile.rotation - angleOffset);
                        else Projectile.velocity = Utility.PolarVector(35, Projectile.rotation + angleOffset - MathHelper.Pi);
                        break;
                    case ProcellarumState.End:
                        int ProjFired = 0;
                        Vector2 ProjSpawnPosition;
                        float FiringAngle;
                        if (currentChargeStage > 0)
                        {
                            int boltDamage = (int)(Projectile.damage * 0.5f * currentChargeStage);
                            for (int i = 0; i < Main.npc.Length && ProjFired <= 4; i++)
                            {
                                if (Main.npc[i].active)
                                {
                                    if (Main.npc[i].HasBuff(ModContent.BuffType<Procellarum_LightningMarkDebuff>()))
                                    {
                                        ProjSpawnPosition = new Vector2(Main.screenPosition.X + 0.5f * Main.screenWidth + Main.rand.Next(-128, 128), Main.screenPosition.Y);
                                        FiringAngle = (Main.npc[i].position - ProjSpawnPosition).ToRotation();
                                        ProjFired += 1;
                                        Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                                            ProjSpawnPosition,
                                            Utility.PolarVector(40 + ProjFired * 4, FiringAngle),
                                            ModContent.ProjectileType<Procellarum_LightBolt>(),
                                            boltDamage,
                                            0,
                                            Player.whoAmI,
                                            i + 1,
                                            ProjFired);
                                    }
                                }
                            }
                            if (ProjFired <= 4)
                            {
                                for (int i = ProjFired; i <= 4; i++)
                                {
                                    ProjSpawnPosition = new Vector2(Main.screenPosition.X + 0.5f * Main.screenWidth + Main.rand.Next(-128, 128), Main.screenPosition.Y);
                                    FiringAngle = (npcHitPostition - ProjSpawnPosition).ToRotation();
                                    ProjFired += 1;
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                                        ProjSpawnPosition,
                                        Utility.PolarVector(40 + ProjFired * 4, FiringAngle),
                                        ModContent.ProjectileType<Procellarum_LightBolt>(),
                                        boltDamage,
                                        0,
                                        Player.whoAmI);
                                }
                            }
                        }
                        Projectile.Kill();
                        break;
                }

                //Old alt attack code
                /*
                switch (chargeState)
                {
                    case ProcellarumState.Charging:
                        Projectile.timeLeft += 1;

                        int dust = Dust.NewDust(Projectile.Center, 5, 5, ModContent.DustType<LuminiteBrightDust>(), Player.velocity.X + Main.rand.Next(-5, 5), Player.velocity.Y + Main.rand.Next(-5, 5));
                        //Main.dust[dust].scale = 1f - (currentChargeTick * 0.1f);

                        //Do something like this
                        //Particle.CreateParticle<PortalSwirl>(p =>
                        //    {
                        //        p.Position = info.center + Main.rand.NextVector2Circular(180, 180) * 0.95f * info.scale;
                        //        p.Velocity = Vector2.One * 22;
                        //        p.Scale = (0.1f + Main.rand.NextFloat(0.1f)) * info.scale;
                        //        p.Color = new Color(92, 206, 130);
                        //        p.TargetCenter = info.center;
                        //        p.CustomDrawer = owner;
                        //    });
                         

                        currentChargeTick += 1;
                        if (currentChargeTick >= TICKS_PER_STAGE && currentChargeStage < 3)
                        {
                            currentChargeTick = 0;
                            currentChargeStage += 1;
                            //Main.NewText(currentChargeStage);
                        }
                        if (!Main.mouseRight)
                        {
                            chargeState = ProcellarumState.Release;
                        }
                        break;
                    case ProcellarumState.Release:

                        currentReleaseTick += 1;
                        if (currentReleaseTick >= RELEASE_TICKS)
                        {
                            chargeState = ProcellarumState.End;
                        }
                        break;
                    case ProcellarumState.End:

                        currentEndTick += 1;
                        if (currentEndTick >= END_TICKS)
                        {
                            int ProjFired = 0;
                            Vector2 ProjSpawnPosition;
                            float FiringAngle;
                            for (int i = 0; i < Main.npc.Length && ProjFired <= 4; i++)
                            {
                                if (Main.npc[i].active)
                                {
                                    if (Main.npc[i].HasBuff(ModContent.BuffType<Procellarum_LightningMarkDebuff>()))
                                    {
                                        ProjSpawnPosition = new Vector2(Main.screenPosition.X + 0.5f * Main.screenWidth + Main.rand.Next(-128, 128), Main.screenPosition.Y);
                                        FiringAngle = (Main.npc[i].position - ProjSpawnPosition).ToRotation();
                                        ProjFired += 1;
                                        Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                                            ProjSpawnPosition,
                                            Utility.PolarVector(40 + ProjFired * 4, FiringAngle),
                                            ModContent.ProjectileType<Procellarum_LightBolt>(),
                                            (int)(Projectile.damage * (0.5f + 1.5f * currentChargeStage)),
                                            0,
                                            Player.whoAmI,
                                            i + 1,
                                            ProjFired);
                                    }
                                }
                            }
                            if (ProjFired <= 4)
                            {
                                for (int i = ProjFired; i <= 4; i++)
                                {
                                    ProjSpawnPosition = new Vector2(Main.screenPosition.X + 0.5f * Main.screenWidth + Main.rand.Next(-128, 128), Main.screenPosition.Y);
                                    FiringAngle = (Main.MouseWorld - ProjSpawnPosition).ToRotation();
                                    ProjFired += 1;
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                                        ProjSpawnPosition,
                                        Utility.PolarVector(40 + ProjFired * 4, FiringAngle),
                                        ModContent.ProjectileType<Procellarum_LightBolt>(),
                                        (int)(Projectile.damage * (0.5f + 0.5f * currentChargeStage)),
                                        0,
                                        Player.whoAmI);
                                }
                            }
                            Projectile.Kill();
                        }
                        break;
                }
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Player.direction * -MathHelper.PiOver2);
                float holdAngle = -MathHelper.PiOver2 + MathHelper.Pi * 20 / 180 * Player.direction;
                Projectile.Center = Player.MountedCenter + Player.direction * new Vector2(8, 0);
                if (chargeState != ProcellarumState.End)
                {
                    Projectile.position.X += MathF.Cos(holdAngle) * MathHelper.Lerp(farOffset / 2, farOffset, 1 - MathF.Cos(MathHelper.TwoPi * 0.25f * currentReleaseTick / RELEASE_TICKS));
                    Projectile.position.Y += MathF.Sin(holdAngle) * MathHelper.Lerp(farOffset / 2, farOffset, 1 - MathF.Cos(MathHelper.TwoPi * 0.25f * currentReleaseTick / RELEASE_TICKS));
                }
                else
                {
                    Projectile.position.X += MathF.Cos(holdAngle) * farOffset;
                    Projectile.position.Y += MathF.Sin(holdAngle) * farOffset;
                }
                Projectile.rotation += (Projectile.Center - Player.MountedCenter - Player.direction * new Vector2(8, 0)).ToRotation();
                */
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] == 2)
            {
                for (int i = 0; i < 70; i++)
                {
                    Particle.Create<TintableSpark>((p) =>
                    {
                        p.Position = Projectile.Center;
                        p.Velocity = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.1f, 0.3f);
                        p.Scale = new(3f);
                        p.Rotation = 0f;
                        p.Color = new List<Color>() {
                        new(77, 99, 124),
                        new(90, 83, 92),
                        new(232, 239, 255)
                        }.GetRandom();
                    });
                }
                Particle.Create<TintableFlash>((p) =>
                {
                    p.Position = Projectile.Center;
                    p.Scale = new(0.01f);
                    p.ScaleVelocity = new(0.3f);
                    p.Color = new Color(232, 239, 255);
                });
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] == 1)
            {
                base.ModifyHitNPC(target, ref modifiers);
                target.AddBuff(ModContent.BuffType<Procellarum_LightningMarkDebuff>(), 1200);
            }
            else
            {
                if (currentChargeStage > 0) SoundEngine.PlaySound(SoundID.Thunder with { Volume = 0.1f * currentChargeStage }, Projectile.position);
                chargeState = ProcellarumState.End;
                npcHitPostition = target.Center;
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1 || target.friendly)
                return base.CanHitNPC(target);

            else return chargeState == ProcellarumState.Travel;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] == 1) return base.Colliding(projHitbox, targetHitbox);
            else return targetHitbox.Intersects(projHitbox);
        }
    }
    public class ProcellarumGlobalNPC : GlobalNPC
    {
        private static Asset<Texture2D> mark;

        public override void Load()
        {
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            mark ??= ModContent.Request<Texture2D>("Macrocosm/Content/Debuffs/Weapons/Procellarum_LightningMark");

            if (npc.HasBuff(ModContent.BuffType<Procellarum_LightningMarkDebuff>()))
            {
                Vector2 markPosition = new(npc.position.X + 0.5f * npc.width - 12, npc.position.Y - 36);
                spriteBatch.Draw(mark.Value, markPosition - screenPos, Color.White);
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ModContent.ProjectileType<Procellarum_LightBolt>())
            {
                modifiers.FinalDamage *= 1.5f;
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ModContent.ProjectileType<Procellarum_LightBolt>())
            {
                if (npc.HasBuff(ModContent.BuffType<Procellarum_LightningMarkDebuff>()))
                    npc.buffTime[npc.FindBuffIndex(ModContent.BuffType<Procellarum_LightningMarkDebuff>())] = 20;
                projectile.ai[0] = 0;
            }
        }
    }
}
