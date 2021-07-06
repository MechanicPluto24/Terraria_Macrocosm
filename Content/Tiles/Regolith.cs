using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;

namespace Macrocosm.Content.Tiles
{
	public class Regolith : ModTile
	{
		public override void SetDefaults() 
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileMerge[Type][ModContent.TileType<Protolith>()] = true;
			minPick = 225;
			mineResist = 3f;
			drop = ModContent.ItemType<Items.Placeables.BlocksAndWalls.Regolith>();
			AddMapEntry(new Color(90, 90, 90));
			soundType = SoundID.Dig;
		}
		public override bool HasWalkDust()
		{
			return true;
		}
		public override bool CreateDust(int i, int j, ref int type)
        {
			type = Dust.NewDust(new Vector2(i, j).ToWorldCoordinates(), 16, 16, ModContent.DustType<RegolithDust>());
			return false;
        }
    }
}