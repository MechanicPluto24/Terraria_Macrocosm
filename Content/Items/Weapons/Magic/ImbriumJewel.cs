using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class ImbriumJewel : ModItem
    {
        public override void SetStaticDefaults()
        {

            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ImbriumJewelProjectile>();
            Item.shootSpeed = 8f;
        }
    }
}
