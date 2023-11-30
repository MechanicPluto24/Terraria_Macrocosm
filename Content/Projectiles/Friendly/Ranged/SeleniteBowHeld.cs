using Iced.Intel;
using Macrocosm.Common.Bases;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class SeleniteBowHeld : ChargedHeldProjectile
    {
        public override string Texture => "Macrocosm/Content/Items/Weapons/Ranged/SeleniteBow";

        public ref float MinCharge => ref Projectile.ai[0];
        public ref float AI_Timer => ref Projectile.ai[1];
        public ref float AI_Charge => ref Projectile.ai[2];

        public override float CircularHoldoutOffset => 8f;

        protected override bool StillInUse => base.StillInUse || Main.mouseRight;

        public override void SetProjectileStaticDefaults()
        {
        }

        public override void SetProjectileDefaults()
        {

        }

        public override void ProjectileAI()
        {
            if(OwnerPlayer.whoAmI == Main.myPlayer)
            {
                int damage = OwnerPlayer.GetWeaponDamage(OwnerPlayer.inventory[OwnerPlayer.selectedItem]);
                float knockback = OwnerPlayer.inventory[OwnerPlayer.selectedItem].knockBack;
                float speed;
                int usedAmmoItemId;

                Item usedItem = Main.mouseItem.type == ItemID.None ? OwnerPlayer.inventory[OwnerPlayer.selectedItem] : Main.mouseItem;
                if (usedItem.type != ModContent.ItemType<SeleniteBow>())
                    Projectile.Kill();

                if (Main.mouseRight)
                {
                    AI_Charge++;

                    if (AI_Charge == MinCharge)
                        SoundEngine.PlaySound(SoundID.Item29 with { Pitch = -0.5f});

                    if(AI_Charge > MinCharge)
                    {
                        Projectile.Center += Main.rand.NextVector2Circular(0.35f, 0.35f);
                    }
                }            
                else
                {
                    if (AI_Charge >= MinCharge)
                    {
                        if(OwnerPlayer.PickAmmo(usedItem, out _, out speed, out damage, out knockback, out usedAmmoItemId))
                        {
                            Projectile.NewProjectile(new EntitySource_ItemUse_WithAmmo(OwnerPlayer, usedItem, usedAmmoItemId), Projectile.Center, Vector2.Normalize(Projectile.velocity) * speed * 0.466f, ModContent.ProjectileType<SeleniteBeam>(), damage, knockback, Projectile.owner, default, Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI));
                            AI_Charge = 0;
                        }
                    } 
                    else if (AI_Timer % usedItem.useTime == 0)
                    {
                        if (OwnerPlayer.PickAmmo(usedItem, out int projToShoot, out speed, out damage, out knockback, out usedAmmoItemId))
                        {
                            SoundEngine.PlaySound(SoundID.Item5, Projectile.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Normalize(Projectile.velocity) * speed, projToShoot, damage, knockback, Projectile.owner, default, Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI));
                            AI_Timer = 0;
                        }
                        else
                        {
                            Projectile.Kill();
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

        SpriteBatchState state;
        public override void PostDraw(Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;

            if(AI_Charge > 0)
            {
                state.SaveState(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(BlendState.AlphaBlend, state);

                float rotation = Projectile.rotation + Projectile.localAI[0];
                float progress = MathHelper.Clamp(AI_Charge / MinCharge, 0f, 1f);
                float scale = 0.5f * Projectile.scale * progress;
                byte alpha = (byte)(255 - MathHelper.Clamp(64 + Projectile.localAI[1], 0, 255));
                Vector2 offset = default;

                if (AI_Charge < MinCharge)
                {
                    scale += 0.3f * Utility.QuadraticEaseOut(progress);
                    rotation += 0.5f * Utility.CubicEaseInOut(progress);
                    Projectile.localAI[1] += 1f;
                }

                if (AI_Charge >= MinCharge)
                {
                    scale += 0.3f;
                    rotation += 0.5f;
                    offset = Main.rand.NextVector2Circular(1, 1);
                    Projectile.localAI[0] += 0.001f;
                    Projectile.localAI[1] += 3f;
                }

                Vector2 rotPoint = Utility.RotatingPoint(Projectile.Center, new Vector2(23, 0), Projectile.rotation) + offset;
                spriteBatch.DrawStar(rotPoint - Main.screenPosition, 2, new Color(131, 168, 171, alpha), scale, rotation);

                spriteBatch.End();
                spriteBatch.Begin(BlendState.AlphaBlend, state);
            }
        }
    }
}
