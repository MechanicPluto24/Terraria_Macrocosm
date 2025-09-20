using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.Weapons;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee;

public class ProcellarumHalberdProjectile : HalberdProjectile
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
        Projectile.alpha = 255;
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
                    float cursorRotation = (Main.MouseWorld - Player.MountedCenter - new Vector2(0, 6)).ToRotation();

                    if (cursorRotation >= -MathHelper.PiOver2 && cursorRotation < MathHelper.PiOver2)
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
                    Projectile.Center = Player.MountedCenter + Utility.PolarVector(50, cursorRotation);
                    Projectile.rotation += (Projectile.Center - Player.MountedCenter - new Vector2(-4 * Player.direction, 6 * Player.gravDir)).ToRotation();

                    float stageProgress = (float)currentChargeTick / TICKS_PER_STAGE;
                    float targetAlpha = 255f * (1f - (currentChargeStage + stageProgress) / 3f);
                    Projectile.alpha = (int)MathHelper.Lerp(Projectile.alpha, targetAlpha, 0.1f);

                    if (currentChargeStage == 3)
                        Projectile.alpha = 0;

                    Projectile.position += Main.rand.NextVector2Circular(1f, 1f) * (1f - Projectile.alpha / 255f);

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
                            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LuminiteBrightDust>(), Utility.PolarVector(Main.rand.Next(2, 4), cursorRotation + (MathHelper.Pi / currentChargeStage * Main.rand.Next(0, 2 * currentChargeStage))).RotatedByRandom(MathHelper.Pi / 64f));
                    }
                    if (!Main.mouseRight)
                    {
                        Projectile.timeLeft = 30;
                        chargeState = ProcellarumState.Travel;
                        Projectile.friendly = true;
                    }

                    if (Player.direction == 1)
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, cursorRotation - MathHelper.ToRadians(70));
                    else
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, cursorRotation - MathHelper.ToRadians(110));
                    break;

                case ProcellarumState.Travel:
                    if (Projectile.spriteDirection == 1) Projectile.velocity = Utility.PolarVector(35, Projectile.rotation - angleOffset);
                    else Projectile.velocity = Utility.PolarVector(35, Projectile.rotation + angleOffset - MathHelper.Pi);
                    break;

                case ProcellarumState.End:
                    Projectile.Kill();
                    break;
            }
        }
    }
    public override void OnKill(int timeLeft)
    {
        if (Projectile.ai[0] == 2)
        {
            float strength = 1f;
            Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ProcellarumExplosion>(), Projectile.damage, 12f, Main.myPlayer, ai0: strength, ai1: 1f);
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Projectile.ai[0] != 1)
        {
            if (currentChargeStage > 0)
                SoundEngine.PlaySound(SoundID.Thunder with { Volume = 0.1f * currentChargeStage }, Projectile.position);

            chargeState = ProcellarumState.End;
            npcHitPostition = target.Center;
            target.AddBuff<ProcellarumLightningMark>(3 * 60);
        }
        else
        {
            base.ModifyHitNPC(target, ref modifiers);
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
