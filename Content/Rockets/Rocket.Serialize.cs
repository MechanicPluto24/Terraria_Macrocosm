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
				[nameof(CurrentSubworld)] = CurrentSubworld,

				[nameof(Position)] = Position,

				[nameof(Fuel)] = Fuel,
				[nameof(FuelCapacity)] = FuelCapacity

			};

			if (InFlight) tag[nameof(InFlight)] = true;
			if (Landing) tag[nameof(Landing)] = true;

			if (TargetLandingPosition != Vector2.Zero) tag[nameof(TargetLandingPosition)] = TargetLandingPosition;

			foreach (string moduleName in ModuleNames)
				tag[moduleName] = Modules[moduleName];

			return tag;
		}

		public static Rocket DeserializeData(TagCompound tag)
		{
			// TODO: should add tag.ContainsKey checks for values that are not saved if default
			Rocket rocket = new()
			{
				WhoAmI = tag.GetInt(nameof(WhoAmI)),

				CurrentSubworld = tag.GetString(nameof(CurrentSubworld)),
				Active = tag.GetBool(nameof(Active)),

				Position = tag.Get<Vector2>(nameof(Position)),

				Fuel = tag.GetFloat(nameof(Fuel)),
				FuelCapacity = tag.GetFloat(nameof(FuelCapacity))
			};

			rocket.InFlight = tag.ContainsKey(nameof(InFlight));
			rocket.Landing = tag.ContainsKey(nameof(Landing));

			if (tag.ContainsKey(nameof(TargetLandingPosition)))
				rocket.TargetLandingPosition = tag.Get<Vector2>(nameof(TargetLandingPosition));

			foreach (string moduleName in rocket.ModuleNames)
			{
				Type moduleType = Type.GetType(moduleName);
				if (moduleType != null && moduleType.IsSubclassOf(typeof(RocketModule)))
				{
					var module = RocketModule.DeserializeData(tag.GetCompound(moduleName));
					rocket.Modules[moduleName] = module;
				}
			}

			return rocket;
		}
	}
}
