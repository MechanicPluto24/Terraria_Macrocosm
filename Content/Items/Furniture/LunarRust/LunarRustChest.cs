using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.LunarRust
{
    public class LunarRustChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteChest>(), (int)LuminiteStyle.LunarRust*2);
            Item.width = 32;
            Item.height = 24;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarRustBrick, 8)
                .AddRecipeGroup(RecipeGroupID.IronBar, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
