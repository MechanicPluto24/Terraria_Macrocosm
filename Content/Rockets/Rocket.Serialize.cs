using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using System;
using Terraria;
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

			foreach (string moduleName in ModuleNames)
				tag[moduleName] = Modules[moduleName];
 
			return tag;
		}

		public static Rocket DeserializeData(TagCompound tag)
		{
			Rocket rocket = new() {
				WhoAmI = tag.GetInt(nameof(WhoAmI)),

				Active = tag.GetBool(nameof(Active)),
				Position = tag.Get<Vector2>(nameof(Position)),

				Fuel = tag.GetFloat(nameof(Fuel)),
				FuelCapacity = tag.GetFloat(nameof(FuelCapacity)),

				InFlight = tag.GetBool(nameof(InFlight)),
				Descending = tag.GetBool(nameof(Descending)),

				CurrentSubworld = tag.GetString(nameof(CurrentSubworld))
			};

			foreach (string moduleName in rocket.ModuleNames)
				if (tag.ContainsKey(moduleName))
					rocket.Modules[moduleName] = tag.Get<RocketModule>(moduleName);

			return rocket;
		}
	}
}
