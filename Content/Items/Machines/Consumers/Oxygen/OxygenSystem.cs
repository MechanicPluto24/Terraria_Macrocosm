using Macrocosm.Content.Items.Refined;
using Macrocosm.Content.Items.Tech;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Oxygen;

public class OxygenSystem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Oxygen.OxygenSystem>());
        Item.width = 26;
        Item.height = 24;
        Item.value = 100;
        Item.rare = ItemRarityID.Green;

    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<Plastic>(10)
            .AddIngredient(ItemID.TinBar, 5)
            .AddIngredient<OxygenTank>(4)
            .AddIngredient<PrintedCircuitBoard>()
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
    }
}