using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture
{
    public class SolarPanelLarge : ModItem
    {
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Large Solar Panel");
		}

		public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 34;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.rare = ItemRarityID.White;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Furniture.SolarPanelLarge>();
        }
    }
}