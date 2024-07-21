using Macrocosm.Common.Bases;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial
{
    [LegacyName("MoonBaseChest")]
    public class IndustrialChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemSets.Chests[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialChest>());
            Item.width = 32;
            Item.height = 28;
            Item.value = 500;

            Item.placeStyle = (int)Tiles.Furniture.Industrial.IndustrialChest.State.Normal;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IndustrialPlating>(8)
                .AddIngredient(ItemID.IronBar)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}
