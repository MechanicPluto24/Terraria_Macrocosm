using Macrocosm.Common.Players;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories;

public class ArcaneBarnacle : ModItem
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
        var leechPlayer = player.GetModPlayer<LeechPlayer>();
        player.GetDamage<MagicDamageClass>() += 0.1f;
        player.manaCost += 0.70f;
        leechPlayer.PercentLeechMana += 0.02f;
        leechPlayer.DisableManaPotions = true;
    }
}
