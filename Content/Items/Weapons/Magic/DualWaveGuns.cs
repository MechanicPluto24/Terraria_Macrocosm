using Macrocosm.Content.Projectiles.Friendly.Magic.WaveGuns;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class DualWaveGuns : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 54;
            Item.height = 36;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 10;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<DualWaveGunsHeld>();
            Item.shootSpeed = 28f;
            Item.channel = true;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<DualWaveGunsHeld>()] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float maxCharge = 90f * player.GetAttackSpeed(DamageClass.Magic);
            Vector2 aim = velocity;
            Projectile.NewProjectileDirect(source, position, aim, ModContent.ProjectileType<DualWaveGunsHeld>(), damage, knockback, player.whoAmI, maxCharge);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<WaveGunBlue>(1)
            .AddIngredient<WaveGunRed>(1)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}