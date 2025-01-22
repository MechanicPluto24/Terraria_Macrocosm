using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools.Hammers
{
    public class RubberWoodHammer : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.damage = 4;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 21;
            Item.useAnimation = 31;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5.5f;
            Item.value = Item.sellPrice(copper: 30);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.hammer = 35;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<RubberTreeWood>(8)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}