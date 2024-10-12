using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
    public class ArcaneBarnacle : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.Coral}";

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
            leechPlayer.percentLeechMana += 0.01f;
            leechPlayer.manaPotionDisabler = true;
        }
    }
}
