using Macrocosm.Content.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials;

public class PrintedCircuitBoard : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.PrintedCircuitBoard>());
        Item.width = 20;
        Item.height = 20;
        Item.value = 100;
        Item.rare = ItemRarityID.Green;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
          .AddIngredient<Silicon>(6)
          .AddIngredient(ItemID.Wire, 10)
          .AddIngredient(ItemID.SilverBar, 1)
          .AddTile(TileID.WorkBenches)
          .Register();
    }
}
