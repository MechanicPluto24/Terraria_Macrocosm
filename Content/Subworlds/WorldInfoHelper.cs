
namespace Macrocosm.Content.Subworlds
{
	public enum WorldInfoUnitType
	{
		Gravity,
		Radius,
		DayPeriod
	}

	public class WorldInfoHelper
	{
		/// <summary>
		/// Used for unit conversion (TODO) 
		/// </summary>
		/// <param name="type"> The type of this unit </param>
		/// <param name="special"> Whether to skip the logic (has special value) </param>
		/// <returns></returns>
		public static string GetUnitText(WorldInfoUnitType type, bool special = false)
		{
			if (!special) switch (type)
			{
				case WorldInfoUnitType.Gravity:
					return "G";

				case WorldInfoUnitType.Radius:
					return "km";

				case WorldInfoUnitType.DayPeriod:
					return "days";
			}

			return "";
		}
	}
}
