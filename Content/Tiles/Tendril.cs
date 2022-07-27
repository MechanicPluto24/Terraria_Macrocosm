using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles {
    public class Tendril : ModTile {
        public override void SetStaticDefaults() {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileMergeDirt[Type] = true;
            MinPick = 300;
            MineResist = 3f;
            ItemDrop = ItemType<Items.Placeables.BlocksAndWalls.Tendril>();
            AddMapEntry(new Color(188, 0, 0));
        }
    }
}