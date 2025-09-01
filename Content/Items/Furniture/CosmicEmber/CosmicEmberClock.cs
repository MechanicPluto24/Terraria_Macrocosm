using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.CosmicEmber
{
    public class CosmicEmberClock : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteClock>(), (int)LuminiteStyle.CosmicEmber);
            Item.width = 20;
            Item.height = 40;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CosmicEmberBrick, 20)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
