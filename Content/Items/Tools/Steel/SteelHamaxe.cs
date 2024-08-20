using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools.Steel
{
    public class SteelHamaxe : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Melee;
            Item.width = 42;
            Item.height = 42;
            Item.useTime = 22;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.axe = 28;
            Item.hammer = 65;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<SteelBar>(12)
            .AddRecipeGroup(RecipeGroupID.Wood, 4)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}