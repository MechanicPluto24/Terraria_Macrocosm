using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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
        }
        private HalberdState state;

        //public abstract int rotationPoint { get; }
        //public abstract int regularHoldOffset { get; }
        //public abstract int midHoldOffset { get; }
        //public abstract int farHoldOffset { get; }
        public Player Player => Main.player[Projectile.owner];
        public abstract int baseSpeed { get; }
        private int currentAnimProgress = 0;
        private int[] baseMaxProgress = new int[5];

        private const float ARC_ANGLE_DEG = 15f;

        //private float armRotation = 0f;
        private float[] stateDmgMulti = new float[5] { 1f, 0.5f, 1.5f, 0.5f, 2f };

        public abstract int halberdSize { get; }
        public abstract int rotationOffset { get; }
        public abstract int startOffset { get; }
        public int midOffset;
        public abstract int farOffset { get; }


        public override void SetDefaults()
        {
            baseMaxProgress = new int[5] { baseSpeed, baseSpeed / 2, baseSpeed, baseSpeed / 2, baseSpeed / 2 };
            midOffset = ((halberdSize * 3/4) + farOffset) / 2;
            Projectile.Size = new Vector2(2 * rotationOffset);
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = true;

            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnSpawn(IEntitySource source)
        {
            currentAnimProgress = 0;
        }


        public override void AI()
        {
            Projectile.timeLeft += 1;
            Player.heldProj = Projectile.whoAmI;

            float useTimeMulti = Player.GetAttackSpeed(DamageClass.Melee);
            int[] finalMaxProgress = new int[5];
            for (int i = 0; i < finalMaxProgress.Length; i++)
                finalMaxProgress[i] = (int)(baseMaxProgress[i] * (1 / useTimeMulti));

            Projectile.Center = Player.MountedCenter;

            float attackOffset;
            float angleOffset = MathHelper.Pi * 3/4;
            Projectile.rotation = angleOffset;
            float CursorRotation = (Main.MouseWorld - Player.Center).ToRotation();
            if (CursorRotation >= -MathHelper.PiOver2 && CursorRotation < MathHelper.PiOver2)
            {
                Player.direction = 1;
                DrawOriginOffsetX = -(halberdSize / 2) + rotationOffset;
                DrawOffsetX = 0;
            }
            else
            {
                Player.direction = -1;
                DrawOriginOffsetX = (halberdSize / 2) - rotationOffset;
                DrawOffsetX = -halberdSize + (2 * rotationOffset);
                Projectile.rotation -= MathHelper.PiOver2;
            }
            Projectile.spriteDirection = Player.direction;

            switch (state)
            {
                case HalberdState.AttackOne:
                    /*
                    Projectile.rotation += CursorRotation;
                    Projectile.rotation -= Player.direction * (MathHelper.Pi * ARC_ANGLE_DEG / 180) * MathF.Sin((MathHelper.TwoPi * currentAnimProgress / finalMaxProgress[0]));

                    if (Player.direction == 1)
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, CursorRotation - MathHelper.ToRadians(70));
                    else
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, CursorRotation - MathHelper.ToRadians(110));

                    attackOffset = (midHoldOffset - regularHoldOffset) * MathF.Sin(MathHelper.Pi * currentAnimProgress / finalMaxProgress[0]);
                    Projectile.position.X -= (int)(MathF.Cos(CursorRotation) * (regularHoldOffset + attackOffset));
                    Projectile.position.Y -= (int)(MathF.Sin(CursorRotation) * (regularHoldOffset + attackOffset));
                    */

                    //Projectile.rotation += CursorRotation;

                    //Projectile.rotation += CursorRotation;
                    //Projectile.rotation -= Player.direction * (MathHelper.Pi * ARC_ANGLE_DEG / 180) * MathF.Sin((MathHelper.TwoPi * currentAnimProgress / finalMaxProgress[0]));
                    attackOffset = (midOffset - startOffset) * MathF.Sin(MathHelper.Pi * currentAnimProgress / finalMaxProgress[0]);
                    Projectile.position.X += (MathF.Cos(CursorRotation) * (startOffset + attackOffset));
                    Projectile.position.Y += (MathF.Sin(CursorRotation) * (startOffset + attackOffset));

                    Projectile.position.X += Player.direction * (MathF.Sin(CursorRotation) * (startOffset + attackOffset)) * MathF.Tan((MathHelper.Pi * ARC_ANGLE_DEG / 180) * MathF.Sin((MathHelper.TwoPi * currentAnimProgress / finalMaxProgress[0])));
                    Projectile.position.Y -= Player.direction * (MathF.Cos(CursorRotation) * (startOffset + attackOffset)) * MathF.Tan((MathHelper.Pi * ARC_ANGLE_DEG / 180) * MathF.Sin((MathHelper.TwoPi * currentAnimProgress / finalMaxProgress[0])));

                    Projectile.rotation += (Projectile.Center - Player.Center).ToRotation();

                    if (Player.direction == 1)
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, CursorRotation - MathHelper.ToRadians(90));
                    else
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, CursorRotation - MathHelper.ToRadians(130));

                    //Projectile.Size = new Vector2(2 * Terraria.Utils.GetLerpValue(rotationOffset, MathF.Sqrt(2 * (rotationOffset ^ 2)), MathF.Cos(2 * (Projectile.rotation % MathHelper.PiOver2))));
                    break;
            }

            currentAnimProgress += 1;
            //Main.NewText($"{currentAnimProgress} / {finalMaxProgress[(int)state]}");
            if (currentAnimProgress >= finalMaxProgress[(int)state])
            {
                Main.NewText(Player.channel);
                if (Player.channel)
                {
                    currentAnimProgress = 0;
                }
                else Projectile.Kill();
                //state = HalberdState.TransOne;
            }
        }
    }
}
