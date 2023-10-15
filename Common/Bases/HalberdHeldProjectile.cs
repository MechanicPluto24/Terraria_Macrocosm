using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Drawing.Text;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases
{
    public abstract class HalberdHeldProjectileItem : HeldProjectileItem<HalberdHeldProjectile>
    {
        public abstract Vector2 SpriteHandlePosition { get; }
        public virtual HalberdSwingStyle SwingStyle => new DefaultHalberdSwingStyle();
        public virtual float? HalberdLength => null;
        public abstract float HalberdWidth { get; }
        public virtual string HeldProjectileTexturePath => Texture + "_HalberdProjectile";
        //public Texture2D HeldProjectileTexture => ModContent.Request<Texture2D>(HeldProjectileTexturePath, AssetRequestMode.ImmediateLoad).Value;

    }

    public class HalberdHeldProjectile : HeldProjectile
    {
        public override string Texture => "Terraria/Images/Item_0";
        public override HeldProjectileKillMode KillMode => HeldProjectileKillMode.Manual;
        private enum HalberdState
        {
            AttackOne,
            TransOne,
            AttackTwo,
            TransTwo,
            AttackThree,
        }
        private HalberdState state;
        public Texture2D HalberdTexture { get; private set; }
        private HalberdSwingStyle SwingStyle { get; set; }
        private float HalberdLength { get; set; }
        private float HalberdWidth { get; set; }
        private Vector2 SpriteHandlePosition { get; set; }

        private int baseSpeed { get; set; }
        private int currentAnimProgress;
        private int[] baseMaxProgress = new int[5];

        private const float ARC_ANGLE_DEG = 20f;

        private float armRotation = 0f;
        private float[] stateDmgMulti = new float[5] { 1f, 0.5f, 1.5f, 0.5f, 2f };

        protected override void OnSpawn()
        {
            if (Item.ModItem is HalberdHeldProjectileItem halberdItem)
            {
                HalberdTexture = ModContent.Request<Texture2D>(halberdItem.HeldProjectileTexturePath, AssetRequestMode.ImmediateLoad).Value;
                SwingStyle = halberdItem.SwingStyle;
                HalberdLength = halberdItem.HalberdLength ?? MathF.Sqrt(
                    MathF.Pow(HalberdTexture.Width, 2) 
                    + MathF.Pow(HalberdTexture.Height, 2));
                HalberdWidth = halberdItem.HalberdWidth;
                baseSpeed = halberdItem.Item.useTime;
                baseMaxProgress = new int[5] { baseSpeed, baseSpeed / 2, baseSpeed, baseSpeed / 2, baseSpeed / 2 };
                SpriteHandlePosition = halberdItem.SpriteHandlePosition;
            }
            else
            {
                UnAlive();
            }
        }

        public override void AI()
        {
            float useTimeMulti = Player.GetAttackSpeed(DamageClass.Melee);
            int[] finalmaxProgress = new int[5];
            for (int i = 0; i < finalmaxProgress.Length; i++) finalmaxProgress[i] = (int)(baseMaxProgress[i] * (1 / useTimeMulti));

            switch (state)
            {
                case HalberdState.AttackOne:
                    armRotation= 0f;
                    //Directly forward
                    Projectile.rotation = MathHelper.PiOver4;
                    Projectile.rotation = (MathHelper.Pi * ARC_ANGLE_DEG / 180) * MathF.Sin((currentAnimProgress / finalmaxProgress[(int)(state)] * 2 * MathHelper.Pi));

                    break;
            }

            currentAnimProgress += 1;
            if (currentAnimProgress >= finalmaxProgress[(int)(state)]) currentAnimProgress = 0;
        }
    }
}
