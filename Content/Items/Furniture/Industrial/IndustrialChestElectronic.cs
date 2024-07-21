using Macrocosm.Common.Sets;
using Macrocosm.Common.Sets.Items;
using Macrocosm.Content.Items.Materials.Tech;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial
{
    [LegacyName("MoonBaseChestElectronic")]
    public class IndustrialChestElectronic : ModItem
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

            Item.placeStyle = (int)Tiles.Furniture.Industrial.IndustrialChest.State.Unlocked;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IndustrialChest>()
                .AddIngredient<PrintedCircuitBoard>()
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}
