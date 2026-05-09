using Macrocosm.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories.Info;

public class Nephelometer : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 16;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateInfoAccessory(Player player)
    {
        player.GetModPlayer<InfoDisplayPlayer>().Nephelometer = true;
    }
}
