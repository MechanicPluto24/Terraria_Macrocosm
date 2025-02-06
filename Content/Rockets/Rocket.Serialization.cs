using Macrocosm.Common.Customization;
using Macrocosm.Common.Storage;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
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

            if(Modules != null) tag[nameof(Modules)] = Modules;

            if(Active) tag[nameof(Active)] = true;
            if(WhoAmI != -1) tag[nameof(WhoAmI)] = WhoAmI;
            if(CurrentWorld != "") tag[nameof(CurrentWorld)] = CurrentWorld;

            if(State != ActionState.Idle) tag[nameof(State)] = (int)State;
            if(Position != default) tag[nameof(Position)] = Position;

            if(OrbitTravel) tag[nameof(OrbitTravel)] = true;
            if(TargetTravelPosition != default) tag[nameof(TargetTravelPosition)] = TargetTravelPosition;

            if(Fuel != 0f) tag[nameof(Fuel)] = Fuel;
            if(FuelCapacity != DefaultFuelCapacity) tag[nameof(FuelCapacity)] = FuelCapacity;

            if(Nameplate != null) tag[nameof(Nameplate)] = Nameplate;
            if(Inventory != null) tag[nameof(Inventory)] = Inventory;

            return tag;
        }

        public static Rocket DeserializeData(TagCompound tag)
        {
            RocketModule[] modules = tag.ContainsKey(nameof(Modules))
                ? tag.GetList<TagCompound>(nameof(Modules)).Select(RocketModule.DeserializeData).ToArray()
                : RocketModule.DefaultLegacyModules.ToArray();

            Rocket rocket = new(modules)
            {
                Active = tag.ContainsKey(nameof(Active)),
                WhoAmI = tag.TryGet(nameof(WhoAmI), out int whoAmI) ? whoAmI : -1,
                CurrentWorld = tag.TryGet(nameof(CurrentWorld), out string world) ? world : "",

                State = tag.TryGet(nameof(State), out int state) ? (ActionState)state : ActionState.Idle,
                Position = tag.TryGet(nameof(Position), out Vector2 position) ? position : Vector2.Zero,

                OrbitTravel = tag.ContainsKey(nameof(OrbitTravel)),
                TargetTravelPosition = tag.TryGet(nameof(TargetTravelPosition), out Vector2 target) ? target : Vector2.Zero,

                Fuel = tag.TryGet(nameof(Fuel), out float fuel) ? fuel : 0f,
                FuelCapacity = tag.TryGet(nameof(FuelCapacity), out float fuelCapacity) ? fuelCapacity : DefaultFuelCapacity,
            };

            if (tag.ContainsKey(nameof(Nameplate)))
                rocket.Nameplate = tag.Get<Nameplate>(nameof(Nameplate));

            if (tag.ContainsKey(nameof(Inventory))) 
                rocket.Inventory = tag.Get<Inventory>(nameof(Inventory));

            return rocket;
        }
    }
}
