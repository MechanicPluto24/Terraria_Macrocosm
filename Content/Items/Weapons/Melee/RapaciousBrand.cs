using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class RapaciousBrand : ModItem
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.damage = 225;
            Item.DamageType = DamageClass.Melee;
            Item.width = 70;
            Item.height = 70;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RapaciousBrandSwing>();
            Item.shootSpeed = 24f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<RapaciousBrandSwing>(), damage, knockback, player.whoAmI, (float)player.direction * player.gravDir, 24); //, player.GetAdjustedItemScale(Item));
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, velocity, ModContent.ProjectileType<RapaciousBrandProjectile>(), (int)(damage * 0.75f), knockback, player.whoAmI);
            return false;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            base.HoldStyle(player, heldItemFrame);
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient<ArtemiteBar>(12);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}