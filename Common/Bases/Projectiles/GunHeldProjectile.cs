using Macrocosm.Common.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Projectiles
{
    public readonly struct GunHeldProjectileData
    {
        public GunHeldProjectileData() { }

        /// <summary> The <c>Y</c> of this should be set to the barrel <c>Y</c> on the sprite, the <c>X</c> should be somewhere near the grip point. </summary>
        public Vector2 GunBarrelPosition { get; init; } = Vector2.Zero;

        /// <summary> Should be the <c>Y</c> distance from barrel to grip holding position. </summary>
        public float CenterYOffset { get; init; } = 0f;

        /// <summary> The offset from where projectiles are shot </summary>
        public float MuzzleOffset { get; init; } = 0f;

        /// <summary> The initial recoil animation horizontal offset and rotation </summary>
        public (float horizontal, float rotation) Recoil { get; init; } = (6f, 0.2f);

        /// <summary> The diminishing multiplier of the recoil animation. Should be less than 1f </summary>
        public float RecoilDiminish { get; init; } = 0.92f;

        /// <summary> The animation frame where the recoil is applied </summary>
        public int RecoilStartFrame { get; init; } = 3;

        /// <summary> Whether the player also uses the back arm to hold the projectile </summary>
        public bool UseBackArm { get; init; } = true;

        /// <summary> Whether the gun actively follows the aim position after shooting. </summary>
        public bool FollowsAimPosition { get; init; } = true;
    }

    public abstract class GunHeldProjectileItem : HeldProjectileItem<GunHeldProjectile>
    {
        public virtual string HeldProjectileTexturePath => Texture;
        public Texture2D HeldProjectileTexture => ModContent.Request<Texture2D>(HeldProjectileTexturePath, AssetRequestMode.ImmediateLoad).Value;

        public abstract GunHeldProjectileData GunHeldProjectileData { get; }


        /// <summary>
        /// Allows you to modify this GunHeldProjectileItem's shooting mechanism. Return false to prevent vanilla's shooting code from running. Returns true by default.<br/>
        /// This method is called after the <see cref="ModifyShootStats"/> hook has had a chance to adjust the spawn parameters.
        /// </summary>
        /// <param name="player"> The player using the GunHeldProjectileItem. </param>
        /// <param name="source"> The shot projectile source's information. </param>
        /// <param name="position"> The center position of the shot projectile. </param>
        /// <param name="velocity"> The velocity of the shot projectile. </param>
        /// <param name="type"> The ID of the shot projectile. </param>
        /// <param name="damage"> The damage of the shot projectile. </param>
        /// <param name="knockback"> The knockback of the shot projectile. </param>
        /// <param name="heldProjectile"> The gun's associated held projectile </param>
        /// <returns></returns>
        public virtual bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, GunHeldProjectile heldProjectile)
        {
            return true;
        }
    }

    public class GunHeldProjectile : HeldProjectile
    {
        public override string Texture => "Terraria/Images/Item_0";
        public override HeldProjectileKillMode KillMode => HeldProjectileKillMode.OnAnimationEnd;

        private GunHeldProjectileData GunHeldProjectileData { get; set; }

        private Texture2D GunTexture { get; set; }
        private Vector2 DirectionToAim
        {
            get
            {
                return new(
                    Projectile.ai[0],
                    Projectile.ai[1]
                );
            }
            set
            {
                Projectile.ai[0] = value.X;
                Projectile.ai[1] = value.Y;
            }
        }

        private Vector2 aim;
        private Vector2 currentRecoil;
        private int frame;

        public void SetAim(Vector2 aim) => this.aim = aim;

        public void UpdateCenterAndDirection(bool updateForAim)
        {
            if (Main.myPlayer == Projectile.owner && updateForAim)
            {
                Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter) + new Vector2(-4 * Player.direction, -2);
                DirectionToAim = Projectile.Center.DirectionTo(aim);
                Projectile.Center += DirectionToAim.RotatedBy(-MathHelper.PiOver2 * Player.direction) * GunHeldProjectileData.CenterYOffset;
                DirectionToAim = (Projectile.Center - DirectionToAim * 50).DirectionTo(aim);

                Projectile.netUpdate = true;
            }
            else
            {
                Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter) + new Vector2(-4 * Player.direction, -2);
                Projectile.Center += DirectionToAim.RotatedBy(-MathHelper.PiOver2 * Player.direction) * GunHeldProjectileData.CenterYOffset;
            }
        }

        protected override void OnSpawn()
        {
            if (item.ModItem is not GunHeldProjectileItem gunHeldProjectileItem)
            {
                UnAlive();
                return;
            }

            if (Main.netMode != NetmodeID.Server)
                GunTexture = gunHeldProjectileItem.HeldProjectileTexture;

            GunHeldProjectileData = gunHeldProjectileItem.GunHeldProjectileData;

            aim = Main.MouseWorld;
            UpdateCenterAndDirection(updateForAim: true);
            Projectile.rotation = DirectionToAim.ToRotation();

            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 shootPosition = Projectile.Center;

                Vector2 extendedPosition = Projectile.Center + DirectionToAim * GunHeldProjectileData.MuzzleOffset;

                if (Collision.CanHit(shootPosition, 0, 0, extendedPosition, 0, 0))
                {
                    shootPosition = extendedPosition;
                }

                Vector2 velocity = DirectionToAim * Projectile.velocity.Length();

                if (
                    gunHeldProjectileItem.Shoot(
                        Player,
                        new EntitySource_ItemUse_WithAmmo(Player, item, item.useAmmo),
                        shootPosition,
                        velocity,
                        ShootProjectileType,
                        Projectile.damage,
                        Projectile.knockBack,
                        this
                        )
                    )
                {
                    Projectile.NewProjectile(
                        new EntitySource_ItemUse_WithAmmo(Player, item, item.useAmmo),
                        shootPosition,
                        velocity,
                        ShootProjectileType,
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner
                    );
                }
            }
        }

        public override void AI()
        {
            bool recoil = ClientConfig.Instance.GunRecoilEffects;

            if (recoil && GunHeldProjectileData.FollowsAimPosition)
                aim = Main.MouseWorld;

            UpdateCenterAndDirection(updateForAim: recoil);

            if (recoil)
            {
                float mouseDirectionRotation = DirectionToAim.ToRotation();
                int newPlayerDirection = MathF.Abs(mouseDirectionRotation) <= MathHelper.PiOver2 ? 1 : -1;
                if (newPlayerDirection != Player.direction)
                {
                    Player.ChangeDir(newPlayerDirection);
                    currentRecoil.Y *= -1;
                }

                Projectile.rotation = mouseDirectionRotation - currentRecoil.Y * Player.direction;

                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

                if (GunHeldProjectileData.UseBackArm)
                {
                    Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);
                }

                if (frame++ == Projectile.extraUpdates * GunHeldProjectileData.RecoilStartFrame)
                {
                    currentRecoil = new Vector2(GunHeldProjectileData.Recoil.horizontal, GunHeldProjectileData.Recoil.rotation);
                }

                currentRecoil *= GunHeldProjectileData.RecoilDiminish;
            }
        }

        public override void Draw(Color lightColor)
        {
            float recoil = ClientConfig.Instance.GunRecoilEffects ? currentRecoil.X : 0f;
            // 19 here is for the arm length.
            Vector2 normOrigin = GunHeldProjectileData.GunBarrelPosition + Vector2.UnitX * (recoil - 19);
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
