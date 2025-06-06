using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Drills
{
    public class OreExcavator : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Drills.OreExcavator>());
            Item.width = 60;
            Item.height = 84;
            Item.value = Item.sellPrice(gold: 10);
            Item.mech = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient<SteelBar>(20)
               .AddIngredient(ItemID.Wire, 50)
               .AddIngredient(ItemID.AdamantiteBar, 10)
               .AddIngredient<AluminumBar>(20)
               .AddIngredient(ItemID.Diamond, 10)
               .AddTile<Tiles.Crafting.Fabricator>()
               .Register();

            CreateRecipe()
                .AddIngredient<SteelBar>(20)
                .AddIngredient(ItemID.Wire, 50)
                .AddIngredient(ItemID.TitaniumBar, 10)
                .AddIngredient<AluminumBar>(20)
                .AddIngredient(ItemID.Diamond, 10)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}