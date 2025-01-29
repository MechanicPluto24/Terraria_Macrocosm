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
            TagCompound tag = new()
            {
                [nameof(Active)] = Active,
                [nameof(Modules)] = Modules,
                [nameof(WhoAmI)] = WhoAmI,
                [nameof(CurrentWorld)] = CurrentWorld,

                [nameof(State)] = (int)State,
                [nameof(Position)] = Position,
                [nameof(OrbitTravel)] = OrbitTravel,

                [nameof(Inventory)] = Inventory,

                [nameof(Fuel)] = Fuel,
                [nameof(FuelCapacity)] = FuelCapacity,

                [nameof(Nameplate)] = Nameplate
            };

            if (TargetTravelPosition != Vector2.Zero) tag[nameof(TargetTravelPosition)] = TargetTravelPosition;

            return tag;
        }
        public static Rocket DeserializeData(TagCompound tag)
        {
            RocketModule[] modules = tag.ContainsKey(nameof(Modules))
                ? tag.GetList<TagCompound>(nameof(Modules)).Select(RocketModule.DeserializeData).ToArray()
                : RocketModule.DefaultLegacyModules.ToArray();

            Rocket rocket = new(modules) 
            { 
                Active = tag.ContainsKey(nameof(Active)) 
            };

            if (tag.ContainsKey(nameof(WhoAmI)))  rocket.WhoAmI = tag.GetInt(nameof(WhoAmI));
            if (tag.ContainsKey(nameof(CurrentWorld))) rocket.CurrentWorld = tag.GetString(nameof(CurrentWorld));

            if (tag.ContainsKey(nameof(State))) rocket.State = (ActionState)tag.GetInt(nameof(State));
            if (tag.ContainsKey(nameof(Position))) rocket.Position = tag.Get<Vector2>(nameof(Position));
            rocket.OrbitTravel = tag.ContainsKey(nameof(OrbitTravel));

            if (tag.ContainsKey(nameof(Inventory))) rocket.Inventory = tag.Get<Inventory>(nameof(Inventory));

            if (tag.ContainsKey(nameof(Fuel)))  rocket.Fuel = tag.GetFloat(nameof(Fuel));
            if (tag.ContainsKey(nameof(FuelCapacity)))  rocket.FuelCapacity = tag.GetFloat(nameof(FuelCapacity));

            if (tag.ContainsKey(nameof(Nameplate))) rocket.Nameplate = tag.Get<Nameplate>(nameof(Nameplate));

            if (tag.ContainsKey(nameof(TargetTravelPosition))) rocket.TargetTravelPosition = tag.Get<Vector2>(nameof(TargetTravelPosition));

            return rocket;
        }
    }
}
