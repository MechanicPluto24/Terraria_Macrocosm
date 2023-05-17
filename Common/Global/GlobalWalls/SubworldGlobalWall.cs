using SubworldLibrary;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.GlobalWalls
{
	public class SubworldGlobalWall : GlobalWall
	{
		/// <summary>
		/// Unnoticeable lighting due to bad rendering at some subworld heigths for pitch black walls
		/// </summary>
		public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
		{
			if (SubworldSystem.AnyActive<Macrocosm>() && (r == 0 || g == 0 || b == 0))
			{
				r += 0.01f;
				g += 0.01f;
				b += 0.01f;
			}
		}
	}
}