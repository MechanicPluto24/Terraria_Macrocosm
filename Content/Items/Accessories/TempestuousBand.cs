using Macrocosm.Common.Players;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
    public class TempestuousBand : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<MoonRarityT1>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MacrocosmPlayer>().ExtraCritDamagePercent += 0.20f;
            player.GetModPlayer<MacrocosmPlayer>().NonCritDamageMultiplier -= 0.20f;
            player.GetCritChance<GenericDamageClass>() += 15;
        }
    }
}
