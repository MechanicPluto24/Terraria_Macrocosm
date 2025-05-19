using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Projectiles
{
    public abstract class HalberdProjectile : ModProjectile
    {
        /*
        private enum HalberdState
        {
            AttackOne,
            TransOne,
            AttackTwo,
            TransTwo,
            AttackThree,
            TransThree,
        }
        private HalberdState state;
        */

        public Player Player => Main.player[Projectile.owner];
        public abstract int BaseSpeed { get; }
        private int currentAnimProgress = 0;
        //private int[] baseMaxProgress = new int[5];
        private float useTimeMulti;

        private float arcAngleDegrees;

        //private float armRotation = 0f;
        //private float[] stateDmgMulti = new float[6] { 3f, 2f, 8f, 1f, 10f, 1f };
        private readonly List<NPC> NPCsHit = new();
        private Rectangle angleHitbox;

        /// <summary>
        /// Square dimensions of the projectile
        /// </summary>
        public abstract int HalberdSize { get; }

        /// <summary>
        /// Horizontal distance in pixels from rotation point to the furthest out part of the blade
        /// </summary>
        public abstract int RotPointToBlade { get; }

        /// <summary>
        /// Horizontal distance in pixels from the tip of the spike to the rotation point
        /// </summary>
        public abstract int RotationOffset { get; }

        /// <summary>
        /// Horizontal distance in pixels from the tip of the spike to the starting point take away the rotationOffset value
        /// </summary>
        public abstract int StartOffset { get; }

        public int midOffset;
        public int farOffset;
        public int RotDiag;
        public bool RotationLock = true;
        public float BaseRotation;
        public override void SetDefaults()
        {
            //baseMaxProgress = new int[6] { baseSpeed, baseSpeed / 3, baseSpeed / 3, (int)(baseSpeed * 2f / 3), baseSpeed / 4, baseSpeed / 2 };
            farOffset = HalberdSize - RotationOffset;
            midOffset = (int)MathHelper.Lerp(StartOffset, farOffset, 0.67f);
            RotDiag = Utility.SquareDiagonal(RotationOffset);
            Projectile.Size = new Vector2(2 * RotDiag);

            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Melee;

            MoRHelper.SetSpearBonus(Projectile);
        }

        public override void OnSpawn(IEntitySource source)
        {
            currentAnimProgress = 0;
            //state = HalberdState.AttackOne;
            useTimeMulti = Player.GetAttackSpeed(DamageClass.Melee);
        }

        public override void AI()
        {
            /*
            Projectile.timeLeft += 1;
            Player.heldProj = Projectile.whoAmI;
            //Main.NewText(Player.altFunctionUse);
            int[] finalMaxProgress = new int[6];
            for (int i = 0; i < finalMaxProgress.Length; i++)
            {
                finalMaxProgress[i] = (int)(baseMaxProgress[i] * (1 / useTimeMulti));
            }

            //Main.NewText(state);
            //Main.NewText($"{currentAnimProgress}/{finalMaxProgress[(int)state]}");

            Projectile.Center = Player.MountedCenter - new Vector2(0, 6);

            float attackOffset = 0f;
            float attackAngle = 0f;

            float angleOffset = MathHelper.Pi * 3 / 4;
            Projectile.rotation = angleOffset;
            float CursorRotation;
            if (currentAnimProgress == 0 && RotationLock) BaseRotation = (Main.MouseWorld - Player.MountedCenter).ToRotation();
            else if (!RotationLock)
            {
                CursorRotation = (Main.MouseWorld - Player.MountedCenter).ToRotation();
                BaseRotation = MathHelper.Lerp(BaseRotation, CursorRotation, 0.5f);
            }

            if (BaseRotation >= -MathHelper.PiOver2 && BaseRotation < MathHelper.PiOver2)
            {
                Player.direction = 1;
                DrawOriginOffsetX = -(HalberdSize / 2) + RotationOffset;
                DrawOriginOffsetY = RotDiag - RotationOffset;
                DrawOffsetX = RotDiag - RotationOffset;
            }
            else
            {
                Player.direction = -1;
                DrawOriginOffsetX = HalberdSize / 2 - RotationOffset;
                DrawOriginOffsetY = RotDiag - RotationOffset;
                DrawOffsetX = -HalberdSize + RotDiag + RotationOffset;
                Projectile.rotation -= MathHelper.PiOver2;
            }
            Projectile.spriteDirection = Player.direction;

            float FloatAnimProgress = (float)currentAnimProgress / finalMaxProgress[(int)state];

            switch (state)
            {
                case HalberdState.AttackOne:
                    
                    attackOffset = (midOffset) * MathF.Sin(MathHelper.Pi * currentAnimProgress / finalMaxProgress[0]);
                    attackAngle = BaseRotation - (Player.direction * (MathHelper.Pi * ARC_ANGLE_DEG / 180) * MathF.Sin(MathHelper.TwoPi * FloatAnimProgress));
                    
                    if (currentAnimProgress == finalMaxProgress[(int)state] / 2)
                        NPCsHit.Clear();

                    break;

                case HalberdState.TransOne:
                    
                    attackOffset = (float)MathHelper.Lerp(startOffset, midOffset, 0.5f * FloatAnimProgress);
                    attackAngle = BaseRotation - (Player.direction * MathHelper.PiOver2 * MathF.Sin(MathHelper.TwoPi * 0.25f * FloatAnimProgress));

                    break;
                case HalberdState.AttackTwo:
                    attackOffset = (float)MathHelper.Lerp(startOffset, midOffset, 0.5f + 0.5f * MathF.Sin(MathHelper.TwoPi * 0.25f * FloatAnimProgress));
                    attackAngle = BaseRotation - Player.direction * MathHelper.PiOver2;
                    attackAngle += Player.direction * (MathHelper.TwoPi / 3) * (1 - MathF.Cos(MathHelper.TwoPi * 0.25f * FloatAnimProgress));

                    break;
                case HalberdState.TransTwo:
                    attackOffset = (float)MathHelper.Lerp(midOffset, startOffset, 1 - MathF.Cos(MathHelper.TwoPi * 0.25f * FloatAnimProgress));
                    attackAngle = (float)MathHelper.Lerp(BaseRotation + Player.direction * (MathHelper.Pi / 6), BaseRotation, FloatAnimProgress);

                    break;
                case HalberdState.AttackThree:
                    attackOffset = (int)MathHelper.Lerp(startOffset, farOffset, FloatAnimProgress);
                    attackAngle = BaseRotation;

                    if (Player.velocity.X / Player.direction > 0 && currentAnimProgress == 0)
                    {
                        Player.velocity.X += Player.direction * useTimeMulti * MathHelper.Lerp(5, 0, FloatAnimProgress);
                    }
                    break;
                case HalberdState.TransThree:
                    attackOffset = (int)Terraria.Utils.Lerp(farOffset, startOffset, FloatAnimProgress);
                    attackAngle = BaseRotation;

                    break;
            }



            Projectile.position.X += MathF.Cos(attackAngle) * (StartOffset + attackOffset);
            Projectile.position.Y += MathF.Sin(attackAngle) * (StartOffset + attackOffset);
            Projectile.rotation += (Projectile.Center - Player.MountedCenter).ToRotation();

            if (Player.direction == 1)
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, attackAngle - MathHelper.ToRadians(70));
            else
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, attackAngle - MathHelper.ToRadians(110));

            int hitboxMin = Utility.SquareDiagonal(RotPointToBlade) * 2;
            int hitboxMax = RotDiag * 2;
            if (attackAngle >= -MathHelper.PiOver2 && attackAngle < MathHelper.PiOver2)
            {
                angleHitbox.Width = (int)Terraria.Utils.Lerp(hitboxMin, hitboxMax, MathF.Cos(attackAngle));
                angleHitbox.Height = (int)Terraria.Utils.Lerp(hitboxMin, hitboxMax, 1 - MathF.Cos(attackAngle));
            }
            else
            {
                angleHitbox.Width = (int)Terraria.Utils.Lerp(hitboxMin, hitboxMax, MathF.Cos(attackAngle + MathHelper.Pi));
                angleHitbox.Height = (int)Terraria.Utils.Lerp(hitboxMin, hitboxMax, 1 - MathF.Cos(attackAngle + MathHelper.Pi));
            }
            angleHitbox.X = (int)(Projectile.Center.X - 0.5f * new Vector2(angleHitbox.Width, angleHitbox.Height).X);
            angleHitbox.Y = (int)(Projectile.Center.Y - 0.5f * new Vector2(angleHitbox.Width, angleHitbox.Height).Y);

            currentAnimProgress += 1;
            //Main.NewText($"{currentAnimProgress}/{finalMaxProgress[(int)state]}");
            if (currentAnimProgress >= finalMaxProgress[(int)state])
            {
                if (Player.channel)
                {
                    currentAnimProgress = 0;
                    useTimeMulti = Player.GetAttackSpeed(DamageClass.Melee);
                    NPCsHit.Clear();
                    
                    state = Utility.Next(state);
                    RotationLock ^= true;
                }
                else Projectile.Kill();
            }
            */
            Projectile.timeLeft += 1;
            Player.heldProj = Projectile.whoAmI;
            int finalMaxProgress = (int)(BaseSpeed / useTimeMulti);
            Projectile.Center = Player.MountedCenter - new Vector2(0, 6);

            float attackOffset;
            float attackAngle;

            float angleOffset = MathHelper.Pi * 3 / 4;
            Projectile.rotation = angleOffset;
            if (currentAnimProgress == 0)
            {
                arcAngleDegrees = Main.rand.Next(20);
                SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce, Projectile.position);
                //TODO play ghastlyglaivepierce sound on swing
            }
            if (RotationLock) BaseRotation = (Main.MouseWorld - Player.MountedCenter).ToRotation();
            //else if (!RotationLock)
            //{
            //    CursorRotation = (Main.MouseWorld - Player.MountedCenter).ToRotation();
            //    BaseRotation = MathHelper.Lerp(BaseRotation, CursorRotation, 0.5f);
            //}
            /*
            if (BaseRotation >= -MathHelper.PiOver2 && BaseRotation < MathHelper.PiOver2)
            {

                Player.direction = 1;
                DrawOriginOffsetX = -(halberdSize / 2) + rotationOffset;
                DrawOriginOffsetY = RotDiag - rotationOffset;
                DrawOffsetX = RotDiag - rotationOffset;
            }
            else
            {
                Player.direction = -1;
                DrawOriginOffsetX = (halberdSize / 2) - rotationOffset;
                DrawOriginOffsetY = RotDiag - rotationOffset;
                DrawOffsetX = -halberdSize + RotDiag + rotationOffset;
                Projectile.rotation -= MathHelper.PiOver2;
            }
            Projectile.spriteDirection = Player.direction;
            */
            if (BaseRotation >= -MathHelper.PiOver2 && BaseRotation < MathHelper.PiOver2)
            {
                Player.direction = 1;
                Projectile.spriteDirection = (int)Player.gravDir;
            }
            else
            {
                Player.direction = -1;
                Projectile.spriteDirection = -1 * (int)Player.gravDir;
            }

            if (Projectile.spriteDirection == 1)
            {
                DrawOriginOffsetX = -(HalberdSize / 2) + RotationOffset;
                DrawOriginOffsetY = RotDiag - RotationOffset;
                DrawOffsetX = RotDiag - RotationOffset;
            }
            else
            {
                DrawOriginOffsetX = (HalberdSize / 2) - RotationOffset;
                DrawOriginOffsetY = RotDiag - RotationOffset;
                DrawOffsetX = -HalberdSize + RotDiag + RotationOffset;
                Projectile.rotation -= MathHelper.PiOver2;
            }

            float FloatAnimProgress = (float)currentAnimProgress / finalMaxProgress;

            attackOffset = (farOffset) * MathF.Sin(MathHelper.Pi * currentAnimProgress / finalMaxProgress);
            attackAngle = BaseRotation - (Projectile.spriteDirection * (MathHelper.Pi * arcAngleDegrees / 180) * MathF.Sin(MathHelper.TwoPi * FloatAnimProgress));

            Projectile.position.X += MathF.Cos(attackAngle) * (StartOffset + attackOffset);
            Projectile.position.Y += MathF.Sin(attackAngle) * (StartOffset + attackOffset);
            Projectile.rotation += (Projectile.Center - Player.MountedCenter - new Vector2(-4 * Player.direction, 6 * Player.gravDir)).ToRotation();

            if (Player.direction == 1)
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, attackAngle - MathHelper.ToRadians(70));
            else
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, attackAngle - MathHelper.ToRadians(110));

            int hitboxMin = Utility.SquareDiagonal(RotPointToBlade) * 2;
            int hitboxMax = RotDiag * 2;
            if (attackAngle >= -MathHelper.PiOver2 && attackAngle < MathHelper.PiOver2)
            {
                angleHitbox.Width = (int)Terraria.Utils.Lerp(hitboxMin, hitboxMax, MathF.Cos(attackAngle));
                angleHitbox.Height = (int)Terraria.Utils.Lerp(hitboxMin, hitboxMax, 1 - MathF.Cos(attackAngle));
            }
            else
            {
                angleHitbox.Width = (int)Terraria.Utils.Lerp(hitboxMin, hitboxMax, MathF.Cos(attackAngle + MathHelper.Pi));
                angleHitbox.Height = (int)Terraria.Utils.Lerp(hitboxMin, hitboxMax, 1 - MathF.Cos(attackAngle + MathHelper.Pi));
            }
            angleHitbox.X = (int)(Projectile.Center.X - 0.5f * new Vector2(angleHitbox.Width, angleHitbox.Height).X);
            angleHitbox.Y = (int)(Projectile.Center.Y - 0.5f * new Vector2(angleHitbox.Width, angleHitbox.Height).Y);

            currentAnimProgress += 1;
            if (currentAnimProgress >= finalMaxProgress)
            {
                if (Player.channel)
                {
                    currentAnimProgress = 0;
                    useTimeMulti = Player.GetAttackSpeed(DamageClass.Melee);
                    NPCsHit.Clear();

                    RotationLock ^= true;
                }
                else Projectile.Kill();
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            NPCsHit.Add(target);
            //modifiers.SourceDamage *= stateDmgMulti[(int)state];
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Intersects(angleHitbox))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            //TODO code for hitting npcs that can be killed with a voodoo doll equipped.
            //if (NPCsHit.Contains(target) | (!(target.type == NPCID.Guide && Player.killGuide) | !(target.type == NPCID.Clothier && Player.killClothier)) && target.friendly)
            if (NPCsHit.Contains(target) | target.friendly)
                return false;
            else
                return true;
        }
        public override void PostDraw(Color lightColor)
        {
            //Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(angleHitbox.X - (int)Main.screenPosition.X, angleHitbox.Y - (int)Main.screenPosition.Y, angleHitbox.Width, angleHitbox.Height), Color.Red);
        }

        public override Color? GetAlpha(Color lightColor) => Lighting.GetColor(Main.player[Projectile.owner].Center.ToTileCoordinates()).WithAlpha((byte)Projectile.alpha);

    }
}
