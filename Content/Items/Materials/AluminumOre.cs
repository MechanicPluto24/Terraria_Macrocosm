﻿using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Materials
{
	public class AluminumOre : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aluminum Ore");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 9999;
			Item.value = 750;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = TileType<Tiles.AluminumOre>();
			Item.placeStyle = 0;
			Item.rare = ItemRarityID.White;
			Item.material = true;
			// Set other Item.X values here
		}

		public override void AddRecipes()
		{

		}
	}
}