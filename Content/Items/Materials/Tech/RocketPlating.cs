using Macrocosm.Content.Items.Crafting;
using Macrocosm.Content.Items.Materials.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials.Tech
{
    public class RocketPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.Purple;
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AluminumBar>(10)
                .AddIngredient<SteelBar>(10)
                .AddIngredient(ItemID.MeteoriteBar, 10)
                .AddIngredient(ItemID.AdamantiteBar, 10)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();

            CreateRecipe()
                .AddIngredient<AluminumBar>(10)
                .AddIngredient<SteelBar>(10)
                .AddIngredient(ItemID.MeteoriteBar, 10)
                .AddIngredient(ItemID.TitaniumBar, 10)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}