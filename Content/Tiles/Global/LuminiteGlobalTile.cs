
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Global
{
	public class LuminiteGlobalTile : GlobalTile
	{
		public override void Load()
		{
			On_Player.GetPickaxeDamage += On_Player_GetPickaxeDamage;
		}

		public override void Unload()
		{
			On_Player.GetPickaxeDamage -= On_Player_GetPickaxeDamage;
		}

		// TODO: modify MineResist as well

		// TODO: modify pickaxe power required 
		private int On_Player_GetPickaxeDamage(On_Player.orig_GetPickaxeDamage orig, Player self, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget)
		{
			return orig(self, x, y, pickPower, hitBufferIndex, tileTarget);
		}

		public override bool? IsTileSpelunkable(int i, int j, int type)
		{
			if (type is TileID.LunarOre)
				return true;
			else
				return null;
		}
	}
}
