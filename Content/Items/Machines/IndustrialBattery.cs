using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Refined;
using Macrocosm.Content.Items.Tech;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines
{
    public class IndustrialBattery : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.IndustrialBattery>());
            Item.width = 44;
            Item.height = 34;
            Item.value = Item.sellPrice(silver: 90);
            Item.mech = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient<Battery>(6)
               .AddIngredient(ItemID.IronBar, 3)
               .AddTile<Tiles.Crafting.Fabricator>()
               .Register();

            CreateRecipe()
               .AddIngredient<Battery>(6)
               .AddIngredient(ItemID.LeadBar, 3)
               .AddTile<Tiles.Crafting.Fabricator>()
               .Register();
        }
    }
}