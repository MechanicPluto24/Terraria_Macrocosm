using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets
{
	public partial class Rocket : TagSerializable
	{
		public static readonly Func<TagCompound, Rocket> DESERIALIZER = DeserializeData;

		public TagCompound SerializeData()
		{
			return new TagCompound
			{
				[nameof(Active)] = Active,
				[nameof(Position)] = Position,

				[nameof(Fuel)] = Fuel,
				[nameof(FuelCapacity)] = FuelCapacity,

				[nameof(CurrentSubworld)] = CurrentSubworld

				// TODO: nested serialization of rocket modules, possbily each will implement it on their own

			};
		}

		public static Rocket DeserializeData(TagCompound tag)
		{
			return new Rocket()
			{
				Active = tag.GetBool(nameof(Active)),
				Position = tag.Get<Vector2>(nameof(Position)),

				Fuel = tag.GetFloat(nameof(Fuel)),	
				FuelCapacity = tag.GetFloat(nameof(FuelCapacity)),

				CurrentSubworld = tag.GetString(nameof(CurrentSubworld))
			};
		}
	}
}
