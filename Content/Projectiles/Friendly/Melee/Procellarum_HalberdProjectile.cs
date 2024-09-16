using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Debuffs.Weapons;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class Procellarum_HalberdProjectile : HalberdProjectile
    {
        public enum ProcellarumState
        {
            Charging,
            Release,
            End
        }
        ProcellarumState chargeState;
        public override int BaseSpeed => 65;
        public override int HalberdSize => 200;
        public override int RotPointToBlade => 20;
        public override int RotationOffset => 39;
        public override int StartOffset => 64 - RotationOffset;

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
            chargeState = ProcellarumState.Charging;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f)
            {
                base.AI();
            }
            else
            {
                Player.heldProj = Projectile.whoAmI;

                float angleOffset = MathHelper.Pi * 3 / 4;
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

                switch (chargeState)
                {
                    case ProcellarumState.Charging:
                        Projectile.timeLeft += 1;

                        int dust = Dust.NewDust(Projectile.Center, 5, 5, ModContent.DustType<LuminiteBrightDust>(), Player.velocity.X + Main.rand.Next(-5, 5), Player.velocity.Y + Main.rand.Next(-5, 5));
                        //Main.dust[dust].scale = 1f - (currentChargeTick * 0.1f);

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
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (Projectile.ai[0] == 1f)
            {
                target.AddBuff(ModContent.BuffType<Procellarum_LightningMarkDebuff>(), 1200);
            }
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
                modifiers.FinalDamage *= 2;
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ModContent.ProjectileType<Procellarum_LightBolt>())
            {
                if (npc.HasBuff(ModContent.BuffType<Procellarum_LightningMarkDebuff>()))
                    //npc.DelBuff(npc.FindBuffIndex(ModContent.BuffType<Procellarum_LightningMarkDebuff>()));
                    npc.buffTime[npc.FindBuffIndex(ModContent.BuffType<Procellarum_LightningMarkDebuff>())] = 20;
                projectile.ai[0] = 0;
            }
        }
    }
}
