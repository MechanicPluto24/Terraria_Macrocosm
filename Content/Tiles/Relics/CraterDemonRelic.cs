using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Relics
{
    public class CraterDemonRelic : BaseRelic
    {
        public override string RelicTextureName => "Macrocosm/Content/Tiles/Relics/CraterDemonRelic";

        public override bool RightPlaceStyle => false;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeable.Relics.CraterDemonRelic>());
        }
    }
}
