using Macrocosm.Common.CrossMod;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    internal class ChampionsBladeHeldProjectile : ModProjectile
    {
        private static Asset<Texture2D> itemTexture;

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
        public ref float Timer => ref Projectile.localAI[0];
        public float MaxTime => Player.itemTimeMax;

        private ChampionsBlade blade; // Client side only.
        private int shots;
        private int hitStacks;
        private bool despawn; // So that the weapon doesn't "blink" during continuous use.
        private OldPositionCache? tipOldPositions;
        private bool spawned;

        public override void OnSpawn(IEntitySource source)
        {
            if(source != null)
                blade = (source as EntitySource_ItemUse_WithAmmo).Item.ModItem as ChampionsBlade;

            hitStacks = blade?.HitStacks ?? 5;
            SwingDirection = blade?.SwingDirection ?? 1;
            Arc = MathHelper.Pi + MathHelper.PiOver2 * ((float)hitStacks / ChampionsBlade.MaxStacks);

            Projectile.netUpdate = true;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (!spawned)
            {
                Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld);
                Projectile.netUpdate = true;
                spawned = true;
            }

            if (!despawn)
            {
                Projectile.timeLeft = 2;
            }

            Timer++;

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Player.heldProj = Projectile.whoAmI;
            Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter) + new Vector2(Player.direction * -3, -1);

            if (Player.noItems || Player.CCed || Player.ItemAnimationEndingOrEnded || Player.HeldItem.type != ModContent.ItemType<ChampionsBlade>())
            {
                despawn = true;
                if (blade is not null)
                {
                    blade.HitStacks = hitStacks;
                }

                return;
            }

            var progress = 1f - (float)Player.itemAnimation / Player.itemAnimationMax;
            var x = progress - 1f;
            Projectile.rotation = Projectile.velocity.ToRotation() + (0.5f * Arc - Arc * (MathF.Sin((x * x * x - 0.5f) * MathHelper.Pi) + 1f) / 2f) * SwingDirection;
               
            float maxShots = 2 + 5 * ((float)hitStacks / ChampionsBlade.MaxStacks);
            if (shots < maxShots && Main.netMode != NetmodeID.MultiplayerClient && progress > 0.3f && progress < 0.5f && Main.rand.NextBool(2))
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromAI(),
                    Projectile.Center,
                    Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4 / 2f) * 12f,
                    ModContent.ProjectileType<ChampionsBladeBoltProjectile>(),
                    (int)(Projectile.damage * 0.5f),
                    2f
                );
                shots++;
            }

            var tipPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * SwordLength * 0.6f;
            tipOldPositions ??= new(20, tipPosition);
            tipOldPositions.Value.Add(tipPosition);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Redemption.Decapitation(target, ref damageDone, ref hit.Crit);

            hitStacks = Math.Min(hitStacks + 1, ChampionsBlade.MaxStacks);
            blade.ResetTimer = 0;

            //Main.instance.CameraModifiers.Add(new ScreenshakeCameraModifier(8f, "ChampionsBlade", 0.7f));
            Projectile.netUpdate = true;
        }

        private float SwordLength => MathHelper.Lerp(100, 180, (float)hitStacks / ChampionsBlade.MaxStacks);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                Projectile.Center + Projectile.rotation.ToRotationVector2() * SwordLength
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSwingEmpowered();
            DrawSwing();

            var rotation = Projectile.rotation + (Player.direction == 1 ? MathHelper.PiOver4 : MathHelper.Pi * 0.75f);
            var origin = new Vector2(Player.direction == 1 ? 10 : 67, 67);

            itemTexture ??= ModContent.Request<Texture2D>("Macrocosm/Content/Items/Weapons/Melee/ChampionsBlade", AssetRequestMode.ImmediateLoad);

            float fullBladeOpacity = (float)hitStacks / ChampionsBlade.MaxStacks;

            Main.EntitySpriteDraw(
                TextureAssets.Projectile[Type].Value,
                Projectile.Center + Projectile.rotation.ToRotationVector2() * 16f - Main.screenPosition,
                null,
                Utility.Colorize(new Color(30, 255, 105, 127), lightColor) * fullBladeOpacity,
                rotation,
                origin,
                Projectile.scale,
                Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0
            );

            Main.EntitySpriteDraw(
                itemTexture.Value,
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

        private void DrawSwing()
        {
            float stacksMultiplier = ((float)hitStacks / ChampionsBlade.MaxStacks);

            Vector2 position = Projectile.Center - Main.screenPosition;
            Texture2D swing = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "Swing").Value;
            Vector2 swingOrigin = swing.Frame(1, 4).Size() / 2f;
            float scale = Projectile.scale * 1.4f;
            SpriteEffects spriteEffects = ((!(SwingDirection >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None); // Flip the sprite based on the direction it is facing.
            float progress = Timer / (MaxTime * 4);
            float lerpTime = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);
            float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

            Color color = new Color(30, 255, 105);
            Color backDarkColor = (color * 0.4f).WithOpacity(progressScale);
            Color middleMediumColor = color;
            Color frontLightColor = (color * 1.4f).WithAlpha(255);

            float rotation = Projectile.rotation - ((MathHelper.PiOver4 - MathHelper.Pi / 16) * SwingDirection);

            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 0), backDarkColor * lerpTime, rotation + SwingDirection * MathHelper.PiOver4 * -1f * (1f - progress), swingOrigin, scale, spriteEffects, 0f);
            // Very faint part affected by the light color
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 0), middleMediumColor * 0.15f, rotation + SwingDirection * 0.01f, swingOrigin, scale, spriteEffects, 0f);
            // Middle part
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 0), middleMediumColor * lerpTime * 0.3f, (float)rotation, swingOrigin, scale, spriteEffects, 0f);
            // Front part
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 0), frontLightColor * lerpTime * 0.5f, (float)rotation, swingOrigin, scale * 0.975f, spriteEffects, 0f);
            // Thin top line (final frame)
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 3), Color.White * 0.6f * lerpTime * (1f - stacksMultiplier), rotation + SwingDirection * 0.01f, swingOrigin, scale, spriteEffects, 0f);
            // Thin middle line (final frame)
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 3), Color.White * 0.5f * lerpTime * (1f - stacksMultiplier), rotation + SwingDirection * -0.05f, swingOrigin, scale * 0.8f, spriteEffects, 0f);
            // Thin bottom line (final frame)
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 3), Color.White * 0.4f * lerpTime, rotation + SwingDirection * -0.1f, swingOrigin, scale * 0.6f, spriteEffects, 0f);

            // This draws some sparkles around the circumference of the swing.
            for (float i = 0f; i < 8f; i += 1f)
            {
                float edgeRotation = rotation + SwingDirection * i * (MathHelper.Pi * -2f) * 0.025f + Utils.Remap(progress, 0f, 1f, 0f, MathHelper.PiOver4) * SwingDirection;
                Vector2 drawPos = position + edgeRotation.ToRotationVector2() * (swing.Width * 0.5f - 6f) * scale;
                Utility.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, new Color(255, 255, 255, 0) * lerpTime * (i / 9f), middleMediumColor * (1f - stacksMultiplier), progress, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(progress, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
            }

            // This draws a large star sparkle at the front of the projectile.
            Vector2 drawPos2 = position + (rotation + Utils.Remap(progress, 0f, 1f, 0f, MathHelper.PiOver4) * SwingDirection).ToRotationVector2() * (swing.Width * 0.5f - 4f) * scale;
            Utility.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos2, new Color(255, 255, 255, 0) * lerpTime * 0.5f, middleMediumColor * (1f - stacksMultiplier), progress, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(progress, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale);
        }

        private void DrawSwingEmpowered()
        {
            float stacksMultiplier = ((float)hitStacks / ChampionsBlade.MaxStacks);

            Vector2 position = Projectile.Center - Main.screenPosition;
            Texture2D swing = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "Swing").Value;
            Vector2 swingOrigin = swing.Frame(1, 4).Size() / 2f;
            float scale = Projectile.scale + (Projectile.scale * stacksMultiplier * 0.8f);
            SpriteEffects spriteEffects = ((!(SwingDirection >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None); // Flip the sprite based on the direction it is facing.
            float progress = Timer / (MaxTime * 4);
            float lerpTime = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f) * stacksMultiplier;
            float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);
            float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

            Color color = new Color(30, 255, 105);
            Color backDarkColor = (color * 0.4f).WithOpacity(progressScale);
            Color middleMediumColor = color;
            Color frontLightColor = (color * 1.4f).WithAlpha(127);

            float rotation = Projectile.rotation - ((MathHelper.PiOver4 - MathHelper.Pi / 8) * SwingDirection);

            // Back part
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 0), backDarkColor * lightingColor * lerpTime, rotation + SwingDirection * MathHelper.PiOver4 * -1f * (1f - progress), swingOrigin, scale, spriteEffects, 0f);
            // Very faint part affected by the light color
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 0), middleMediumColor * 0.15f, rotation + SwingDirection * 0.01f, swingOrigin, scale, spriteEffects, 0f);
            // Middle part
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 0), middleMediumColor * lightingColor * lerpTime * 0.3f, rotation, swingOrigin, scale, spriteEffects, 0f);
            // Front part
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 0), frontLightColor * lightingColor * lerpTime * 0.5f, rotation, swingOrigin, scale * 0.975f, spriteEffects, 0f);
            // Thin top line (final frame)
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 3), Color.White * 0.6f * lerpTime, rotation + SwingDirection * 0.01f, swingOrigin, scale, spriteEffects, 0f);
            // Thin middle line (final frame)
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 3), Color.White * 0.5f * lerpTime, rotation + SwingDirection * -0.05f, swingOrigin, scale * 0.8f, spriteEffects, 0f);
            // Thin bottom line (final frame)
            Main.EntitySpriteDraw(swing, position, swing.Frame(1, 4, frameY: 3), Color.White * 0.4f * lerpTime, rotation + SwingDirection * -0.1f, swingOrigin, scale * 0.6f, spriteEffects, 0f);

            // This draws some sparkles around the circumference of the swing.
            for (float i = 0f; i < 8f; i += 1f)
            {
                float edgeRotation = rotation + SwingDirection * i * (MathHelper.Pi * -2f) * 0.025f + Utils.Remap(progress, 0f, 1f, 0f, MathHelper.PiOver4) * SwingDirection;
                Vector2 drawPos = position + edgeRotation.ToRotationVector2() * ((float)swing.Width * 0.5f - 6f) * scale;
                Utility.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, new Color(255, 255, 255, 0) * lerpTime * (i / 9f) * stacksMultiplier, middleMediumColor * stacksMultiplier, progress, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(progress, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
            }

            // This draws a large star sparkle at the front of the projectile.
            Vector2 drawPos2 = position + (rotation + Utils.Remap(progress, 0f, 1f, 0f, MathHelper.PiOver4) * SwingDirection).ToRotationVector2() * ((float)swing.Width * 0.5f - 4f) * scale;
            Utility.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos2, new Color(255, 255, 255, 0) * lerpTime * 0.5f, middleMediumColor * stacksMultiplier, progress, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(progress, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale);
        }
    }
}
