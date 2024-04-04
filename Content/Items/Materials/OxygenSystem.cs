using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials
{
    public class OxygenSystem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.OxygenSystem>());
            Item.width = 26;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.Green;
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<OxygenTank>(2)
            .AddIngredient<PrintedCircuitBoard>()
            .AddTile(TileID.WorkBenches) // TODO: @ fabricator
            .Register();
        }
    }
}