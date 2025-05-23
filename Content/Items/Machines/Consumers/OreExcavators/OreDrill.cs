using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.OreExcavators
{
    public class OreDrill : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Drills.OreDrill>());
            Item.width = 36;
            Item.height = 50;
            Item.value = Item.sellPrice(gold: 2);
            Item.mech = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient<SteelBar>(5)
               .AddIngredient(ItemID.Wire, 25)
               .AddIngredient<AluminumBar>(20)
               .AddIngredient(ItemID.Diamond, 1)
               .AddTile(TileID.Anvils)
               .Register();
        }
    }
}