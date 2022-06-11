using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles {
	public class Hemostone : ModTile {
		public override void SetStaticDefaults()  {
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			MinPick = 275;
			MineResist = 3f;
			ItemDrop = ItemType<Items.Placeables.BlocksAndWalls.Hemostone>();
            AddMapEntry(new Color(129, 0, 0));
			HitSound = SoundID.Tink;
		}
	}
}