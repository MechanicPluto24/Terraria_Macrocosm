using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Tiles.Furniture.Industrial;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial
{
    public class IndustrialDoor : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<IndustrialDoorClosed>());
            Item.width = 12;
            Item.height = 32;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IndustrialPlating>(5)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
        }
    }
}