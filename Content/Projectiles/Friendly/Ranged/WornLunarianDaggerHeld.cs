using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing.Imaging;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class WornLunarianDaggerHeld : ChargedHeldProjectile
    {
        public override string Texture => "Macrocosm/Content/Items/Weapons/Magic/WornLunarianDagger";

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
            //Projectile.hide = true;
        }

        public override void ProjectileAI()
        {
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi/2 + MathHelper.Pi/16);

            if (Player.whoAmI == Main.myPlayer)
            {
                Item currentItem = Player.CurrentItem();

                int damage = Player.GetWeaponDamage(currentItem);
                float knockback = currentItem.knockBack;
                float speed;

                if (Main.mouseRight && !altAttackActive)
                {
                    AI_Charge += 1f * Player.GetAttackSpeed(DamageClass.Magic);

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
                        if (Player.CheckMana(currentItem.mana, true))
                        {
                            Vector2 position = Projectile.Center + new Vector2(0, -12 * Projectile.direction).RotatedBy(Projectile.rotation);
                            Vector2 velocity = Vector2.Normalize(Projectile.velocity).RotatedByRandom(MathHelper.ToRadians(15)) * currentItem.shootSpeed * 0.666f;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<LuminiteBolt>(), damage, knockback, Projectile.owner, 1f);

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
                        if (Player.CheckMana(currentItem.mana, true))
                        {
                            Vector2 position = Projectile.Center + new Vector2(0, -12 * Projectile.direction).RotatedBy(Projectile.rotation);
                            Vector2 velocity = Vector2.Normalize(Projectile.velocity).RotatedByRandom(MathHelper.ToRadians(15)) * currentItem.shootSpeed * 0.666f;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<LuminiteBolt>(), damage, knockback, Projectile.owner, 1f);
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
            float rotation = Projectile.rotation;
            Vector2 offset = Projectile.direction > 0 ? new Vector2(-8, 6) : new Vector2(-6, -2);
            Vector2 position = Utility.RotatingPoint(Projectile.Center, offset, rotation);
            spriteBatch.Draw(texture, position - Main.screenPosition, null, lightColor, rotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
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

                Vector2 rotPoint = Utility.RotatingPoint(Projectile.Center, new Vector2(8, -8 * Projectile.direction), Projectile.rotation) + offset;
                Utility.DrawStar(rotPoint - Main.screenPosition, 2, new Color(44, 209, 147, alpha), new Vector2(0.5f, 1.5f) * scale, rotation);

                spriteBatch.End();
                spriteBatch.Begin(BlendState.AlphaBlend, state);
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            //overPlayers.Add(Projectile.whoAmI);
        }
    }
}
