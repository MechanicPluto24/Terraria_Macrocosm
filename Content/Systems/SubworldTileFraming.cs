using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Systems
{
	public class SubworldTileFraming : ModSystem
	{
		internal static void INTERNAL_SubworldTileFraming()
		{
			//for (int i = 0; i < Main.maxTilesX; i++)
			//{
			//	for (int j = Main.maxTilesY - 180; j < Main.maxTilesY; j++)
			//	{
			//		if (Framing.GetTileSafely(i, j).HasTile)
			//			WorldGen.SquareTileFrame(i, j);
			//		if (Main.tile[i, j] != null)
			//			WorldGen.SquareWallFrame(i, j);
			//	}
			//}
		}

		private bool _anySubworldActive;
		private bool _anySubworldActiveLastTick;
		public override void PostUpdateEverything()
		{
		//	_anySubworldActive = SubworldSystem.AnyActive<Macrocosm>();
		//	if (_anySubworldActive && !_anySubworldActiveLastTick)
		//		INTERNAL_SubworldTileFraming();
		//
		//	_anySubworldActiveLastTick = _anySubworldActive;
		}
	}
}