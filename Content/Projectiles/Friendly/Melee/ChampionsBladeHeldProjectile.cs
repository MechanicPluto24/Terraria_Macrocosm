using Macrocosm.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    internal class ChampionsBladeHeldProjectile : ModProjectile
    {
        public override string Texture => "Macrocosm/Content/Items/Weapons/Melee/ChampionsBlade";

        public override void SetDefaults()
        {
            Projectile.width = 0;
            Projectile.height = 0;

            Projectile.aiStyle = -1;

            Projectile.DamageType = DamageClass.Melee;

            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.extraUpdates = 2;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 999;
        }

        private Player Player => Main.player[Projectile.owner];
        private ref float Arc => ref Projectile.ai[1];
        private ref float LastRotation => ref Projectile.ai[0];
        private ChampionsBlade item;

        // So that the weapon doesn't "blink" during continuous use.
        private bool despawn;

        public override void OnSpawn(IEntitySource source)
        {
            // we just gon assume this true !
            item = (source as EntitySource_ItemUse_WithAmmo).Item.ModItem as ChampionsBlade;

            item.lastDirection = -item.lastDirection;
            LastRotation = item.lastRotation;
            Arc = Projectile.velocity.ToRotation() + item.lastDirection * Main.rand.NextFloat(MathHelper.PiOver2, MathHelper.Pi) - item.lastRotation;

            Projectile.netUpdate = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            if (!despawn)
            {
                Projectile.timeLeft = 2;
            }

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Player.heldProj = Projectile.whoAmI;
            Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter) + new Vector2(Player.direction * -3, -1);

            if (Player.noItems || Player.CCed || Player.ItemAnimationEndingOrEnded || Player.HeldItem.type != ModContent.ItemType<ChampionsBlade>())
            {
                despawn = true;
                if (item is not null)
                {
                    item.lastRotation = Projectile.rotation;
                }

                return;
            }

            var x = 1f - (float)Player.itemAnimation / Player.itemAnimationMax - 1f;
            Projectile.rotation = LastRotation + Arc * (MathF.Sin((x * x - 0.5f) * MathHelper.Pi) + 1f) / 2f;


            /*            if (Main.netMode != NetmodeID.MultiplayerClient)
                        {

                            if (Main.rand.NextBool(30))
                            {
                                Projectile.NewProjectile(
                                    Projectile.GetSource_FromAI(),
                                    Projectile.Center,
                                    Projectile.velocity.SafeNormalize(Vector2.Zero) * 23f,
                                    ModContent.ProjectileType<ChampionsBladeBoltProjectile>(),
                                    40,
                                    2f);
                            }
                        }
            */
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                Projectile.Center + Projectile.rotation.ToRotationVector2() * 80f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var rotation = Projectile.rotation + (Player.direction == 1 ? MathHelper.PiOver4 : MathHelper.Pi * 0.75f);
            var origin = new Vector2(Player.direction == 1 ? 10 : 67, 67);

            Main.spriteBatch.Draw(
                texture,
                Projectile.Center + Projectile.rotation.ToRotationVector2() * 16f - Main.screenPosition,
                null,
                lightColor,
                rotation,
                origin,
                Projectile.scale,
                Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0
            );

            return false;
        }
    }
}
