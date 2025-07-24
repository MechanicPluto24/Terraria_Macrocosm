using Macrocosm.Content.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Refined;

public class Plastic : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = 100;
        Item.rare = ItemRarityID.LightRed;

    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<Coal>(3)
            .AddIngredient<OilShale>(1)
            .AddTile(TileID.AlchemyTable)
            .Register();
    }
}