using Macrocosm.Content.Items.Ores;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Bars
{
    public class SeleniteBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(gold: 1);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = TileType<Tiles.Bars.SeleniteBar>();
            Item.placeStyle = 0;
            Item.rare = RarityType<MoonRarityT1>();
            

            // Set other Item.X values here
        }

        public override void AddRecipes()
        {

            CreateRecipe()
            .AddIngredient<SeleniteOre>(6)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}