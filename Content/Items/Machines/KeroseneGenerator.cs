using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines
{
    public class KeroseneGenerator : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.KeroseneGenerator>());
            Item.width = 52;
            Item.height = 30;
            Item.value = Item.sellPrice(gold: 1);
            Item.mech = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(18)
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient(ItemID.TitaniumBar, 6)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
            CreateRecipe()
                .AddIngredient<SteelBar>(18)
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient(ItemID.AdamantiteBar, 6)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }

    }
}