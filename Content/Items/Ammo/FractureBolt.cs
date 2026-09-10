using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Ores;

namespace Macrocosm.Content.Items.Ammo;

public class FractureBolt : ModItem
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
        Item.shoot = ModContent.ProjectileType<Projectiles.Friendly.Ranged.FractureBolt>();
        Item.shootSpeed = 20f;
        Item.ammo = ModContent.ItemType<RailgunBolt>(); // Custom ammo
    }

    public override void AddRecipes()
    {
        CreateRecipe(50)
            .AddIngredient<RailgunBolt>(50)
            .AddIngredient<ArtemiteBar>(1)
            .AddIngredient<QuartzFragment>(5)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
