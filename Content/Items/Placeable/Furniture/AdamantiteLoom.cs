using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture
{
    public class AdamantiteLoom : ModItem
    {
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Adamantite Loom");
		}

		public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.rare = ItemRarityID.White;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Furniture.TitaniumLoom>();
            Item.placeStyle = 1;
        }
    }
}