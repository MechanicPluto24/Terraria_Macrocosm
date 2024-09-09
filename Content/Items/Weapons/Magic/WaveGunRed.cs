using Macrocosm.Content.Projectiles.Friendly.Magic.WaveGuns;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class WaveGunRed : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 54;
            Item.height = 36;
            Item.useTime = 20;
            Item.noUseGraphic = true;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 10;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WaveGunRedHeld>();
            Item.shootSpeed = 28f;
            Item.channel = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int fireRate = (int)(Item.useTime * player.GetAttackSpeed(DamageClass.Magic));
            Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<WaveGunRedHeld>(), damage, knockback, player.whoAmI, ai0: fireRate);
            return false;
        }
    }
}
