using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles {
	public class IrradiatedRock : ModTile {
		public override void SetStaticDefaults()  {
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			MinPick = 275;
			MineResist = 3f;
			ItemDrop = ItemType<Items.Placeables.BlocksAndWalls.IrradiatedRock>();
            AddMapEntry(new Color(129, 117, 0));
			HitSound = SoundID.Tink;
		}
	}
}