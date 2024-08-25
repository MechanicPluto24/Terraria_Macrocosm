using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    // Scrapped but alt attack can be reused for a magic weapon
    public class SeleniteBowHeld : ChargedHeldProjectile
    {
        public override string Texture => "Macrocosm/Content/Items/Weapons/Ranged/SeleniteBow";

        public float MinCharge => MaxCharge * 0.2f;
        public ref float MaxCharge => ref Projectile.ai[0];
        public ref float AI_Timer => ref Projectile.ai[1];
        public ref float AI_Charge => ref Projectile.ai[2];

        private bool altAttackActive = false;
        private int altAttackCount = 0;

        public override float CircularHoldoutOffset => 8f;

        protected override bool StillInUse => base.StillInUse || Main.mouseRight || itemUseTime > 0 || altAttackActive;

        public override bool ShouldUpdateAimRotation => true;


        public override void SetProjectileStaticDefaults()
        {
        }

        public override void SetProjectileDefaults()
        {

        }

        public override void ProjectileAI()
        {
            if (Player.whoAmI == Main.myPlayer)
            {
                Item currentItem = Player.CurrentItem();

                int damage = Player.GetWeaponDamage(currentItem);
                float knockback = currentItem.knockBack;
                float speed;
                int usedAmmoItemId;

                if (Main.mouseRight && !altAttackActive)
                {
                    AI_Charge += 1f * Player.GetAttackSpeed(DamageClass.Ranged);

                    if (AI_Charge == MaxCharge)
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
                        if (Player.PickAmmo(currentItem, out _, out _, out _, out _, out _))
                        {
                            float charge = MathHelper.Clamp(AI_Charge / MaxCharge, 0f, 1f);

                            altAttackCount = 3 + (int)(7 * charge);
                            altAttackActive = true;

                            AI_Timer = 0;
                            AI_Charge = 0;
                        }
                    }
                    else if (AI_Timer % currentItem.useTime == 0)
                    {
                        if (Player.PickAmmo(currentItem, out int projToShoot, out speed, out damage, out knockback, out usedAmmoItemId))
                        {
                            // Shoot 2 parallel arrows
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -7).RotatedBy(Projectile.velocity.ToRotation()), Vector2.Normalize(Projectile.velocity) * speed, projToShoot, damage, knockback, Projectile.owner);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, +7).RotatedBy(Projectile.velocity.ToRotation()), Vector2.Normalize(Projectile.velocity) * speed, projToShoot, damage, knockback, Projectile.owner);

                            AI_Timer = 0;
                            SoundEngine.PlaySound(SoundID.Item5, Projectile.position);
                        }
                        else if (itemUseTime <= 0)
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
                        if (Player.PickAmmo(currentItem, out _, out speed, out damage, out knockback, out _))
                        {
                            Vector2 position = Projectile.Center - new Vector2(20, 0).RotatedBy(Projectile.rotation) + Main.rand.NextVector2Circular(32, 32);
                            Vector2 velocity = Vector2.Normalize(Projectile.velocity).RotatedByRandom(MathHelper.ToRadians(15)) * speed * 0.666f;
                            damage = (int)(damage * 0.5f);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<SeleniteBolt>(), damage, knockback, Projectile.owner, 1f);
                        }
                    }
                }

                AI_Timer++;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 rotPoint = Utility.RotatingPoint(Projectile.Center, new Vector2(10, 0), Projectile.rotation);
            spriteBatch.Draw(texture, rotPoint - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            return false;
        }

        private ref float EffectTimer => ref Projectile.localAI[0];
        private ref float Opacity => ref Projectile.localAI[1];

        SpriteBatchState state;
        public override void PostDraw(Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;

            if (AI_Charge > 0 || altAttackActive)
            {
                state.SaveState(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(BlendState.AlphaBlend, state);

                float progress = MathHelper.Clamp(AI_Charge / MaxCharge, 0f, 1f);
                if (altAttackActive)
                    progress = (1f - (AI_Timer / 50f)) * (altAttackCount / 10f);

                float rotation = Projectile.rotation + EffectTimer;
                float scale = 0.5f * Projectile.scale * progress;
                byte alpha = (byte)(255 - MathHelper.Clamp(64 + Opacity, 0, 255));
                Vector2 offset = default;

                if (AI_Charge < MaxCharge)
                {
                    scale += 0.5f * Utility.QuadraticEaseOut(progress);
                    rotation += 0.5f * Utility.CubicEaseInOut(progress);
                    Opacity += 1f;
                    offset = Main.rand.NextVector2Circular(1, 1) * progress;
                }

                if (AI_Charge >= MaxCharge)
                {
                    scale += 0.5f;
                    rotation += 0.5f;
                    offset = Main.rand.NextVector2Circular(1, 1);
                    EffectTimer += 0.001f;
                    Opacity += 3f;
                }

                Vector2 rotPoint = Utility.RotatingPoint(Projectile.Center, new Vector2(20, 0), Projectile.rotation) + offset;
                spriteBatch.DrawStar(rotPoint - Main.screenPosition, 2, new Color(131, 168, 171, alpha), new Vector2(0.5f, 1.5f) * scale, rotation);

                spriteBatch.End();
                spriteBatch.Begin(BlendState.AlphaBlend, state);
            }
        }
    }
}
