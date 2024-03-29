using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools
{
    public class ZombieFinger : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.GoldenKey);
            Item.width = 22;
            Item.height = 14;
            Item.value = 200;
            Item.rare = ModContent.RarityType<MoonRarityT2>();
        }
    }
}
