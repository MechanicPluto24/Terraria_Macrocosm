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
        public abstract int regularHoldOffset { get; }
        public abstract int midHoldOffset { get; }
        public abstract int farHoldOffset { get; }
        public Player Player => Main.player[Projectile.owner];
        public abstract int baseSpeed { get; }
        private int currentAnimProgress = 0;
        private int[] baseMaxProgress = new int[5];

        private const float ARC_ANGLE_DEG = 15f;

        private float armRotation = 0f;
        private float[] stateDmgMulti = new float[5] { 1f, 0.5f, 1.5f, 0.5f, 2f };

        public override void SetDefaults()
        {
            baseMaxProgress = new int[5] { baseSpeed, baseSpeed / 2, baseSpeed, baseSpeed / 2, baseSpeed / 2 };
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            currentAnimProgress = 0;
        }

        public override void AI()
        {
            float useTimeMulti = Player.GetAttackSpeed(DamageClass.Melee);
            int[] finalMaxProgress = new int[5];
            for (int i = 0; i < finalMaxProgress.Length; i++)
                finalMaxProgress[i] = (int)(baseMaxProgress[i] * (1 / useTimeMulti));
            Projectile.timeLeft += 1;
            currentAnimProgress += 1;
            Main.NewText($"{currentAnimProgress} / {finalMaxProgress[(int)state]}");
            if (currentAnimProgress == finalMaxProgress[(int)state])
            {
                Projectile.Kill();
            }

            Projectile.Center = Player.MountedCenter;
            float attackOffset;

            float angleOffset = -MathHelper.PiOver4;
            Projectile.rotation = angleOffset;
            float CursorRotation = (Main.MouseWorld - Player.Center).ToRotation();
            if (CursorRotation >= -MathHelper.PiOver2 && CursorRotation < MathHelper.PiOver2)
                Player.direction = 1;
            else
            {
                Player.direction = -1;
                Projectile.rotation -= MathHelper.PiOver2;
            }
            Projectile.spriteDirection = Player.direction;
            switch (state)
            {
                case HalberdState.AttackOne:
                    Projectile.rotation += CursorRotation;
                    Projectile.rotation -= Player.direction * (MathHelper.Pi * ARC_ANGLE_DEG / 180) * MathF.Sin((MathHelper.TwoPi * currentAnimProgress / finalMaxProgress[0]));



                    attackOffset = (midHoldOffset - regularHoldOffset) * MathF.Sin(MathHelper.Pi * currentAnimProgress / finalMaxProgress[0]);
                    Projectile.position.X -= (int)(MathF.Cos(CursorRotation) * (regularHoldOffset + attackOffset));
                    Projectile.position.Y -= (int)(MathF.Sin(CursorRotation) * (regularHoldOffset + attackOffset));

                    break;
            }

            if (currentAnimProgress == finalMaxProgress[0])
            {
                if (Player.channel)
                {
                    Main.NewText(Player.channel);
                    currentAnimProgress = 0;
                }
                //state = HalberdState.TransOne;
            }
        }
    }
}
