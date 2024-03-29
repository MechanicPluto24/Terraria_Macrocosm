using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Ammo
{
    public class InvarBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 6;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(copper: 3);
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.shoot = ModContent.ProjectileType<Projectiles.Friendly.Ranged.InvarBullet>();
            Item.shootSpeed = 3.4f;
            Item.ammo = AmmoID.Bullet;
        }
    }
}
