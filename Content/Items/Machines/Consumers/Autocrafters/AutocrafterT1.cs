using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Autocrafters
{
    public class AutocrafterT1 : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Autocrafters.AutocrafterT1>());
            Item.width = 36;
            Item.height = 22;
            Item.value = Item.sellPrice(gold: 1);
            Item.mech = true;
        }

        public override void AddRecipes()
        {
        }
    }
}