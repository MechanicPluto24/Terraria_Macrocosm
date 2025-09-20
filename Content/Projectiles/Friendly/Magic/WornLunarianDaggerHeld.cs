using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic;

public class WornLunarianDaggerHeld : ChargedHeldProjectile
{
    private static Asset<Texture2D> glow;

    public float MinCharge => MaxCharge * 0.2f;
    public ref float MaxCharge => ref Projectile.ai[0];
    public ref float AI_Timer => ref Projectile.ai[1];
    public ref float AI_Charge => ref Projectile.ai[2];

    private bool altAttackActive = false;
    private int altAttackCount = 0;
    private int usePeriod = 0;

    public override float CircularHoldoutOffset => 8f;

    protected override bool StillInUse => base.StillInUse || Main.mouseRight || itemUseTimer > 0 || altAttackActive;

    public override bool ShouldUpdateAimRotation => true;


    public override void SetProjectileStaticDefaults()
    {
    }

    public override void SetProjectileDefaults()
    {
        //Projectile.hide = true;
    }

    public override void ProjectileAI()
    {
        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi / 2 + MathHelper.Pi / 16);

        if (Player.whoAmI == Main.myPlayer)
        {
            Item currentItem = Player.CurrentItem();
            usePeriod = currentItem.useTime;

            int damage = Player.GetWeaponDamage(currentItem);
            float knockback = currentItem.knockBack;

            if (Main.mouseRight && !altAttackActive)
            {
                AI_Charge += 1f * Player.GetAttackSpeed(DamageClass.Magic);

                if ((int)AI_Charge == (int)MaxCharge)
                {
                    SoundEngine.PlaySound(SoundID.Item29 with { Pitch = 0.2f, Volume = 0.35f }, Projectile.position);
                }

                if (AI_Charge > MinCharge && AI_Charge < MaxCharge && AI_Charge % 5 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item29 with
                    {
                        Pitch = 0.2f + 0.5f * (AI_Charge / MaxCharge),
                        Volume = 0.15f * (AI_Charge / MaxCharge),
                        SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
                    }, Projectile.position);
                }
            }
            else if (!altAttackActive)
            {
                if (AI_Charge > MinCharge)
                {
                    float charge = MathHelper.Clamp(AI_Charge / MaxCharge, 0f, 1f);
                    int attackCount = 3 + (int)(7 * charge);

                    if (Player.CheckMana(currentItem.mana * (int)(attackCount * 0.2f), true))
                    {
                        altAttackCount = attackCount;
                        altAttackActive = true;

                        AI_Timer = 0;
                        AI_Charge = 0;
                    }
                }
                else if (AI_Timer % usePeriod == 0)
                {
                    if (Player.CheckMana(currentItem.mana, true))
                    {
                        Vector2 position = Projectile.Center + new Vector2(0, -12 * Projectile.direction).RotatedBy(Projectile.rotation);
                        Vector2 velocity = Vector2.Normalize(Projectile.velocity).RotatedByRandom(MathHelper.ToRadians(15)) * currentItem.shootSpeed * 0.666f;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<LuminiteBolt>(), damage, knockback, Projectile.owner, 1f);

                        AI_Timer = 0;
                        SoundEngine.PlaySound(SoundID.Item43 with { Volume = 0.4f, Pitch = 0.2f }, Projectile.position);
                    }
                    else if (itemUseTimer <= 0)
                    {
                        Projectile.Kill();
                    }
                }
            }
            else
            {
                int altAttackRate = 5;

                if (AI_Timer >= altAttackCount * altAttackRate)
                {
                    altAttackActive = false;
                    altAttackCount = 0;
                }
                else if (AI_Timer % altAttackRate == 0)
                {
                    Vector2 position = Projectile.Center + new Vector2(0, -12 * Projectile.direction).RotatedBy(Projectile.rotation);
                    Vector2 velocity = Vector2.Normalize(Projectile.velocity).RotatedByRandom(MathHelper.ToRadians(15)) * currentItem.shootSpeed * 0.666f;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<LuminiteBolt>(), damage, knockback, Projectile.owner, 1f);
                    SoundEngine.PlaySound(SoundID.Item43 with { Volume = 0.4f, Pitch = 0.25f }, Projectile.position);
                }
            }

            AI_Timer++;
        }
    }


    public override bool PreDraw(ref Color lightColor)
    {
        var spriteBatch = Main.spriteBatch;
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        float rotation = Projectile.rotation;
        Vector2 offset = Projectile.direction > 0 ? new Vector2(-8, 6) : new Vector2(-6, -2);
        Vector2 position = Utility.RotatingPoint(Projectile.Center, offset, rotation);
        spriteBatch.Draw(texture, position - Main.screenPosition, null, lightColor, rotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
        return false;
    }

    private ref float EffectTimer => ref Projectile.localAI[0];
    private ref float Opacity => ref Projectile.localAI[1];

    public override void PostDraw(Color lightColor)
    {
        var spriteBatch = Main.spriteBatch;
        glow ??= ModContent.Request<Texture2D>(Texture + "_Glow");
        float rotation = Projectile.rotation;
        Vector2 offset = Projectile.direction > 0 ? new Vector2(-8, 6) : new Vector2(-6, -2);
        Vector2 position = Utility.RotatingPoint(Projectile.Center, offset, rotation);

        byte alpha = (byte)(altAttackActive ? 0 : (int)(255f * ((AI_Timer % usePeriod) / (float)usePeriod)));
        spriteBatch.Draw(glow.Value, position - Main.screenPosition, null, Color.White.WithAlpha(alpha), rotation, glow.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);

        if (AI_Charge > 0 || altAttackActive)
        {
            float progress = MathHelper.Clamp(AI_Charge / MaxCharge, 0f, 1f);
            if (altAttackActive)
                progress = (1f - AI_Timer / 50f) * (altAttackCount / 10f);

            float glowRotation = Projectile.rotation + EffectTimer;
            float scale = 0.5f * Projectile.scale * progress;
            byte glowAlpha = (byte)(255 - MathHelper.Clamp(64 + Opacity, 0, 255));
            Vector2 glowOffset = default;

            if (AI_Charge < MaxCharge)
            {
                scale += 0.5f * Utility.QuadraticEaseOut(progress);
                glowRotation += 0.5f * Utility.CubicEaseInOut(progress);
                Opacity += 1f;
                glowOffset = Main.rand.NextVector2Circular(1, 1) * progress;
            }

            if (AI_Charge >= MaxCharge)
            {
                scale += 0.5f;
                glowRotation += 0.5f;
                glowOffset = Main.rand.NextVector2Circular(1, 1);
                EffectTimer += 0.001f;
                Opacity += 3f;
            }

            Vector2 rotPoint = Utility.RotatingPoint(Projectile.Center, new Vector2(8, -8 * Projectile.direction), Projectile.rotation) + glowOffset;
            Utility.DrawStar(rotPoint - Main.screenPosition, 2, new Color(44, 209, 147, glowAlpha), new Vector2(0.5f, 1.5f) * scale, glowRotation);

        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        //overPlayers.Add(Projectile.whoAmI);
    }
}
