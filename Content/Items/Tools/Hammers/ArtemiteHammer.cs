using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools.Hammers
{
    public class ArtemiteHammer : ModItem
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.width = 44;
            Item.height = 38;
            Item.useTime = 5;
            Item.useAnimation = 12;
            Item.hammer = 125;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.tileBoost = 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<ArtemiteBar>(12)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}