using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.Modules;
using Macrocosm.Content.Rockets.Storage;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket : TagSerializable
    {
        public Rocket Clone() => DeserializeData(SerializeData());

        public static readonly Func<TagCompound, Rocket> DESERIALIZER = DeserializeData;

        public TagCompound SerializeData()
        {
            TagCompound tag = new();

            if (Active)
            {
                tag[nameof(Active)] = true;
                tag[nameof(WhoAmI)] = WhoAmI;

                tag[nameof(CurrentWorld)] = CurrentWorld;
                tag[nameof(Position)] = Position;
                tag[nameof(worldExitSpeed)] = worldExitSpeed;
                tag[nameof(Fuel)] = Fuel;
                tag[nameof(FuelCapacity)] = FuelCapacity;

                if (HasInventory)
                    tag[nameof(Inventory)] = Inventory;

                if (Launched) tag[nameof(Launched)] = true;
                if (Landing) tag[nameof(Landing)] = true;

                if (TargetLandingPosition != Vector2.Zero) tag[nameof(TargetLandingPosition)] = TargetLandingPosition;

				tag[nameof(Nameplate)] = Nameplate;

				foreach (string moduleName in ModuleNames)
				{
					tag[moduleName] = Modules[moduleName];
					tag[moduleName + "_Type"] = Modules[moduleName].FullName;
				}
			}

            return tag;
        }

        public static Rocket DeserializeData(TagCompound tag)
        {
            Rocket rocket = new();
            rocket.Active = tag.ContainsKey(nameof(Active));
            if (rocket.Active)
            {
                if (tag.ContainsKey(nameof(WhoAmI)))
                    rocket.WhoAmI = tag.GetInt(nameof(WhoAmI));

                if (tag.ContainsKey(nameof(CurrentWorld)))
                    rocket.CurrentWorld = tag.GetString(nameof(CurrentWorld));

                if (tag.ContainsKey(nameof(Position)))
                    rocket.Position = tag.Get<Vector2>(nameof(Position));

                if (tag.ContainsKey(nameof(worldExitSpeed)))
                    rocket.worldExitSpeed = tag.GetFloat(nameof(worldExitSpeed));

                if (tag.ContainsKey(nameof(Fuel)))
                    rocket.Fuel = tag.GetFloat(nameof(Fuel));

                if (tag.ContainsKey(nameof(FuelCapacity)))
                    rocket.FuelCapacity = tag.GetFloat(nameof(FuelCapacity));

                if (tag.ContainsKey(nameof(Inventory)))
                    rocket.Inventory = tag.Get<Inventory>(nameof(Inventory));

                rocket.Launched = tag.ContainsKey(nameof(Launched));
                rocket.Landing = tag.ContainsKey(nameof(Landing));

                if (tag.ContainsKey(nameof(TargetLandingPosition)))
                    rocket.TargetLandingPosition = tag.Get<Vector2>(nameof(TargetLandingPosition));

				if (tag.ContainsKey(nameof(Nameplate)))
					rocket.Nameplate = tag.Get<Nameplate>(nameof(Nameplate));

				foreach (string moduleName in rocket.ModuleNames)
				{
					// This mess is just so each module can save their own data
					if (tag.ContainsKey(moduleName + "_Type"))
					{
						Type moduleType = Type.GetType(tag.GetString(moduleName + "_Type"));
						if (moduleType != null && moduleType.IsSubclassOf(typeof(RocketModule)))
						{
							var module = RocketModule.DeserializeData(tag.GetCompound(moduleName), rocket);
							rocket.Modules[moduleName] = module;
						}
					}
				}
			};

            return rocket;
        }
    }
}
