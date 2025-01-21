using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Customization
{
    public readonly struct PatternColorData : TagSerializable
    {
        public readonly bool IsUserModifiable { get; }
        public readonly Color Color { get; }
        public readonly ColorFunction ColorFunction { get; }

        public bool HasColorFunction => ColorFunction != null && !string.IsNullOrEmpty(ColorFunction.Name);

        public PatternColorData()
        {
            IsUserModifiable = false;
            Color = Color.Transparent;
            ColorFunction = null;
        }

        public PatternColorData(Color color, bool isUserModifiable = true)
        {
            IsUserModifiable = isUserModifiable;
            Color = color;
            ColorFunction = null;
        }

        public PatternColorData(ColorFunction colorFunction)
        {
            IsUserModifiable = false;
            Color = Color.Transparent;
            ColorFunction = colorFunction;
        }

        public PatternColorData WithUserColor(Color newUserColor)
        {
            if (!IsUserModifiable || HasColorFunction)
                return this;

            return new PatternColorData(newUserColor);
        }

        public PatternColorData WithColorFunction(ColorFunction function)
        {
            return new PatternColorData(function);
        }

        public string ToJSON() => ToJObject().ToString(Formatting.Indented);
        public JObject ToJObject()
        {
            JObject jObject = new()
            {
                ["color"] = Color.GetHex(),
                ["isUserModifiable"] = IsUserModifiable
            };

            if (HasColorFunction)
            {
                jObject["colorFunction"] = new JObject()
                {
                    ["name"] = ColorFunction.Name,
                    ["parameters"] = new JArray(ColorFunction.UnparseParameters(ColorFunction.Parameters))
                };
            }

            return jObject;
        }

        public static PatternColorData FromJSON(string json) => FromJObject(JObject.Parse(json));
        public static PatternColorData FromJObject(JObject jObject)
        {
            // Special color function, depends on other colors in the pattern, not user modifiable
            if (jObject["colorFunction"] is JObject functionObject)
            {
                string name = functionObject["name"]?.Value<string>() ?? throw new ArgumentException("Missing 'name' field.");
                object[] parameters = functionObject["parameters"]?.ToObject<List<object>>()?.ToArray() ?? throw new ArgumentException("Missing 'parameters' field.");
                var colorFunction = ColorFunction.CreateByName(name, parameters); 
                return new PatternColorData(colorFunction);
            }

            // Whether the color data is modifiable or not, defaults to false
            bool isUserModifiable = jObject["isUserModifiable"]?.Value<bool>() ?? false;

            // Parse color value
            string colorHex = jObject["color"]?.Value<string>() ?? throw new ArgumentException("Missing color field.");
            if (!Utility.TryGetColorFromHex(colorHex, out Color color))
                throw new ArgumentException($"Invalid color: {colorHex}");

            return new PatternColorData(color, isUserModifiable);
        }

        public TagCompound SerializeData()
        {
            return new()
            {
                ["JSON"] = ToJSON()
            }; ;
        }


        public static readonly Func<TagCompound, PatternColorData> DESERIALIZER = DeserializeData;

        public static PatternColorData DeserializeData(TagCompound tag)
        {
            if (tag.ContainsKey("JSON"))
                return FromJSON(tag.GetString("JSON"));

            return new();
        }
    }
}
