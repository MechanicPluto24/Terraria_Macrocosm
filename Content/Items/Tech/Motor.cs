using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tech;

public class Motor : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Tech.Motor>());
        Item.width = 20;
        Item.height = 20;
        Item.value = 100;
        Item.rare = ItemRarityID.Green;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<Gear>(2)
            .AddIngredient<SteelBar>(5)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
    }
}