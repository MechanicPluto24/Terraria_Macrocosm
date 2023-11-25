using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture.MoonBase
{
    public class MoonBaseChestElectronic : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoonBase.MoonBaseChest>());
            Item.width = 32;
            Item.height = 28;
            Item.value = 500;

            Item.placeStyle = (int)Tiles.Furniture.MoonBase.MoonBaseChest.State.Unlocked;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MoonBaseChest>()
                .AddIngredient<PrintedCircuitBoard>()
                .Register();
        }
    }
}
