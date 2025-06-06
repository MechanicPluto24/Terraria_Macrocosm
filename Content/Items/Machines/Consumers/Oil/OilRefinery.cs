using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.LiquidContainers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Oil
{
    public class OilRefinery : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Oil.OilRefinery>());
            Item.width = 42;
            Item.height = 32;
            Item.value = Item.sellPrice(gold: 1);
            Item.mech = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(10)
                .AddIngredient(ItemID.CopperBar, 12)
                .AddIngredient<AluminumBar>(6)
                .AddIngredient<Canister>(2)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
            CreateRecipe()
                .AddIngredient<SteelBar>(10)
                .AddIngredient(ItemID.TinBar, 12)
                .AddIngredient<AluminumBar>(6)
                .AddIngredient<Canister>(2)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}