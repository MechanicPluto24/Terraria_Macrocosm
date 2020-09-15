using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Tiles
{
	public class Protolith : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			minPick = 225;
			mineResist = 3f;
			drop = ItemType<Items.Placeables.BlocksAndWalls.Protolith>();
			AddMapEntry(new Color(65, 65, 65));
			soundType = SoundID.Tink;
		}
	}
}