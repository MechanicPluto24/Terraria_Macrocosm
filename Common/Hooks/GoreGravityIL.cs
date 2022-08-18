using MonoMod.Cil;
using Mono.Cecil.Cil;
using Terraria.ModLoader;
using SubworldLibrary;
using Macrocosm.Content.Subworlds.Moon;
using Terraria;

namespace Macrocosm.Common.Hooks
{
	public class GoreGravityIL : ILoadable
	{
		public void Load(Mod mod)
		{
			IL.Terraria.Gore.Update += LowerGoreGravity;
		}

		public void Unload() { }

		private static void LowerGoreGravity(ILContext il)
		{
			var c = new ILCursor(il);

			// matches "if (type < 411 || type > 430)" (general gores)
			if (!c.TryGotoNext(
				i => i.MatchLdfld<Gore>("type"),
				i => i.MatchLdcI4(430)
				)) return;

			// matches "velocity.Y += 0.2f"
			if (!c.TryGotoNext(i => i.MatchLdcR4(0.2f))) return;

			c.Remove();
			c.EmitDelegate(GetGoreGravity); 
		}

		// replace gravity increment with desired value 
		private static float GetGoreGravity()
		{
			if (SubworldSystem.IsActive<Moon>())
				return 0.033f; // base gravity divided by 6 

			return 0.2f;
		}

	}
}