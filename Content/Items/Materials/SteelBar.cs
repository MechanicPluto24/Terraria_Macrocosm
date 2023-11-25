using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Materials
{
    public class SteelBar : ModItem
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
            Item.value = Item.sellPrice(silver: 35);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = TileType<Tiles.Bars.SteelBar>();
            Item.placeStyle = 0;
            Item.rare = ItemRarityID.Green;
            Item.material = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.IronBar, 2);
            recipe.AddIngredient(ItemID.Coal);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
        }
    }
}