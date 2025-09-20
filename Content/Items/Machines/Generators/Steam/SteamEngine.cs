using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Generators.Steam;

public class SteamEngine : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Generators.Steam.SteamEngine>());
        Item.width = 68;
        Item.height = 48;
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }
}

