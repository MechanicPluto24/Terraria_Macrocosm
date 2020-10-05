using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Tiles
{
	public class HemostoneBrick : ModTile
	{
		public override void SetDefaults() 
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			minPick = 275;
			mineResist = 3f;
			drop = ItemType<Items.Placeables.BlocksAndWalls.HemostoneBrick>();
            AddMapEntry(new Color(129, 0, 0));
			soundType = SoundID.Tink;
		}
	}
}