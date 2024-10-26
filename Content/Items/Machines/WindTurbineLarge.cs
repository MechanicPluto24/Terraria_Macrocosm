using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Refined;
using Terraria.ID;

namespace Macrocosm.Content.Items.Machines
{
    public class WindTurbineLarge : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.WindTurbineLarge>());
            Item.width = 28;
            Item.height = 56;
            Item.value =  Item.sellPrice(silver:25);
            Item.mech = true;
        }
        public override void AddRecipes()
        {
             CreateRecipe()
                .AddIngredient<SteelBar>(40)
                .AddIngredient(ItemID.Wire, 32)
                .AddIngredient<Plastic>(8)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}