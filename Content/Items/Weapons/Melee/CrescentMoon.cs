using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class CrescentMoon : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 350;
            Item.DamageType = DamageClass.Melee;
            Item.width = 36;
            Item.height = 36;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: 3, copper: 10);
            Item.rare = ModContent.RarityType<MoonRarity2>();
            Item.UseSound = SoundID.Item7;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CrescentMoonProjectile>();
            Item.shootSpeed = 20;
            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 2;

    }
}