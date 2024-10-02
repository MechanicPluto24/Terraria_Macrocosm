using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
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
        public ref float Timer => ref Projectile.localAI[0];
        public ref float SwingDirection => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];
        public ref float Alt => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            // The width and height don't really matter here because we have custom collision.
            Projectile.width = 142;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true; // A line of sight check so the projectile can't deal damage through tiles.
            Projectile.ownerHitCheckDistance = 300f; // The maximum range that the projectile can hit a target. 300 pixels is 18.75 tiles.
            Projectile.usesOwnerMeleeHitCD = true; // This will make the projectile apply the standard number of immunity frames as normal melee attacks.
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.aiStyle = -1;
       
        }

        int RuneTimer;
        private bool shot;
        public Vector2 aim;
        float altSwingOpacity=0f;
        int shootTimer=0;
        public override void AI()
        {
            // Current time that the projectile has been alive.
            Player player = Main.player[Projectile.owner];
            float progress = Timer / MaxTime; // The current time over the max time.
            float velocityRotation = Projectile.velocity.ToRotation();
            if (Alt == 0f)
            {
            float adjustedRotation = (!(SwingDirection >= 0f)) ? ((MathHelper.Pi+(MathHelper.PiOver4/2)) * SwingDirection * progress + velocityRotation + SwingDirection *(MathHelper.Pi+(MathHelper.PiOver4/2)) + player.fullRotation)+MathHelper.PiOver4:((MathHelper.Pi+(MathHelper.PiOver4/2)) * SwingDirection * progress + velocityRotation + SwingDirection *(MathHelper.Pi+(MathHelper.PiOver4/2)) + player.fullRotation)-MathHelper.PiOver4;

                Projectile.rotation = adjustedRotation; // Set the rotation to our to the new rotation we calculated.

                    aim = (Main.MouseWorld.X<Main.player[Main.myPlayer].Center.X ? new Vector2(-1f,0f): new Vector2(1f,0f))+((Main.MouseWorld - Main.player[Main.myPlayer].Center).SafeNormalize(default)*1f);
                if (!shot && progress > 0.1f && Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, aim, ModContent.ProjectileType<LuminiteWave>(), (int)(Projectile.damage / 2), 1f, Main.myPlayer, 20f);

                    for(int i=0; i<3; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, aim.RotatedByRandom(MathHelper.PiOver4)*Main.rand.NextFloat(2f,5f), ModContent.ProjectileType<LuminiteRune>(), (int)(Projectile.damage / 4), 1f, Main.myPlayer, 1f);

                    } 
                    shot = true;
                }

                Timer++;
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter + (new Vector2(Projectile.width / 2, 0).RotatedBy(adjustedRotation))) - Projectile.velocity;

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - ((MathHelper.Pi / 4) + MathHelper.Pi / 6));

                if (Timer >= MaxTime)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                
                float adjustedRotation = (!(SwingDirection >= 0f)) ? -((MathHelper.Pi+MathHelper.PiOver4) * SwingDirection * progress + -velocityRotation + SwingDirection *(MathHelper.Pi+MathHelper.PiOver4) + player.fullRotation)+MathHelper.Pi:-((MathHelper.Pi+MathHelper.PiOver4) * SwingDirection * progress + -velocityRotation + SwingDirection *(MathHelper.Pi+MathHelper.PiOver4) + player.fullRotation)-MathHelper.Pi;

                Projectile.rotation = adjustedRotation; // Set the rotation to our to the new rotation we calculated.

                    aim = (Main.MouseWorld.X<Main.player[Main.myPlayer].Center.X ? new Vector2(-1f,0f): new Vector2(1f,0f))+((Main.MouseWorld - Main.player[Main.myPlayer].Center).SafeNormalize(default)*1f);
                if (!shot && progress > 0.1f && Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, aim, ModContent.ProjectileType<LuminiteWave>(), (int)(Projectile.damage / 2), 1f, Main.myPlayer, 20f);

                    for(int i=0; i<3; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, aim.RotatedByRandom(MathHelper.PiOver4)*Main.rand.NextFloat(2f,5f), ModContent.ProjectileType<LuminiteRune>(), (int)(Projectile.damage / 4), 1f, Main.myPlayer, 1f);

                    } 
                    shot = true;
                }

                Timer++;
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter + (new Vector2(Projectile.width / 2, 0).RotatedBy(adjustedRotation))) - Projectile.velocity;

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - ((MathHelper.Pi / 4) + MathHelper.Pi / 6));

                if (Timer >= MaxTime)
                {
                    Projectile.Kill();
                }
        }
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {

            /*
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);
            Player player = Main.player[Projectile.owner];
            Vector2 position = player.Center - Main.screenPosition;
            Texture2D texture1 = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Twirl1").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Twirl2").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "SwingEdgeAlt").Value;
            Vector2 origin = texture1.Size() / 2f;
            float scale = Projectile.scale * 1.3f;
            SpriteEffects spriteEffects = Projectile.rotation < 0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally; // Flip the sprite based on the direction it is facing.
            float progress = MathHelper.Clamp(Timer / MaxTime, 0f, 1f);
            float lerpTime = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);
            float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);

            Color Colour2 = new Color(213, 155, 148, 0);
            Color Colour1 = new Color(94, 229, 163, 0);
            Color frontLightColor = new Color(150, 240, 255) * 1.3f;

            Color whiteTimesLerpTime = Color.White * lerpTime * 0.5f;
            whiteTimesLerpTime.A = (byte)(whiteTimesLerpTime.A * (1f - lightingColor));
            Color faintLightingColor = whiteTimesLerpTime * lightingColor * 0.5f;
            faintLightingColor.G = (byte)(faintLightingColor.G * lightingColor);
            faintLightingColor.B = (byte)(faintLightingColor.R * (0.25f + lightingColor * 0.75f));

            if (Alt == 0f)
            {
                Main.EntitySpriteDraw(texture1, position, null, Colour1 * lerpTime * 0.2f, Projectile.rotation + MathHelper.PiOver2 + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 0.6f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture2, position, null, Colour2 * lerpTime * 0.3f, Projectile.rotation + MathHelper.PiOver2 + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 0.5f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture1, position, null, Colour1 * lerpTime * 0.1f, Projectile.rotation + MathHelper.PiOver2 + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 1.1f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture2, position, null, Colour2 * lerpTime * 0.3f, Projectile.rotation + MathHelper.PiOver2 + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 1f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture1, position, null, Colour1 * lerpTime * 0.5f, Projectile.rotation + MathHelper.PiOver2 + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 0.9f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture2, position, null, Colour2 * lerpTime * 0.8f, Projectile.rotation + MathHelper.PiOver2 + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 0.8f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture3, position, null, Colour1 * lerpTime * 1f, Projectile.rotation + MathHelper.PiOver2 + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 0.8f, spriteEffects, 0f);

            }
            else
            {
                float offset = Projectile.rotation < 0f ? (MathHelper.Pi / 2) + (MathHelper.Pi / 4) : MathHelper.Pi / 4;

                Main.EntitySpriteDraw(texture1, position, null, Colour1 * altSwingOpacity * 0.2f, Projectile.rotation + offset + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 0.6f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture2, position, null, Colour2 * altSwingOpacity * 0.3f, Projectile.rotation + offset + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 0.5f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture1, position, null, Colour1 * altSwingOpacity * 0.1f, Projectile.rotation + offset + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 1.1f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture2, position, null, Colour2 * altSwingOpacity * 0.3f, Projectile.rotation + offset + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 1f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture1, position, null, Colour1 * altSwingOpacity * 0.5f, Projectile.rotation + offset + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 0.9f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture2, position, null, Colour2 * altSwingOpacity * 0.8f, Projectile.rotation + offset + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 0.8f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture3, position, null, Colour1 * altSwingOpacity * 1f, Projectile.rotation + offset + Projectile.ai[0] * 1f * (1f - progress), origin, scale * 0.8f, spriteEffects, 0f);

            }


            // This draws some sparkles around the circumference of the swing.

            // Uncomment this line for a visual representation of the projectile's size.
            // Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, position, sourceRectangle, Color.Orange * 0.75f, 0f, origin, scale, spriteEffects);

            return true;
            */
            Player player = Main.player[Projectile.owner];
            Vector2 position = player.Center - Main.screenPosition;
            Texture2D texture = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "Swing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 origin = texture.Frame(1, 4).Size() / 2f;
            float scale = Projectile.scale * 2f;
            SpriteEffects spriteEffects = ((!(SwingDirection >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None); // Flip the sprite based on the direction it is facing.
            float progress = Timer / MaxTime;
            float lerpTime = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);
            float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);
            float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);
            float Rotation=(!(SwingDirection >= 0f)) ? Projectile.rotation+(MathHelper.PiOver4):Projectile.rotation-(MathHelper.PiOver4);
            Color color = new Color(94, 229, 163);
            Color backDarkColor = (color * 0.4f).WithOpacity(progressScale);
            Color middleMediumColor = color;
            Color frontLightColor = (color * 1.4f).WithAlpha(255); ;

            Color whiteTimesLerpTime = Color.White * lerpTime * 0.5f;
            whiteTimesLerpTime.A = (byte)(whiteTimesLerpTime.A * (1f - lightingColor));
            Color faintLightingColor = whiteTimesLerpTime * lightingColor * 0.5f;
            faintLightingColor.G = (byte)(faintLightingColor.G * lightingColor);
            faintLightingColor.B = (byte)(faintLightingColor.R * (0.25f + lightingColor * 0.75f));

            // Back part
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), backDarkColor * lightingColor * lerpTime, Rotation+ SwingDirection * MathHelper.PiOver4 * -1f * (1f - progress), origin, scale, spriteEffects, 0f);
            // Very faint part affected by the light color
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), faintLightingColor * 0.15f, Rotation + SwingDirection * 0.01f, origin, scale, spriteEffects, 0f);
            // Middle part
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), new Color(213,155,148) * lightingColor * lerpTime * 0.8f, Rotation, origin, scale, spriteEffects, 0f);
            // Front part
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), frontLightColor * lightingColor * lerpTime * 0.5f, Rotation, origin, scale * 0.975f, spriteEffects, 0f);
            // Thin top line (final frame)
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), Color.White * 0.6f * lerpTime, Rotation+ SwingDirection * 0.01f, origin, scale, spriteEffects, 0f);
            // Thin middle line (final frame)
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), Color.White * 0.5f * lerpTime, Rotation + SwingDirection * -0.05f, origin, scale * 0.8f, spriteEffects, 0f);
            // Thin bottom line (final frame)
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), Color.White * 0.4f * lerpTime, Rotation + SwingDirection * -0.1f, origin, scale * 0.6f, spriteEffects, 0f);

            // This draws some sparkles around the circumference of the swing.
            for (float i = 0f; i < 8f; i += 1f)
            {
                float edgeRotation = Rotation + SwingDirection * i * (MathHelper.Pi * -2f) * 0.025f + Utils.Remap(progress, 0f, 1f, 0f, MathHelper.PiOver4) * SwingDirection;
                Vector2 drawPos = position + edgeRotation.ToRotationVector2() * ((float)texture.Width * 0.5f - 6f) * scale;
                Utility.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, new Color(255, 255, 255, 0) * lerpTime * (i / 9f), middleMediumColor, progress, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(progress, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
            }

            // This draws a large star sparkle at the front of the projectile.
            Vector2 drawPos2 = position + (Rotation+ Utils.Remap(progress, 0f, 1f, 0f, MathHelper.PiOver4) * SwingDirection).ToRotationVector2() * ((float)texture.Width * 0.5f - 4f) * scale;
            Utility.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos2, new Color(255, 255, 255, 0) * lerpTime * 0.5f, middleMediumColor, progress, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(progress, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale);
            return true;
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
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), ProjSpawnPosition, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10f, 15f)*(float)((damage/Projectile.damage)/1.5), ModContent.ProjectileType<LunarBlood>(), (int)(damage * 0.5f), 0, Projectile.owner, 1f);
                }
            }
            else
            {
                for (int i = 1; i < 9; i++)
                {
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), ProjSpawnPosition, -Vector2.UnitY.RotatedBy(((MathHelper.PiOver4 / 2) * (i - 1)) - MathHelper.PiOver2 + (MathHelper.PiOver4 / 4)) * 15f*(float)((damage/Projectile.damage)/1.5), ModContent.ProjectileType<LunarBlood>(), (int)(damage * 2f), 0, Projectile.owner, (float)(i + 1));
                }

            }
        }
    }
}