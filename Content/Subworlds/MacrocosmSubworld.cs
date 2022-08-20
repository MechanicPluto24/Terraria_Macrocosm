using SubworldLibrary;
using Terraria;

namespace Macrocosm.Content.Subworlds
{
	public abstract class MacrocosmSubworld : Subworld
	{
		public virtual double TimeRate { get; set; } = 1.0;
		public virtual double DayLenght { get; set; } = Main.dayLength;
		public virtual double NightLenght { get; set; } = Main.nightLength;
		public virtual float GravityMultiplier { get; set; } = 1f;

		public static MacrocosmSubworld Current() => SubworldSystem.Current as MacrocosmSubworld;
		public static bool AnyActive() => SubworldSystem.AnyActive<Macrocosm>();

		//public static bool IsActive<T>() where T : Subworld => SubworldSystem.IsActive<T>();
	}
}
