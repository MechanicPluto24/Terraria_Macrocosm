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
				[nameof(Landing)] = Landing,

				[nameof(TargetLandingPosition)] = TargetLandingPosition,

				[nameof(Fuel)] = Fuel,
				[nameof(FuelCapacity)] = FuelCapacity,

				[nameof(CurrentSubworld)] = CurrentSubworld
			};

			foreach (string moduleName in ModuleNames)
				tag[moduleName] = Modules[moduleName];

			if (WhoAmI == 0)
				Main.NewText("");
 
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

				TargetLandingPosition = tag.Get<Vector2>(nameof(TargetLandingPosition)),

				InFlight = tag.GetBool(nameof(InFlight)),
				Landing = tag.GetBool(nameof(Landing)),

				CurrentSubworld = tag.GetString(nameof(CurrentSubworld))
			};

			foreach (string moduleName in rocket.ModuleNames)
			{
				RocketModule module = RocketModule.DeserializeData(tag.GetCompound(moduleName));

				Type moduleType = Type.GetType(moduleName);
				if (moduleType != null && moduleType.IsSubclassOf(typeof(RocketModule)))
				{
					RocketModule typedModule = Activator.CreateInstance(moduleType) as RocketModule;
					rocket.Modules[moduleName] = typedModule;
				}
				else
				{
					rocket.Modules[moduleName] = module;
				}
			}


			if (rocket.WhoAmI == 0)
				Main.NewText("");


			return rocket;
		}
	}
}
