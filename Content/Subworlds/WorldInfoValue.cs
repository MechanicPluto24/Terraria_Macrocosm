

using System;
using System.Globalization;

namespace Macrocosm.Content.Subworlds
{
	public struct WorldInfoValue
	{
		private float number = 0f;
		private string specialValue = "";

		public float Number { get => number; set => number = value; }
		public string SpecialValue { get => specialValue; set => specialValue = value; }

		public bool HasSpecial() => specialValue != "";

		public WorldInfoValue(float number = 0f, string special = "")
		{
			this.number = number;
			specialValue = special;
		}

		public static implicit operator WorldInfoValue(string value)
			=> new(special: value);
 
		public static implicit operator WorldInfoValue(float number) 
			=> new(number: number);

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
			=> specialValue.GetHashCode() ^ number.GetHashCode();
	}
}
