using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Generators.Solar
{
    public class SolarPanelExtraLarge : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Generators.Solar.SolarPanelExtraLarge>());
            Item.width = 50;
            Item.height = 46;
            Item.value = Item.sellPrice(gold: 4);
            Item.mech = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient<SolarCell>(64)
               .AddIngredient(ItemID.Glass, 24)
               .AddIngredient(ItemID.Wire, 32)
               .AddTile<Tiles.Crafting.Fabricator>()
               .Register();
        }
    }
}