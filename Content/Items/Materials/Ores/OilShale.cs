﻿using Macrocosm.Common.Sets.Items;
using Macrocosm.Content.Liquids;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Materials.Ores
{
    public class OilShale : ModItem, ILiquidExtractable
    {
        public LiquidType LiquidType => LiquidType.Oil;
        public float ExtractedAmount => 10f;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
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
            Item.createTile = TileType<Tiles.Ores.OilShale>();
            Item.placeStyle = 0;
            Item.rare = ItemRarityID.Blue;
            Item.material = true;

            // Set other Item.X values here
        }

        public override void AddRecipes()
        {

        }
    }
}