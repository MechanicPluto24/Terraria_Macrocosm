using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Refined;
using Macrocosm.Content.Items.Tech;
using Macrocosm.Content.Tiles.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Oxygen;

public class ACUnit : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Oxygen.ACUnit>());
        Item.width = 26;
        Item.height = 16;
        Item.value = 100;
        Item.rare = ItemRarityID.LightRed;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<AluminumBar>(6)
            .AddIngredient<Plastic>(2)
            .AddIngredient<PrintedCircuitBoard>()
            .AddTile<Fabricator>()
            .Register();
    }
}