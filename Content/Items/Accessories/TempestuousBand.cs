using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Players;
using Terraria;
using Terraria.ID;
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
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CritMultiPlayer>().AddCritMulti += 0.25f;
            player.GetModPlayer<CritMultiPlayer>().NonCritMulti -= 0.25f;
            player.GetCritChance<GenericDamageClass>() += 15;
        }
    }
}
