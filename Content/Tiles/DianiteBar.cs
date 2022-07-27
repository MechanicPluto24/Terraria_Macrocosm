using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles {
    public class DianiteBar : ModTile {
        public override void SetStaticDefaults() {
            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(161, 68, 48), Language.GetText("Dianite Bar")); // localized text for "Metal Bar"
        }
        public override bool Drop(int x, int y) {
            Tile t = Main.tile[x, y];
            int style = t.TileFrameX / 18;
            if (style == 0) {
                Item.NewItem(new EntitySource_TileBreak(x, y), x * 16, y * 16, 16, 16, ItemType<Items.Materials.DianiteBar>());
            }
            return base.Drop(x, y);
        }
        public override bool CreateDust(int i, int j, ref int type) {
            type = Dust.NewDust(new Vector2(i, j).ToWorldCoordinates(), 16, 16, DustType<DianiteDust>());
            return false;
        }
    }
}