

using System;
using System.Globalization;

namespace Macrocosm.Common.Subworlds
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
        private float value = 0f;
        private string specialValue = "";
        private UnitType unitType = UnitType.None;

        public float Value { get => value; set => this.value = value; }
        public string SpecialValue { get => specialValue; set => specialValue = value; }
        public UnitType UnitType { get => unitType; set => unitType = value; }

        public bool HasSpecial() => specialValue != "";

        public void SetUnits(UnitType type) => unitType = type;
        public void SetValue(float value) => this.value = value;
        public void SetSpecialValue(string specialValue) => this.specialValue = specialValue;

        /// <summary> Used for unit conversion (TODO) </summary>
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
            value = float.MinValue;
            unitType = UnitType.None;
            specialValue = special;
        }

        public WorldInfoValue(float number, UnitType type = UnitType.None)
        {
            value = number;
            unitType = type;
            specialValue = "";
        }

        /// <summary>
        /// Assign the structure a text value which will reset the numeric value and the unit
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
             => value.HasSpecial() ? value.specialValue : string.Format("{0:n}", value.Value).TrimEnd('0').TrimEnd('.');

        public static implicit operator float(WorldInfoValue value)
            => value.value;

        public static bool operator !=(WorldInfoValue value, string other)
            => value.specialValue != other;

        public static bool operator ==(WorldInfoValue value, string other)
            => value.specialValue != other;

        public static bool operator !=(WorldInfoValue value, float other)
            => value.value != other;

        public static bool operator ==(WorldInfoValue value, float other)
            => value.value != other;

        public static bool operator >(WorldInfoValue value, float other)
            => value.value > other;

        public static bool operator <(WorldInfoValue value, float other)
            => value.value < other;

        public static bool operator <=(WorldInfoValue value, float other)
            => value.value <= other;

        public static bool operator >=(WorldInfoValue value, float other)
            => value.value >= other;

        public override bool Equals(object obj)
        {
            if (obj is string text)
                return specialValue == text;
            else if (obj is float number)
                return value == number;
            else
                return false;
        }

        public override int GetHashCode()
            => specialValue.GetHashCode() ^ value.GetHashCode() ^ unitType.GetHashCode();
    }
}
