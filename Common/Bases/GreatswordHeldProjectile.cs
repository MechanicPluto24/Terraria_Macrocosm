using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Macrocosm.Common.Bases
{
    internal abstract class GreatswordHeldProjectileItem : HeldProjectileItem<GreatswordHeldProjectile>
    {
        public abstract Vector2 SpriteHandlePosition { get; }
        public virtual GreatswordSwingStyle SwingStyle => new DefaultGreatswordSwingStyle();
        /// <summary>
        /// The lenght of the sword used for collision. <br/>
        /// If set to <c>null</c> the lenght will be taken from the sword's texture.
        /// </summary>
        public virtual float? SwordLenght => null;
        public virtual float SwordWidth => 10;
        public virtual string HeldProjectileTexturePath => Texture;
        public Texture2D HeldProjectileTexture => ModContent.Request<Texture2D>(HeldProjectileTexturePath, AssetRequestMode.ImmediateLoad).Value;
    }

    internal class GreatswordHeldProjectile : HeldProjectile
    {
        public override string Texture => "Terraria/Images/Item_0";
        public sealed override HeldProjectileKillMode KillMode => HeldProjectileKillMode.Manual;
        private enum GreatswordState
        {
            Charge,
            Swing
        }

        private GreatswordState state;
        private GreatswordSwingStyle SwingStyle { get; set; }

        private const int MAX_CHARGE = 100;
        private int chargeTimer = 0;
        /// <summary>
        /// Charge ranging from 0 to 1.
        /// </summary>
        public float Charge => (float)chargeTimer / MAX_CHARGE;

        public GreatswordHeldProjectileItem GreatswordHeldProjectileItem { get; private set; }
        public Texture2D GreatswordTexture { get; private set; }
        private float armRotation = 0f;
        protected override void OnSpawn()
        {
            if (Item.ModItem is GreatswordHeldProjectileItem greatswordHeldProjectileItem) 
            {
                SwingStyle = greatswordHeldProjectileItem.SwingStyle;
                GreatswordTexture = greatswordHeldProjectileItem.HeldProjectileTexture;
                GreatswordHeldProjectileItem = greatswordHeldProjectileItem;
            }
            else
            {
                UnAlive();
            }
        }

        public override void AI()
        {
            switch (state)
            {
                case GreatswordState.Charge:
                    armRotation = MathHelper.Pi * 0.75f - Charge * MathHelper.PiOver4 * 0.25f;

                    Projectile.rotation = MathHelper.Pi - MathF.Sin(Charge * MathHelper.PiOver2) * MathHelper.PiOver4 * 0.25f;
                    if (chargeTimer < MAX_CHARGE)
                    {
                        chargeTimer++;
                    }

                    if (!Player.channel)
                    {
                        state = GreatswordState.Swing;
                    }

                    break;
                case GreatswordState.Swing:
                    if (!SwingStyle.Update(ref armRotation, ref Projectile.rotation))
                    {
                        UnAlive();
                    }
                    break;
            }

            float localArmRot = Player.direction * armRotation;
            Vector2 armDirection = (localArmRot + MathHelper.PiOver2).ToRotationVector2();

            Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter) + armDirection * 18;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, localArmRot);
            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, localArmRot + 0.1f * Player.direction);
        }

        public override (Vector2 startPosition, Vector2 endPosition, float width)? LineCollision => (
            Projectile.Center,
            Projectile.Center + (Projectile.rotation + MathHelper.PiOver4 * Player.direction).ToRotationVector2() * (
                GreatswordHeldProjectileItem.SwordLenght ?? GreatswordTexture.Width * 1.4142f
            ),
            GreatswordHeldProjectileItem.SwordWidth
        );

        public override void Draw(Color lightColor)
        {
            SwingStyle.PreDrawSword(this, lightColor);

            Vector2 origin = GreatswordHeldProjectileItem.SpriteHandlePosition;
            Main.spriteBatch.Draw(
                GreatswordTexture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation * Player.direction,
                Player.direction == -1 ? new Vector2(GreatswordTexture.Width - origin.X, origin.Y) : origin,
                Projectile.scale,
                Player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );

            SwingStyle.PostDrawSword(this, lightColor);
        }
    }
}
