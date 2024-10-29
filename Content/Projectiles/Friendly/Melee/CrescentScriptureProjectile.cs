using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class CrescentScriptureProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
            Main.projFrames[Type] = 1;
        }

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

            Projectile.extraUpdates = 3;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 999;
        }
        private Player Player => Main.player[Projectile.owner];
        private ref float SwingDirection => ref Projectile.ai[0];
        private ref float Arc => ref Projectile.ai[1];
        private CrescentScripture blade;
        private bool despawn;
        private int runeTimer;
        private bool shot;
        float transparency = 0f;
        private Vector2 aim;
        private bool spawned = false;
        private int shootTimer = 0;
        public override void OnSpawn(IEntitySource source)
        {
            blade = (source as EntitySource_ItemUse_WithAmmo).Item.ModItem as CrescentScripture;

            SwingDirection = blade.SwingDirection;
            Arc = MathHelper.TwoPi * 0.75f;

            Projectile.netUpdate = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if (!spawned)
            {
                Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld);
                spawned = true;
            }


            if (!despawn)
            {
                Projectile.timeLeft = 2;
            }


            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Player.heldProj = Projectile.whoAmI;
            Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter) + new Vector2(Player.direction * -3, -1);

            if (Player.noItems || Player.CCed || Player.ItemAnimationEndingOrEnded || Player.HeldItem.type != ModContent.ItemType<CrescentScripture>())
            {
                despawn = true;
                return;
            }

            if (Main.myPlayer == Player.whoAmI)
            {
                Projectile.netUpdate = true;
            }

            var progress = 1f - (float)Player.itemAnimation / Player.itemAnimationMax;
            var x = progress - 1f;

            if (transparency < 1f)
                transparency += 0.005f;

            Projectile.rotation = Projectile.velocity.ToRotation() + (0.5f * Arc - Arc * (MathF.Sin((x * x * x - 0.5f) * MathHelper.Pi) + 1f) / 2f) * SwingDirection;

            if (!shot && progress > 0.2f && Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitY), ModContent.ProjectileType<CrescentScriptureSlash>(), (int)(Projectile.damage / 2), 1f, Main.myPlayer, 45f);

                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(9f, 12f), ModContent.ProjectileType<LuminiteRune>(), (int)(Projectile.damage / 4), 1f, Main.myPlayer, 1f);

                }
                shot = true;
            }
        }

        private SpriteBatchState state;

        public override bool PreDraw(ref Color lightColor)
        {
            var rotation = Projectile.rotation + (Player.direction == 1 ? MathHelper.PiOver4 : MathHelper.Pi * 0.75f);
            float rotation2 = rotation + (SwingDirection > 0 ? (Player.direction == 1 ? -((MathHelper.PiOver4 / 2) + (MathHelper.PiOver4)) : MathHelper.Pi) : (Player.direction == 1 ? 0 : ((MathHelper.PiOver4 / 2) + (MathHelper.PiOver4)) + MathHelper.Pi));
            var origin = new Vector2(Player.direction == 1 ? 10 : 67, 67);

            Player player = Main.player[Projectile.owner];
            Vector2 position = player.Center - Main.screenPosition;
            Texture2D texture = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "Swing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 Origin = texture.Frame(1, 4).Size() / 2f;
            float scale = Projectile.scale * 1.3f;
            SpriteEffects spriteEffects = ((!(SwingDirection >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None); // Flip the sprite based on the direction it is facing.

            float lerpTime = Utils.Remap(transparency, 0f, 0.6f, 0f, 1f) * Utils.Remap(transparency, 0.6f, 1f, 1f, 0f);
            float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);
            float progressScale = Utils.Remap(transparency, 0f, 0.6f, 0f, 1f) * Utils.Remap(transparency, 0.6f, 1f, 1f, 0f);
            Color color = new(94, 229, 163);
            Color backDarkColor = (color * 0.8f).WithOpacity(progressScale);
            Color middleMediumColor = color;
            Color frontLightColor = (color * 1.4f).WithAlpha(255);
            Color secondaryColor = (new Color(164, 101, 124)).WithOpacity(progressScale);

            Color whiteTimesLerpTime = Color.White * lerpTime * 0.5f;
            whiteTimesLerpTime.A = (byte)(whiteTimesLerpTime.A * (1f - lightingColor));
            Color faintLightingColor = whiteTimesLerpTime * lightingColor * 0.5f;
            faintLightingColor.G = (byte)(faintLightingColor.G * lightingColor);
            faintLightingColor.B = (byte)(faintLightingColor.R * (0.25f + lightingColor * 0.75f));

            // Back part, pink
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), secondaryColor * transparency, rotation2 + SwingDirection * MathHelper.Pi / 2 * -1f * (1f - (0.3f + 0.7f * transparency)), Origin, scale, spriteEffects, 0f);
            // Back part, green
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), backDarkColor * transparency, rotation2 + SwingDirection * MathHelper.Pi / 4 * -1f * (1f - transparency), Origin, scale, spriteEffects, 0f);
            // Very faint part affected by the light color
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), faintLightingColor * 0.15f, rotation2 + SwingDirection * 0.01f, Origin, scale, spriteEffects, 0f);
            // Middle part
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), middleMediumColor * transparency * 0.8f, rotation2, Origin, scale, spriteEffects, 0f);
            // Front part
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), frontLightColor * transparency * 0.5f, rotation2, Origin, scale * 0.975f, spriteEffects, 0f);
            // Thin top line (final frame)
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), Color.White * 0.6f * transparency, rotation2 + SwingDirection * 0.01f, Origin, scale, spriteEffects, 0f);
            // Thin middle line (final frame)
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), Color.White * 0.5f * transparency, rotation2 + SwingDirection * -0.05f, Origin, scale * 0.8f, spriteEffects, 0f);
            // Thin bottom line (final frame)
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), Color.White * 0.4f * transparency, rotation2 + SwingDirection * -0.1f, Origin, scale * 0.6f, spriteEffects, 0f);

            // This draws some sparkles around the circumference of the swing.
            for (float i = 0f; i < 8f; i += 1f)
            {
                float edgeRotation = rotation2 + SwingDirection * i * (MathHelper.Pi * -2f) * 0.045f + Utils.Remap(transparency, 0f, 1f, 0f, MathHelper.PiOver4) * SwingDirection;
                Vector2 drawPos = position + edgeRotation.ToRotationVector2() * ((float)texture.Width * 0.5f - 6f) * scale;
                Utility.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, new Color(255, 255, 255, 0) * lerpTime * (i / 8f), middleMediumColor, transparency, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(transparency, 0f, 1f, 2f, 0f)) * scale, Vector2.One * scale * 1.5f * (1f - i / 8f));
            }

            // This draws a large star sparkle at the front of the projectile.
            Vector2 drawPos2 = position + (rotation2 + Utils.Remap(transparency, 0f, 1f, 0f, MathHelper.Pi / 2f) * SwingDirection).ToRotationVector2() * ((float)texture.Width * 0.5f - 4f) * scale;
            Utility.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos2, new Color(255, 255, 255, 0) * lerpTime * 0.5f, middleMediumColor, transparency, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(transparency, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale);

            Main.spriteBatch.Draw(
               TextureAssets.Projectile[Type].Value,
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

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public void CreateBlood(float damage)
        {
            Vector2 ProjSpawnPosition = Projectile.Center + new Vector2((Projectile.width / 2), 0).RotatedBy(Projectile.rotation);
            if (!Main.rand.NextBool(50))
            {
                for (int i = 0; i < 8; i++)
                {
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), ProjSpawnPosition, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10f, 15f) * (float)((damage / Projectile.damage) / 1.5), ModContent.ProjectileType<LunarBlood>(), (int)(damage * 0.5f), 0, Projectile.owner, 1f);
                }
            }
            else
            {
                for (int i = 1; i < 9; i++)
                {
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), ProjSpawnPosition, -Vector2.UnitY.RotatedBy(((MathHelper.PiOver4 / 2) * (i - 1)) - MathHelper.PiOver2 + (MathHelper.PiOver4 / 4)) * 15f * (float)((damage / Projectile.damage) / 1.5), ModContent.ProjectileType<LunarBlood>(), (int)(damage * 2f), 0, Projectile.owner, (float)(i + 1));
                }
            }
        }
    }
}