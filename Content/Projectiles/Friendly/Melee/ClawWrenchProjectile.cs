using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ClawWrenchProjectile : ModProjectile
    {
        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = 40;
        }

        public Player OwnerPlayer => Main.player[Projectile.owner];

        public ref float SwingTime => ref Projectile.ai[0];

        public override void OnSpawn(IEntitySource source)
        {
            spawned = false;
        }
        bool spawned = false;

        public override void AI()
        {
            if (!OwnerPlayer.active || OwnerPlayer.dead || OwnerPlayer.CCed || OwnerPlayer.noItems)
                return;

            if (!spawned)
            {
                SwingTime /= OwnerPlayer.GetAttackSpeed(DamageClass.Melee);
                Projectile.timeLeft = (int)SwingTime;
                spawned = true;
            }

            float holdOffset = 2f;

            Projectile.spriteDirection = OwnerPlayer.direction;

            float swingProgress = Utils.GetLerpValue(SwingTime, 0f, Projectile.timeLeft);
            float start = Projectile.velocity.ToRotation() - MathHelper.PiOver2 * Projectile.spriteDirection;
            float end = Projectile.velocity.ToRotation() + MathHelper.PiOver2 * Projectile.spriteDirection;
            float rotation = MathHelper.Lerp(start, end, swingProgress);

            if (OwnerPlayer.whoAmI == Main.myPlayer)
            {
                Projectile.Center = OwnerPlayer.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation - MathHelper.PiOver2) + rotation.ToRotationVector2() * holdOffset;
                Projectile.netUpdate = true;
            }

            Projectile.rotation = (Projectile.Center - OwnerPlayer.Center).ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi + MathHelper.PiOver4 : -0.6f);
            OwnerPlayer.ChangeDir(Math.Sign(Projectile.velocity.X));
            OwnerPlayer.heldProj = Projectile.whoAmI;
            OwnerPlayer.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation - MathHelper.PiOver2);
            OwnerPlayer.itemTime = 2;
            OwnerPlayer.itemAnimation = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 polar = Utility.PolarVector(35, Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver4 : MathHelper.PiOver2 + MathHelper.PiOver4));
            hitbox.X -= (int)polar.X;
            hitbox.Y -= (int)polar.Y;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 orig = texture.Size() / 2 + (Projectile.spriteDirection == -1 ? new Vector2(16, 16) : new Vector2(-16f, 16f));
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, Projectile.rotation, orig, 1f, effect, 0);
            return false;
        }
    }
}