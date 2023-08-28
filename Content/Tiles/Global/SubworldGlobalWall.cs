using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Global
{
	internal class SubworldGlobalWall : GlobalWall
	{
		/// <summary>
		/// Unnoticeable lighting due to bad rendering at some subworld heigths for pitch black walls
		/// </summary>
		public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
		{
			if (SubworldSystem.AnyActive<Macrocosm>() && j > 900 && (r == 0 || g == 0 || b == 0))
			{
				r = 0.004f;
				g = 0.004f;
				b = 0.004f;
			}
		}
	}
}