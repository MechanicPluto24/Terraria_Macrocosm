using Macrocosm.Common.Customization;
using Macrocosm.Common.Storage;
using Macrocosm.Content.Rockets.Modules;
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

                tag[nameof(State)] = (int)State;

                tag[nameof(CurrentWorld)] = CurrentWorld;
                tag[nameof(Position)] = Position;
                tag[nameof(Fuel)] = Fuel;
                tag[nameof(FuelCapacity)] = FuelCapacity;

                tag[nameof(Inventory)] = Inventory;

                if (TargetTravelPosition != Vector2.Zero)
                    tag[nameof(TargetTravelPosition)] = TargetTravelPosition;

                tag[nameof(OrbitTravel)] = OrbitTravel;

                tag[nameof(Nameplate)] = Nameplate;

                foreach (var module in Modules)
                {
                    tag[module.Name] = module;
                    tag[module.Name + "_Type"] = module.FullName;
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

                if (tag.ContainsKey(nameof(State)))
                    rocket.State = (ActionState)tag.GetInt(nameof(State));

                if (tag.ContainsKey(nameof(Position)))
                    rocket.Position = tag.Get<Vector2>(nameof(Position));

                if (tag.ContainsKey(nameof(Fuel)))
                    rocket.Fuel = tag.GetFloat(nameof(Fuel));

                if (tag.ContainsKey(nameof(FuelCapacity)))
                    rocket.FuelCapacity = tag.GetFloat(nameof(FuelCapacity));

                if (tag.ContainsKey(nameof(Inventory)))
                    rocket.Inventory = tag.Get<Inventory>(nameof(Inventory));

                if (tag.ContainsKey(nameof(TargetTravelPosition)))
                    rocket.TargetTravelPosition = tag.Get<Vector2>(nameof(TargetTravelPosition));

                if (tag.ContainsKey(nameof(OrbitTravel)))
                    rocket.OrbitTravel = tag.GetBool(nameof(OrbitTravel));

                if (tag.ContainsKey(nameof(Nameplate)))
                    rocket.Nameplate = tag.Get<Nameplate>(nameof(Nameplate));

                foreach (string moduleName in ModuleTemplateNames)
                {
                    // This mess is just so each module can save their own data
                    if (tag.ContainsKey(moduleName + "_Type"))
                    {
                        Type moduleType = Type.GetType(tag.GetString(moduleName + "_Type"));
                        if (moduleType != null && moduleType.IsSubclassOf(typeof(RocketModule)))
                        {
                            var module = RocketModule.DeserializeData(tag.GetCompound(moduleName), rocket);
                            rocket.Modules[module.Slot] = module;
                        }
                    }
                }
            };

            return rocket;
        }
    }
}
