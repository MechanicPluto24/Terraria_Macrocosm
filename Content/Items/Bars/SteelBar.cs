﻿using Macrocosm.Content.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Bars
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
            CreateRecipe(3)
                .AddIngredient(ItemID.IronBar, 8)
                .AddIngredient(ItemType<Coal>())
                .AddTile(TileID.Hellforge)
                .Register();

            CreateRecipe(3)
                .AddIngredient(ItemID.LeadBar, 8)
                .AddIngredient(ItemType<Coal>())
                .AddTile(TileID.Hellforge)
                .Register();
        }
    }
}