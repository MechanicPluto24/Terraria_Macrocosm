﻿using Macrocosm.Common.Utils;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Tombstones
{
	public class MoonHeadstone : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 2;
            Utility.AddVariationToRubblemakers(Type, ModContent.TileType<Tiles.Tombstones.MoonTombstone>(), 3);
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 30;
			Item.useTurn = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Tiles.Tombstones.MoonTombstone>();
			Item.placeStyle = 2;
			Item.rare = ItemRarityID.Purple;
		}
	}
}