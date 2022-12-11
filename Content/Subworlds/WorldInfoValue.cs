

using System;
using System.Globalization;

namespace Macrocosm.Content.Subworlds
{
	public enum UnitType
	{
		None,
		Gravity,
		Radius,
		DayPeriod
	}

	public struct WorldInfoValue
	{
		private float number = 0f;
		private string specialValue = "";
		private UnitType unitType = UnitType.None;

		public float Number { get => number; set => number = value; }
		public string SpecialValue { get => specialValue; set => specialValue = value; }

		public bool HasSpecial() => specialValue != "";

		public void SetUnits(UnitType type)
		{
			unitType = type;
		}

		/// <summary>
		/// Used for unit conversion (TODO) 
		/// </summary>
		/// <param name="type"> The type of this unit </param>
		/// <param name="special"> Whether to skip the logic (has special value) </param>
		/// <returns></returns>
		public string GetUnitText(bool singular = false)
		{
			if (!HasSpecial())
				return unitType switch
				{
					UnitType.Gravity => "G",
					UnitType.Radius => "km",
					UnitType.DayPeriod => singular ? "day" : "days", // localize this lmao
					_ => "",
				};

			return "";
		}

		public WorldInfoValue(string special)
		{
			number = float.MinValue;
			unitType = UnitType.None;
			specialValue = special;
		}

		public WorldInfoValue(float number, UnitType type = UnitType.None)
		{
			this.number = number;
			unitType = type;
			specialValue = "";
		}

		/// <summary>
		/// Assign the structure a text value which will reset the numeric values
		/// </summary>
		public static implicit operator WorldInfoValue(string value)
			=> new(value);
 
		/// <summary>
		/// Assign the structure a numeric value using a float literal. 
		/// This will reset the unit type!
		/// </summary>
		public static implicit operator WorldInfoValue(float number) 
			=> new(number);

		public static implicit operator string(WorldInfoValue value) 
			 => value.HasSpecial() ? value.specialValue : String.Format("{0:n}", value.Number).TrimEnd('0').TrimEnd('.');

		public static implicit operator float(WorldInfoValue value)
			=> value.number;

		public static bool operator !=(WorldInfoValue value, string other)
			=> value.specialValue != other;

		public static bool operator ==(WorldInfoValue value, string other)
			=> value.specialValue != other;

		public static bool operator !=(WorldInfoValue value, float other)
			=> value.number != other;

		public static bool operator ==(WorldInfoValue value, float other)
			=> value.number != other;

		public static bool operator >(WorldInfoValue value, float other)
			=> value.number > other;

		public static bool operator <(WorldInfoValue value, float other)
			=> value.number < other;

		public static bool operator <=(WorldInfoValue value, float other)
			=> value.number <= other;

		public static bool operator >=(WorldInfoValue value, float other)
			=> value.number >= other;

		public override bool Equals(object obj)
		{
			if (obj is string text)
				return specialValue == text;
			else if (obj is float number)
				return this.number == number;
			else
				return false;
 		}

		public override int GetHashCode()
			=> specialValue.GetHashCode() ^ number.GetHashCode() ^ unitType.GetHashCode();
	}
}
