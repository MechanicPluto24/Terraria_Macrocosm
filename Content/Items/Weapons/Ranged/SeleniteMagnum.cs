using Macrocosm.Common.Bases;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class SeleniteMagnum : GunHeldProjectileItem
    {
        public override GunHeldProjectileData GunHeldProjectileData => new()
        {
            GunBarrelPosition = new(18, 7),
            CenterYOffset = 4,
            MuzzleOffset = 20,
            Recoil = (7, 0.4f),
            UseBackArm = false,
            RecoilStartFrame = 5
        };

        public override void SetDefaultsHeldProjectile()
        {
            Item.DefaultToRangedWeapon(10, AmmoID.Bullet, 20, 20, true);
            Item.damage = 150;

            Item.width = 34;
            Item.height = 20;

            Item.knockBack = 4f;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.UseSound = SoundID.Item38;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItemHeldProjectile(Player player)
        {
            if (player.AltFunction())
            {
                Item.useTime = 6;
                Item.useAnimation = 36;
                Item.reuseDelay = 30;
            }
            else
            {
                Item.useTime = 20;
                Item.useAnimation = 20;
                Item.reuseDelay = 0;
            }

            return true;
        }


        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.AltFunction())
            {
                float radians = MathHelper.ToRadians(5);
                velocity = velocity.RotatedBy(Main.rand.NextFloat(-radians, radians));
            }

            position += Vector2.Normalize(velocity) * 30;
            Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(4, 0);

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<SeleniteBar>(), 12);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
