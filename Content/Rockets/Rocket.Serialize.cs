using Macrocosm.Content.Rockets.Modules;
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
			TagCompound tag = new()
			{
				[nameof(WhoAmI)] = WhoAmI,

				[nameof(Active)] = Active,
				[nameof(Position)] = Position,

				[nameof(InFlight)] = InFlight,
				[nameof(Descending)] = Descending,

				[nameof(Fuel)] = Fuel,
				[nameof(FuelCapacity)] = FuelCapacity,

				[nameof(CurrentSubworld)] = CurrentSubworld
			};

			//for(int i = 0; i < Modules.Count; i++)
			//{
			//	tag[ModuleNames[i]] = (RocketModule)Modules[i];
			//}

			return tag;
		}

		public static Rocket DeserializeData(TagCompound tag)
		{
			return new Rocket()
			{
				WhoAmI = tag.GetInt(nameof(WhoAmI)),

				Active = tag.GetBool(nameof(Active)),
				Position = tag.Get<Vector2>(nameof(Position)),

				Fuel = tag.GetFloat(nameof(Fuel)),	
				FuelCapacity = tag.GetFloat(nameof(FuelCapacity)),

				InFlight = tag.GetBool(nameof(InFlight)),
				Descending = tag.GetBool(nameof(Descending)),

				CurrentSubworld = tag.GetString(nameof(CurrentSubworld))
			};
		}
	}
}
