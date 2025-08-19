using Macrocosm.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories.Info;

public class Barometer : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 40;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<InfoDisplayPlayer>().Barometer = true;
    }
}
