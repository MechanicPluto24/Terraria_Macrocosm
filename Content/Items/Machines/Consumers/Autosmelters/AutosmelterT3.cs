using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Autosmelters
{
    public class AutosmelterT3 : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Autosmelters.AutosmelterT3>());
            Item.width = 36;
            Item.height = 22;
            Item.value = Item.sellPrice(gold: 1);
            Item.mech = true;
        }
    }
}

