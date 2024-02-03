﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Materials
{
	public class Coal : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 16;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 750;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = TileType<Tiles.Ores.Coal>();
			Item.placeStyle = 0;
			Item.rare = ItemRarityID.White;
			Item.material = true;
		}

		public override void AddRecipes()
		{
			Recipe.Create(ItemID.Torch, 5)
			.AddIngredient(Type, 1)
			.AddRecipeGroup(RecipeGroupID.Wood, 1)
			.Register();
		}
	}
}