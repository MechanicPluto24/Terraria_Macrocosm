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
        public virtual string HeldProjectileTexturePath => Texture;
        public Texture2D HeldProjectileTexture => ModContent.Request<Texture2D>(HeldProjectileTexturePath, AssetRequestMode.ImmediateLoad).Value;
    }

    internal class GreatswordHeldProjectile : HeldProjectile
    {
        public override string Texture => "Terraria/Images/Item_0";
        public sealed override HeldProjectileKillMode KillMode => HeldProjectileKillMode.Manual;

        private GreatswordHeldProjectileItem greatswordHeldProjectileItem;
        protected override void OnSpawn()
        {
            if (Item.ModItem is GreatswordHeldProjectileItem greatswordHeldProjectileItem) 
            {
                this.greatswordHeldProjectileItem = greatswordHeldProjectileItem;
            }
            else
            {
                shouldDie = true;
            }
        }

        private enum GreatswordState
        {
            Charge,
            Swing
        }

        private GreatswordState State { get; set; }

        public override void AI()
        {
            if (!Player.channel)
            {
                shouldDie = true;
            }

            switch (State)
            {
                case GreatswordState.Charge:
                    Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter) + new Vector2(-Player.direction, -1f) * 30f;
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Player.direction == -1 ? -MathHelper.Pi * 0.75f : MathHelper.Pi * 0.75f);
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Player.direction == -1 ? -MathHelper.Pi * 0.75f : MathHelper.Pi * 0.75f);
                    Projectile.rotation = Player.direction * MathHelper.Pi;
                    break;
                case GreatswordState.Swing:
                    break;
            }
        }

        public override void Draw(Color lightColor)
        {
            Texture2D texture = greatswordHeldProjectileItem.HeldProjectileTexture;
            Vector2 origin = greatswordHeldProjectileItem.SpriteHandlePosition;
            Main.spriteBatch.Draw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White,
                Projectile.rotation,
                Player.direction == -1 ? new Vector2(texture.Width - origin.X, origin.Y) : origin, 
                Projectile.scale,
                Player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );
        }
    }
}
