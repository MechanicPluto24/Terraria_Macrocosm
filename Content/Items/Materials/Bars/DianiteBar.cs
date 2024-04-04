using Macrocosm.Content.Items.Materials.Ores;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials.Bars
{
    public class DianiteBar : ModItem
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
            Item.value = 750;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Bars.DianiteBar>();
            Item.placeStyle = 0;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.material = true;

        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<DianiteOre>(6)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}