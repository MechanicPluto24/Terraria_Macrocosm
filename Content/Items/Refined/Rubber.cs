using Macrocosm.Content.Items.Ores;
using Macrocosm.Content.Items.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Refined;

public class Rubber : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 26;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = 100;
        Item.rare = ItemRarityID.Green;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<RubberTreeSap>(3)
            .AddIngredient<Coal>()
            .AddTile(TileID.AlchemyTable)
            .Register();
    }
}