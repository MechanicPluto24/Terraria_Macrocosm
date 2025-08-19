using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools.Axes;

public class AluminumAxe : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 5;
        Item.DamageType = DamageClass.Melee;
        Item.width = 32;
        Item.height = 28;
        Item.useTime = 27;
        Item.useAnimation = 19;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 4.5f;
        Item.value = Item.sellPrice(silver: 2, copper: 75);
        Item.rare = ItemRarityID.White;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.useTurn = true;
        Item.axe = 7;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<AluminumBar>(8)
        .AddRecipeGroup(RecipeGroupID.Wood, 4)
        .AddTile(TileID.Anvils)
        .Register();
    }
}