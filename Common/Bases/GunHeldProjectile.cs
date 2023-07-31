using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.GameContent;
using Mono.Cecil;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Macrocosm.Common.Bases
{
    internal struct GunHeldProjectileData
    {
        public GunHeldProjectileData() { }

        /// <summary>
        /// The <c>Y</c> of this should be set to the barrel <c>Y</c> on the sprite, the <c>X</c> should be somewhere near the grip point.
        /// </summary>
        public Vector2 GunBarrelPosition { get; init; } = Vector2.Zero;
        /// <summary>
        /// Should be the <c>Y</c> distance from barrel to grip holding position.
        /// </summary>
        public float CenterYOffset { get; init; } = 0f;
        public float MuzzleOffset { get; init; } = 0f;
        public (float horizontal, float rotation) Recoil { get; init; } = (6f, 0.2f);
        public float RecoilDiminish { get; init; } = 0.92f;
    }

    internal abstract class GunHeldProjectileItem : HeldProjectileItem<GunHeldProjectile>
    {
        public abstract GunHeldProjectileData GunHeldProjectileData { get; }
        public virtual string HeldProjectileTexturePath => Texture;

        public new virtual bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }

        public Texture2D HeldProjectileTexture => ModContent.Request<Texture2D>(HeldProjectileTexturePath, AssetRequestMode.ImmediateLoad).Value;
    }

    internal class GunHeldProjectile : HeldProjectile
    {
        public override string Texture => "Terraria/Images/Item_0";
        public override HeldProjectileKillMode KillMode => HeldProjectileKillMode.OnAnimationEnd;

        private GunHeldProjectileData GunHeldProjectileData { get; set; }
        private Texture2D GunTexture { get; set; }
        private Vector2 DirectionToMouse
        {
            get
            {
                return new Vector2(Projectile.ai[1], Projectile.ai[2]);
            }
            set
            {
                Projectile.ai[1] = value.X;
                Projectile.ai[2] = value.Y;
            }
        }

        private Vector2 currentRecoil;

        private void UpdateCenterAndDirection()
        {
            Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter) + new Vector2(-4 * Player.direction, -2);
            DirectionToMouse = Projectile.Center.DirectionTo(Main.MouseWorld);
            Projectile.Center += DirectionToMouse.RotatedBy(-MathHelper.PiOver2 * Player.direction) * GunHeldProjectileData.CenterYOffset;
            DirectionToMouse = (Projectile.Center - DirectionToMouse * 50).DirectionTo(Main.MouseWorld);

            Projectile.netUpdate = true;
        }

        protected override void OnSpawn()
        {
            if (Item.ModItem is not GunHeldProjectileItem gunHeldProjectileItem)
            {
                UnAlive();
                return;
            }

            GunTexture = gunHeldProjectileItem.HeldProjectileTexture;
            GunHeldProjectileData = gunHeldProjectileItem.GunHeldProjectileData;

            currentRecoil = new Vector2(GunHeldProjectileData.Recoil.horizontal, GunHeldProjectileData.Recoil.rotation);

            if (Main.myPlayer != Projectile.owner)
            {
                return;
            }

            UpdateCenterAndDirection();

            Vector2 shootPosition = Projectile.Center;
            Vector2 extendedPosition = Projectile.Center + DirectionToMouse * GunHeldProjectileData.MuzzleOffset;
            if (Collision.CanHit(shootPosition, 0, 0, extendedPosition, 0, 0))
            {
                shootPosition = extendedPosition;
            }

            Vector2 velocity = DirectionToMouse * Projectile.velocity.Length();

            if (
                gunHeldProjectileItem.Shoot(
                    Player,
                    Source,
                    shootPosition,
                    velocity,
                    ShootProjectileType,
                    Projectile.damage,
                    Projectile.knockBack
                )
                )
            {
                Projectile.NewProjectile(
                    Source, 
                    shootPosition, 
                    velocity, 
                    ShootProjectileType, 
                    Projectile.damage, 
                    Projectile.knockBack, 
                    Projectile.owner
                );
            }
            
        }

        public override void AI()
        {
            UpdateCenterAndDirection();
            Projectile.rotation = DirectionToMouse.ToRotation() + -currentRecoil.Y * Player.direction;

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);

            currentRecoil *= GunHeldProjectileData.RecoilDiminish;
        }

        public override void Draw(Color lightColor)
        {
            // 19 here is for the arm length.
            Vector2 normOrigin = GunHeldProjectileData.GunBarrelPosition + Vector2.UnitX * (currentRecoil.X - 19);
            Main.EntitySpriteDraw(
                GunTexture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation + (Player.direction == -1 ? MathHelper.Pi : 0),
                Player.direction == -1 ? new Vector2(GunTexture.Width - normOrigin.X, normOrigin.Y) : normOrigin,
                Projectile.scale,
                Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0
            );
        }
    }
}
