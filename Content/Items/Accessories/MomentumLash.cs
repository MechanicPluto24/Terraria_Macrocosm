using Macrocosm.Common.Players;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories;

public class MomentumLash : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ModContent.RarityType<MoonRarity1>();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<MomentumLashPlayer>().MomentumLash = true;
    }
}
