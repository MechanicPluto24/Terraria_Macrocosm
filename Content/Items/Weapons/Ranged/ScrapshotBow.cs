using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class ScrapshotBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToBow(16, 18, hasAutoReuse: true);
            Item.width = 26;
            Item.height = 62;
            Item.damage = 80;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarity1>();
            Item.useAnimation = 16;
            Item.useTime = Item.useAnimation / 4;
            Item.reuseDelay = 16;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            #region Muzzle Offset
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 15f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            #endregion

            #region Shrapnel
            int currentAmmo = type;

            if (Main.rand.NextBool(6))
            {
                type = ModContent.ProjectileType<Projectiles.Friendly.Ranged.ScrapShrapnel>();

                for (int i = 0; i < Main.rand.NextFloat(3, 6); i++)
                {
                    Vector2 shrapnelVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.8f, 1.2f);
                    float velocityMultiplier = Main.rand.NextFloat(0.3f, 0.5f);

                    Projectile.NewProjectile(Item.GetSource_FromThis(), position, shrapnelVelocity * velocityMultiplier, type, damage, knockback, player.whoAmI);
                }

                // SFX
                SoundEngine.PlaySound(SoundID.NPCHit4 with
                {
                    Volume = 0.5f,
                    Pitch = -0.5f,
                    PitchVariance = 0.5f
                });
            }
            else
            {
                type = currentAmmo;
            }
            #endregion
        }
    }
}
