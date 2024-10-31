using Macrocosm.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
    public class ThaumaturgicWard : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ManaWardPlayer>().ManaWard = true;
            player.GetDamage<GenericDamageClass>() -= 100;
            player.GetDamage<MagicDamageClass>() += 100;
        }
    }
}
