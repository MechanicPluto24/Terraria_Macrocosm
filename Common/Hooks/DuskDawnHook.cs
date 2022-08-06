using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
	public class DuskDawnHook : ILoadable
	{
		public void Load(Mod mod)
		{
			//On.Terraria.Main.UpdateTime_StartDay += OnDawn;
			//On.Terraria.Main.UpdateTime_StartNight += OnDusk;
		}

		public void Unload() { }

		private static void OnDawn(On.Terraria.Main.orig_UpdateTime_StartDay orig, ref bool stopEvents)
		{
			// TODO: add here some bg star randomizations on the Moon 
			orig(ref stopEvents);
		}

		private static void OnDusk(On.Terraria.Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
		{
			// TODO: add here some bg star randomizations on the Moon 
			orig(ref stopEvents);
		}


	}
}