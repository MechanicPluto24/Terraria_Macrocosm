using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases
{
    public abstract class GreatswordHeldProjectileItem : HeldProjectileItem<GreatswordHeldProjectile>
    {
        public abstract Vector2 SpriteHandlePosition { get; }

        public virtual GreatswordSwingStyle SwingStyle => new DefaultGreatswordSwingStyle();
        /// <summary>
        /// The lenght of the sword used for collision. <br/>
        /// If set to <c>null</c> the lenght will be taken from the sword's texture.
        /// </summary>
        public virtual float? SwordLenght => null;
        public virtual float SwordWidth => 10;
        public virtual int MaxCharge => 33;
        public virtual (float min, float max) ChargeBasedDamageRatio => (0.2f, 1f);
        public virtual (float min, float max) ChargeBasedDashAmount => (0f, 5.5f);
        public virtual string HeldProjectileTexturePath => Texture;

        public Texture2D HeldProjectileTexture => ModContent.Request<Texture2D>(HeldProjectileTexturePath, AssetRequestMode.ImmediateLoad).Value;
    }

    public class GreatswordHeldProjectile : HeldProjectile
    {
        public override string Texture => "Terraria/Images/Item_0";
        public sealed override HeldProjectileKillMode KillMode => HeldProjectileKillMode.Manual;
        private enum GreatswordState
        {
            Charge,
            Swing
        }

        private GreatswordState State
        {
            get => (GreatswordState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private int ChargeEndPlayerDirection
        {
            get => (int)Projectile.ai[1] > 0 ? 1 : -1;
            set => Projectile.ai[1] = value > 0 ? 1 : -1;
        }

        private int MaxCharge { get; set; }
        private int chargeTimer = 0;
        /// <summary>
        /// Charge ranging from 0 to 1.
        /// </summary>
        public float Charge => (float)chargeTimer / MaxCharge;

        public Texture2D GreatswordTexture { get; private set; }
        private GreatswordSwingStyle SwingStyle { get; set; }
        private float SwordWidth { get; set; }
        private float SwordLenght { get; set; }
        private (float min, float max) ChargeBasedDashAmount { get; set; }
        private (float min, float max) ChargeBasedDamageRatio { get; set; }
        private Vector2 SpriteHandlePosition { get; set; }

        private float armRotation = 0f;
        private float hitTimer = 0f;
        protected override void OnSpawn()
        {
            ChargeEndPlayerDirection = 1;

            if (item.ModItem is GreatswordHeldProjectileItem greatswordHeldProjectileItem)
            {
                if (Main.netMode != NetmodeID.Server)
                    GreatswordTexture = greatswordHeldProjectileItem.HeldProjectileTexture;

                SwingStyle = greatswordHeldProjectileItem.SwingStyle;
                SwordLenght = greatswordHeldProjectileItem.SwordLenght ?? 0.8f * MathF.Sqrt(
                    MathF.Pow(greatswordHeldProjectileItem.HeldProjectileTexture.Width, 2)
                    + MathF.Pow(greatswordHeldProjectileItem.HeldProjectileTexture.Height, 2)
                );
                SwordWidth = greatswordHeldProjectileItem.SwordWidth;
                ChargeBasedDashAmount = greatswordHeldProjectileItem.ChargeBasedDashAmount;
                ChargeBasedDamageRatio = greatswordHeldProjectileItem.ChargeBasedDamageRatio;
                SpriteHandlePosition = greatswordHeldProjectileItem.SpriteHandlePosition;
                MaxCharge = greatswordHeldProjectileItem.MaxCharge * (Projectile.extraUpdates + 1);
            }
            else
            {
                UnAlive();
            }
        }

        public override void AI()
        {
            switch (State)
            {
                case GreatswordState.Charge:
                    armRotation = MathHelper.Pi * 0.75f - Charge * MathHelper.PiOver4 * 0.25f;

                    Projectile.rotation = MathHelper.Pi - MathF.Sin(Charge * MathHelper.PiOver2) * MathHelper.PiOver4 * 0.25f;
                    if (chargeTimer < MaxCharge)
                    {
                        chargeTimer++;
                    }

                    Player.velocity.X *= 0.96f;

                    if (Projectile.owner == Main.myPlayer && !Player.channel)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);
                        ChargeEndPlayerDirection = Player.direction;
                        Player.velocity.X += Player.direction * MathHelper.Lerp(
                            ChargeBasedDashAmount.min,
                            ChargeBasedDashAmount.max,
                            Charge
                        );
                        State = GreatswordState.Swing;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            Projectile.netUpdate = true;
                            NetMessage.SendData(MessageID.PlayerControls, number: Main.myPlayer);
                        }
                    }

                    break;

                case GreatswordState.Swing:
                    Player.direction = ChargeEndPlayerDirection;
                    if (!SwingStyle.Update(ref armRotation, ref Projectile.rotation, Charge))
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

            hitTimer *= 0.95f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0;
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                Projectile.Center + (
                    (Projectile.rotation - MathHelper.PiOver4) * Player.direction + (Player.direction == -1 ? MathHelper.Pi : 0f)
                ).ToRotationVector2() * SwordLenght,
                SwordWidth,
                ref _
            );
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= MathHelper.Lerp(
                ChargeBasedDamageRatio.min,
                ChargeBasedDamageRatio.max,
                Charge
            );
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitTimer = 1f;
        }

        public override bool? CanHitNPC(NPC target) => State == GreatswordState.Swing ? null : false;

        public override bool CanHitPvp(Player target)
        {
            return base.CanHitPvp(target);
        }

        public override void Draw(Color lightColor)
        {
            SwingStyle.PreDrawSword(this, lightColor);

            Main.spriteBatch.Draw(
                GreatswordTexture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation * Player.direction,
                Player.direction == -1 ? new Vector2(GreatswordTexture.Width - SpriteHandlePosition.X, SpriteHandlePosition.Y) : SpriteHandlePosition,
                Projectile.scale,
                Player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );

            if (State == GreatswordState.Charge)
            {
                // TODO: Some charge up effect.
            }

            SwingStyle.PostDrawSword(this, lightColor);
        }
    }

    public class GreatswordGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool HasMark { get; private set; }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.ModProjectile is GreatswordHeldProjectile)
            {
                if (HasMark)
                {
                    modifiers.SourceDamage *= 2f;
                    HasMark = false;
                    return;
                }

                HasMark = true;
            }
        }
    }
}
