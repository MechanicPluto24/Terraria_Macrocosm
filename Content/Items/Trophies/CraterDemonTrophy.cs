using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Trophies
{
    public class CraterDemonTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Trophies.CraterDemonTrophy>());
            Item.width = 32;
            Item.height = 32;
            Item.rare = ModContent.RarityType<MoonRarity1>();
            Item.value = Item.buyPrice(0, 1);
        }
    }
}
