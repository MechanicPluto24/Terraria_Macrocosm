using Macrocosm.Content.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Bars
{
    public class InvarBar : ModItem
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
            Item.createTile = TileType<Tiles.Bars.InvarBar>();
            Item.placeStyle = 0;
            Item.rare = ItemRarityID.Green;
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IronBar)
                .AddIngredient(ItemType<NickelOre>())
                .AddTile(TileID.Hellforge)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.LeadBar)
                .AddIngredient(ItemType<NickelOre>())
                .AddTile(TileID.Hellforge)
                .Register();
        }
    }
}