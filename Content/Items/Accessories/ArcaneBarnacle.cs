using Macrocosm.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
    public class ArcaneBarnacle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var leechPlayer = player.GetModPlayer<LeechPlayer>();
            player.GetDamage<MagicDamageClass>() += 0.15f;
            player.manaCost += 1f;
            leechPlayer.PercentLeechMana += 0.01f;
            leechPlayer.DisableManaPotions = true;
        }
    }
}
