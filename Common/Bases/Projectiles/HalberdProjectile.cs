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
        public abstract int BaseSpeed { get; }
        private int currentAnimProgress = 0;
        private int[] baseMaxProgress = new int[5];
        private float useTimeMulti;

        private const float ARC_ANGLE_DEG = 30f;

        //private float armRotation = 0f;
        private readonly float[] stateDmgMulti = [3f, 2f, 8f, 1f, 10f, 1f];
        private readonly List<NPC> NPCsHit = new();
        private Rectangle angleHitbox;

        public abstract int HalberdSize { get; }
        public abstract int RotPointToBlade { get; }
        public abstract int RotationOffset { get; }
        public abstract int StartOffset { get; }

        public int midOffset;
        public int farOffset;
        public int RotDiag;

        public override void SetDefaults()
        {
            baseMaxProgress = [BaseSpeed, BaseSpeed / 3, BaseSpeed / 3, (int)(BaseSpeed * 2f / 3), BaseSpeed / 4, BaseSpeed / 2];
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
            float CursorRotation = (Main.MouseWorld - Player.MountedCenter).ToRotation();

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

                    attackOffset = midOffset * MathF.Sin(MathHelper.Pi * currentAnimProgress / finalMaxProgress[0]);
                    attackAngle = CursorRotation - Player.direction * (MathHelper.Pi * ARC_ANGLE_DEG / 180) * MathF.Sin(MathHelper.TwoPi * FloatAnimProgress);

                    if (currentAnimProgress == finalMaxProgress[(int)state] / 2)
                        NPCsHit.Clear();

                    break;

                case HalberdState.TransOne:

                    attackOffset = (float)MathHelper.Lerp(StartOffset, midOffset, 0.5f * FloatAnimProgress);
                    attackAngle = CursorRotation - Player.direction * MathHelper.PiOver2 * MathF.Sin(MathHelper.TwoPi * 0.25f * FloatAnimProgress);

                    break;
                case HalberdState.AttackTwo:
                    attackOffset = (float)MathHelper.Lerp(StartOffset, midOffset, 0.5f + 0.5f * MathF.Sin(MathHelper.TwoPi * 0.25f * FloatAnimProgress));
                    attackAngle = CursorRotation - Player.direction * MathHelper.PiOver2;
                    attackAngle += Player.direction * (MathHelper.TwoPi / 3) * (1 - MathF.Cos(MathHelper.TwoPi * 0.25f * FloatAnimProgress));

                    break;
                case HalberdState.TransTwo:
                    attackOffset = (float)MathHelper.Lerp(midOffset, StartOffset, 1 - MathF.Cos(MathHelper.TwoPi * 0.25f * FloatAnimProgress));
                    attackAngle = (float)MathHelper.Lerp(CursorRotation + Player.direction * (MathHelper.Pi / 6), CursorRotation, FloatAnimProgress);

                    break;
                case HalberdState.AttackThree:
                    attackOffset = (int)MathHelper.Lerp(StartOffset, farOffset, FloatAnimProgress);
                    attackAngle = CursorRotation;

                    if (Player.velocity.X / Player.direction > 0 && currentAnimProgress == 0)
                    {
                        Player.velocity.X += Player.direction * useTimeMulti * MathHelper.Lerp(5, 0, FloatAnimProgress);
                    }
                    break;
                case HalberdState.TransThree:
                    attackOffset = (int)Terraria.Utils.Lerp(farOffset, StartOffset, FloatAnimProgress);
                    attackAngle = CursorRotation;

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
                    state = state.Next();

                    if ((int)state % 2 == 0)
                        SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
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
            //TODO code for hitting npcs that can be killed with a voodoo doll equipped.
            //if (NPCsHit.Contains(target) | (!(target.type == NPCID.Guide && Player.killGuide) | !(target.type == NPCID.Clothier && Player.killClothier)) && target.friendly)
            if (NPCsHit.Contains(target) | target.friendly)
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
        public override Color? GetAlpha(Color lightColor) => Lighting.GetColor(Main.player[Projectile.owner].Center.ToTileCoordinates());

    }
}
