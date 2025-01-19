using Macrocosm.Common.Customization;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Customization
{
    public readonly partial struct PatternColorData : TagSerializable
    {
        public JObject ToJObject()
        {
            JObject jObject = new()
            {
                ["color"] = Color.GetHex(),
                ["isUserModifiable"] = IsUserModifiable
            };

            if (HasColorFunction)
            {
                jObject["colorFunction"] = ColorFunction.Name;
                jObject["parameters"] = new JArray(ColorFunction.Parameters);
            }

            return jObject;
        }

        public static PatternColorData FromJObject(JObject jObject)
        {
            string colorHex = jObject["color"]?.Value<string>() ?? throw new ArgumentException("Missing color field.");
            if (!Utility.TryGetColorFromHex(colorHex, out Color color))
                throw new ArgumentException($"Invalid color: {colorHex}");

            bool isUserModifiable = jObject["isUserModifiable"]?.Value<bool>() ?? false;

            if (jObject["colorFunction"] != null)
            {
                string functionName = jObject["colorFunction"].Value<string>();
                var parameters = jObject["parameters"]?.ToObject<List<object>>() ?? new List<object>();
                var colorFunction = ColorFunction.CreateByName(functionName, parameters.ToArray());
                return new PatternColorData(colorFunction);
            }

            return new PatternColorData(color, isUserModifiable);
        }


        public TagCompound SerializeData()
        {
            TagCompound tag = new()
            {
                ["Color"] = Color.PackedValue,
                ["IsUserModifiable"] = IsUserModifiable,
                ["HasColorFunction"] = HasColorFunction
            };

            if (HasColorFunction)
            {
                tag["ColorFunction"] = ColorFunction.Name;
                tag["FunctionParams"] = ColorFunction.Parameters.ToList();
            }

            return tag;
        }


        public static readonly Func<TagCompound, PatternColorData> DESERIALIZER = DeserializeData;

        public static PatternColorData DeserializeData(TagCompound tag)
        {
            if (tag.Get<bool>("HasColorFunction"))
            {
                string functionName = tag.GetString("ColorFunction");
                var parameters = tag.GetList<object>("FunctionParams");
                var colorFunction = ColorFunction.CreateByName(functionName, parameters.ToArray());
                return new PatternColorData(colorFunction);
            }

            Color color = new() { PackedValue = tag.Get<uint>("Color") };
            bool isUserModifiable = tag.Get<bool>("IsUserModifiable");
            return new PatternColorData(color, isUserModifiable);
        }

    }
}
