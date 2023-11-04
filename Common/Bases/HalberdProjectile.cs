using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases
{
    public abstract class HalberdProjectile : ModProjectile
    {
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

        public Player Player => Main.player[Projectile.owner];
        public abstract int baseSpeed { get; }
        private int currentAnimProgress = 0;
        private int[] baseMaxProgress = new int[5];
        private float useTimeMulti;

        private const float ARC_ANGLE_DEG = 30f;

        //private float armRotation = 0f;
        private float[] stateDmgMulti = new float[6] { 1f, 0.5f, 2f, 0.5f, 3f, 0.5f };
        private List<NPC> NPCsHit = new List<NPC>();
        private Rectangle angleHitbox;

        public abstract int halberdSize { get; }
        public abstract int rotPointToBlade { get; }
        public abstract int rotationOffset { get; }
        public abstract int startOffset { get; }
        private int midOffset;
        public abstract int farOffset { get; }
        private int RotDiag;

        public override void SetDefaults()
        {
            baseMaxProgress = new int[6] { baseSpeed, baseSpeed / 2, baseSpeed / 3, baseSpeed / 2, baseSpeed / 4, baseSpeed / 2 };

            midOffset = (int)Terraria.Utils.Lerp(startOffset, farOffset, 0.75);
            RotDiag = Utility.SquareDiagonal(rotationOffset);
            Projectile.Size = new Vector2(2 * RotDiag);

            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = true;

            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnSpawn(IEntitySource source)
        {
            currentAnimProgress = 0;
            state = HalberdState.AttackOne;
            useTimeMulti = Player.GetAttackSpeed(DamageClass.Melee);
        }


        public override void AI()
        {
            Projectile.timeLeft += 1;
            Player.heldProj = Projectile.whoAmI;

            int[] finalMaxProgress = new int[6];
            for (int i = 0; i < finalMaxProgress.Length; i++)
                finalMaxProgress[i] = (int)(baseMaxProgress[i] * (1 / useTimeMulti));

            Projectile.Center = Player.MountedCenter - new Vector2(0, 6);

            float attackOffset = 0f;
            float attackAngle = 0f;

            float angleOffset = MathHelper.Pi * 3/4;
            Projectile.rotation = angleOffset;
            float CursorRotation = (Main.MouseWorld - Player.MountedCenter - new Vector2(0, 6)).ToRotation();

            if (CursorRotation >= -MathHelper.PiOver2 && CursorRotation < MathHelper.PiOver2)
            {
                Player.direction = 1;
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

            float FloatAnimProgress = (float)currentAnimProgress / finalMaxProgress[(int)state];

            switch (state)
            {
                case HalberdState.AttackOne:
                    
                    attackOffset = (midOffset) * MathF.Sin(MathHelper.Pi * currentAnimProgress / finalMaxProgress[0]);
                    attackAngle = CursorRotation - (Player.direction * (MathHelper.Pi * ARC_ANGLE_DEG / 180) * MathF.Sin(MathHelper.TwoPi * FloatAnimProgress));
                    
                    if (currentAnimProgress == finalMaxProgress[(int)state] / 2)
                        NPCsHit.Clear();

                    break;

                case HalberdState.TransOne:
                    
                    attackOffset = (float)Terraria.Utils.Lerp(startOffset, midOffset, 0.5f * FloatAnimProgress);
                    attackAngle = CursorRotation - (Player.direction * MathHelper.PiOver2 * MathF.Sin(MathHelper.TwoPi * 0.25f * FloatAnimProgress));

                    break;
                case HalberdState.AttackTwo:
                    attackOffset = (float)Terraria.Utils.Lerp(startOffset, midOffset, 0.5f + 0.5f * MathF.Sin(MathHelper.TwoPi * 0.25f * FloatAnimProgress));
                    attackAngle = CursorRotation - Player.direction * MathHelper.PiOver2;
                    attackAngle += Player.direction * (MathHelper.TwoPi / 3) * (1 - MathF.Cos(MathHelper.TwoPi * 0.25f * FloatAnimProgress));

                    break;
                case HalberdState.TransTwo:
                    attackOffset = (float)Terraria.Utils.Lerp(midOffset, startOffset, FloatAnimProgress);
                    attackAngle = (float)Terraria.Utils.Lerp(CursorRotation + Player.direction * (MathHelper.Pi / 6), CursorRotation, FloatAnimProgress);

                    break;
                case HalberdState.AttackThree:
                    attackOffset = (int)Terraria.Utils.Lerp(startOffset, farOffset, FloatAnimProgress);
                    attackAngle = CursorRotation;
                    /*
                    if (Player.velocity.X / Player.direction > 0 && currentAnimProgress == 0)
                    {
                        Player.velocity += 25f * new Vector2(MathF.Sin(CursorRotation), MathF.Cos(CursorRotation));
                    }
                    */
                    break;
                case HalberdState.TransThree:
                    attackOffset = (int)Terraria.Utils.Lerp(farOffset, startOffset, FloatAnimProgress);
                    attackAngle = CursorRotation;

                    break;
            }

            Projectile.position.X += (MathF.Cos(attackAngle) * (startOffset + attackOffset));
            Projectile.position.Y += (MathF.Sin(attackAngle) * (startOffset + attackOffset));
            Projectile.rotation += (Projectile.Center - Player.Center).ToRotation();

            if (Player.direction == 1)
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, attackAngle - MathHelper.ToRadians(70));
            else
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, attackAngle - MathHelper.ToRadians(110));

            int hitboxMin = Utility.SquareDiagonal(rotPointToBlade) * 2;
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
            if (currentAnimProgress >= finalMaxProgress[(int)state])
            {
                if (Player.channel)
                {
                    currentAnimProgress = 0;
                    useTimeMulti = Player.GetAttackSpeed(DamageClass.Melee);
                    NPCsHit.Clear();
                    
                    state = Utility.Next(state);
                }
                else Projectile.Kill();
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            NPCsHit.Add(target);
            modifiers.SourceDamage *= stateDmgMulti[(int)state];
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
            if (NPCsHit.Contains(target))
                return false;
            else
                return true;
        }
        /*
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(angleHitbox.X - (int)Main.screenPosition.X, angleHitbox.Y - (int)Main.screenPosition.Y, angleHitbox.Width, angleHitbox.Height), Color.Red);
        }
        */
    }
}
