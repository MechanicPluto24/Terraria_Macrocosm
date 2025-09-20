using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tech;

public class LandingGear : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 30;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = 100;
        Item.rare = ItemRarityID.Purple;
        Item.material = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<SteelBar>(20)
            .AddIngredient<Gear>(10)
            .AddIngredient(ItemID.AdamantiteBar, 15)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();

        CreateRecipe()
            .AddIngredient<SteelBar>(20)
            .AddIngredient<Gear>(10)
            .AddIngredient(ItemID.TitaniumBar, 15)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
    }
}