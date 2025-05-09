using Macrocosm.Common.Bases.NPCs;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Projectiles
{
    public abstract class GreatswordHeldProjectileItem : HeldProjectileItem<GreatswordHeldProjectile>
    {
        public enum UseBehavior
        {
            None,
            Swing,
            Charge
        }

        public abstract Vector2 SpriteHandlePosition { get; }
        public virtual UseBehavior LeftClickBehavior => UseBehavior.Swing;
        public virtual UseBehavior RightClickBehavior => UseBehavior.Charge;
        public sealed override bool RightClickUse => RightClickBehavior != UseBehavior.None && Main.mouseRight;

        /// <summary>
        /// The length of the sword used for collision. <br/>
        /// If set to <c>null</c> the length will be taken from the sword's texture.
        /// </summary>
        public virtual float SwordLength => 0.8f * MathF.Sqrt(MathF.Pow(HeldProjectileTexture?.Width ?? 1, 2) + MathF.Pow(HeldProjectileTexture?.Height ?? 1, 2));
        public virtual float SwordWidth => 10;
        public virtual int MaxCharge => 33;
        public virtual int MaxSwingRestTime => 8;
        public virtual (float min, float max) ChargeBasedDamageRatio => (1f, 1.6f);
        public virtual (float min, float max) ChargeBasedDashAmount => (0f, 5.5f);
        public virtual string HeldProjectileTexturePath => Texture;
        public Texture2D HeldProjectileTexture => ModContent.Request<Texture2D>(HeldProjectileTexturePath, AssetRequestMode.ImmediateLoad).Value;

        public virtual void OnMaxCharge(GreatswordHeldProjectile proj, Player player) { }
        public virtual void OnRelease(GreatswordHeldProjectile proj, Player player, int damage, float charge) { }
        public virtual bool UpdateSwing(GreatswordHeldProjectile proj, Player player, float charge, ref float timer, ref float armRotation, float initialArmRotation)
        {
            int maxTimer = Item.useAnimation * (proj.Projectile.extraUpdates + 1);
            float swingProgress = timer / maxTimer;
            if (timer++ >= maxTimer)
            {
                if (timer >= maxTimer + MaxSwingRestTime)
                    return false;

                armRotation += 0.01f;
                return true;
            }

            float progress = MathF.Pow(MathF.Sin(MathHelper.PiOver2 * swingProgress), 4);

            armRotation = MathHelper.Lerp(initialArmRotation, initialArmRotation + MathHelper.Pi * 1.15f, progress);
            proj.Projectile.rotation = MathHelper.Lerp(proj.Projectile.rotation, armRotation + MathHelper.Pi * 0.6f, progress);

            return true;
        }

        public virtual bool PreDrawSword(GreatswordHeldProjectile greatswordProjectile, Color lightColor, ref Color? drawColor) { return true; }
        public virtual void PostDrawSword(GreatswordHeldProjectile greatswordProjectile, Color lightColor) { }
    }

    public class GreatswordHeldProjectile : HeldProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;
        public sealed override HeldProjectileKillMode KillMode => HeldProjectileKillMode.Manual;
        public enum GreatswordState
        {
            Charge,
            Swing
        }

        public GreatswordState State
        {
            get => (GreatswordState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        public int ChargeEndPlayerDirection
        {
            get => (int)Projectile.ai[1] > 0 ? 1 : -1;
            set => Projectile.ai[1] = value > 0 ? 1 : -1;
        }

        public GreatswordHeldProjectileItem Sword { get; set; }

        /// <summary> Charge ranging from 0 to 1. </summary>
        public float Charge => MathHelper.Clamp((float)chargeTimer / MaxCharge, 0, 1);
        public int MaxCharge { get; set; }

        private int chargeTimer = 0;
        private float armRotation = 0f;
        private bool released = false;
        protected float swingTimer = 1f;
        private GreatswordHeldProjectileItem.UseBehavior currentUseBehavior;
        protected override void OnSpawn()
        {
            ChargeEndPlayerDirection = 1;
            Projectile.usesOwnerMeleeHitCD = true;

            if (item.ModItem is GreatswordHeldProjectileItem sword)
            {
                Sword = sword;
                MaxCharge = sword.MaxCharge * (Projectile.extraUpdates + 1);
                currentUseBehavior = Main.mouseRight && !Main.mouseLeft ? sword.RightClickBehavior : sword.LeftClickBehavior;
                switch(currentUseBehavior)
                {
                    case GreatswordHeldProjectileItem.UseBehavior.Swing:
                        State = GreatswordState.Swing;
                        break;
                    case GreatswordHeldProjectileItem.UseBehavior.Charge:
                        State = GreatswordState.Charge;
                        break;
                    case GreatswordHeldProjectileItem.UseBehavior.None:
                    default:
                        Despawn();
                        break;
                }
            }
            else
            {
                Despawn();
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
                        if (chargeTimer == MaxCharge)
                            Sword.OnMaxCharge(this, Player);
                    }                        

                    Player.velocity.X *= 0.96f;

                    bool holding = Player.channel && Sword.LeftClickBehavior == GreatswordHeldProjectileItem.UseBehavior.Charge || Main.mouseRight && Sword.RightClickBehavior == GreatswordHeldProjectileItem.UseBehavior.Charge;
                    if (Projectile.owner == Main.myPlayer && !holding)
                    {
                        State = GreatswordState.Swing;
                        ChargeEndPlayerDirection = Player.direction;
                    }

                    break;

                case GreatswordState.Swing:

                    if (currentUseBehavior == GreatswordHeldProjectileItem.UseBehavior.Charge)
                        Player.direction = ChargeEndPlayerDirection;

                    if (!released && Player.whoAmI == Main.myPlayer)
                    {
                        if(currentUseBehavior == GreatswordHeldProjectileItem.UseBehavior.Swing)
                        {
                            armRotation = MathHelper.Pi * 0.75f;
                            Projectile.rotation = MathHelper.Pi;
                        }

                        Player.velocity.X += Player.direction * MathHelper.Lerp(Sword.ChargeBasedDashAmount.min, Sword.ChargeBasedDashAmount.max, Charge);

                        int damage = (int)(Projectile.damage * MathHelper.Lerp(Sword.ChargeBasedDamageRatio.min, Sword.ChargeBasedDamageRatio.max, Charge));
                        Sword.OnRelease(this, Player, damage, Charge);

                        if (Sword.Item.UseSound.HasValue)
                            SoundEngine.PlaySound(Sword.Item.UseSound, Projectile.Center);

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            Projectile.netUpdate = true;
                            NetMessage.SendData(MessageID.PlayerControls, number: Main.myPlayer);
                        }

                        released = true;
                    }

                    if (!Sword.UpdateSwing(this, Player, Charge, ref swingTimer, ref armRotation, MathHelper.Pi * 0.75f))
                        Despawn();

                    break;
            }

            float localArmRot = Player.direction * armRotation;
            Vector2 armDirection = (localArmRot + MathHelper.PiOver2).ToRotationVector2();

            Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter) + armDirection * 18;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, localArmRot);
            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, localArmRot + 0.1f * Player.direction);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0;
            Vector2 hitboxEnd = Projectile.Center + ((Projectile.rotation - MathHelper.PiOver4) * Player.direction + (Player.direction == -1 ? MathHelper.Pi : 0f)).ToRotationVector2() * (Sword.SwordLength);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, hitboxEnd, Sword.SwordWidth, ref _);
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= MathHelper.Lerp(Sword.ChargeBasedDamageRatio.min, Sword.ChargeBasedDamageRatio.max, Charge);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Sword.OnHitNPC(Player, target, hit, damageDone);
        public override bool? CanHitNPC(NPC target) => State == GreatswordState.Swing ? null : false;

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => Sword.OnHitPvp(Player, target, info);
        public override bool CanHitPvp(Player target) => State == GreatswordState.Swing;

        public override void Draw(Color lightColor)
        {
            Color? drawColor = null;
            if (Sword.PreDrawSword(this, lightColor, ref drawColor))
            {
                Main.EntitySpriteDraw(
                    Sword.HeldProjectileTexture,
                    Projectile.Center - Main.screenPosition,
                    null,
                    drawColor is null ? lightColor : drawColor.Value,
                    Projectile.rotation * Player.direction,
                    Player.direction == -1 ? new Vector2(Sword.HeldProjectileTexture.Width - Sword.SpriteHandlePosition.X, Sword.SpriteHandlePosition.Y) : Sword.SpriteHandlePosition,
                    Projectile.scale,
                    Player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0f
                );
            }

            Sword.PostDrawSword(this, lightColor);
        }
    }
}
