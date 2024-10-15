using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Keys
{
    public class XaocKey : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ShadowKey);
            Item.width = 18;
            Item.height = 30;
            Item.value = 200;
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.consumable = false;
        }
    }
}
