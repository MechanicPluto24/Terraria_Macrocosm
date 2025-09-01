using Macrocosm.Common.Players;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories;

public class StalwartTowerShield : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 52;
        Item.accessory = true;
        Item.defense = 5;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ModContent.RarityType<MoonRarity1>();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<StalwartPlayer>().StalwartShield = true;
    }
}
