using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Ammo;

public class RailgunBolt : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.damage = 18;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 14;
        Item.height = 26;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 4.5f;
        Item.value = Item.sellPrice(copper: 3);
        Item.rare = ModContent.RarityType<MoonRarity1>();
        Item.shoot = ModContent.ProjectileType<Projectiles.Friendly.Ranged.RailgunBolt>();
        Item.shootSpeed = 20f;
        Item.ammo = Item.type; // Custom ammo
    }

    public override void AddRecipes()
    {
    }
}
