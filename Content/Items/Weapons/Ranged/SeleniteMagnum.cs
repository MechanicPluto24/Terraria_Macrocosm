using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
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
            Recoil = (5, 0.25f),
            UseBackArm = false,
            RecoilStartFrame = 5,
            FollowsAimPosition = false
        };

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.DefaultToRangedWeapon(10, AmmoID.Bullet, 20, 20, true);
            Item.damage = 280;

            Item.width = 34;
            Item.height = 20;

            Item.knockBack = 4f;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.UseSound = SoundID.Item38;

            Item.autoReuse = true;
        }

        private int altUseCooldown;

        public override void UpdateInventory(Player player)
        {
            if (altUseCooldown > 0)
                altUseCooldown--;
        }

        public override bool AltFunctionUse(Player player) => altUseCooldown <= 0;

        public override bool CanUseItemHeldProjectile(Player player)
        {
            if (player.AltFunction())
            {
                Item.damage = 240;
                Item.useTime = 6;
                Item.useAnimation = 36;
                altUseCooldown = 65;
            }
            else
            {
                Item.damage = 280;
                Item.useTime = 20;
                Item.useAnimation = 20;
            }

            return base.CanUseItemHeldProjectile(player);
        }


        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, GunHeldProjectile heldProjectile)
        {
            if (player.AltFunction())
            {
                float halfSpreadCone = MathHelper.ToRadians(20);
                float spreadCone = Main.rand.NextFloat(-halfSpreadCone, halfSpreadCone);
                velocity = velocity.RotatedBy(spreadCone);
                heldProjectile.Projectile.rotation = velocity.ToRotation();
                SoundEngine.PlaySound(SoundID.Item38 with { PitchRange = (0f, 0.5f) }, position);
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
