using Macrocosm.Common.Customization;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Customization
{
    public readonly partial struct Pattern : TagSerializable
    {
        public string ToJSON(bool includeColorFunctions = false, bool includeNonModifiableColors = false) => ToJObject(includeColorFunctions, includeNonModifiableColors).ToString(Formatting.Indented);

        public JObject ToJObject(bool includeColorFunctions = true, bool includeNonModifiableColors = false)
        {
            JObject jObject = new()
            {
                ["patternName"] = Name,
                ["context"] = Context,
                ["texturePath"] = texture.Name,
                ["iconPath"] = iconTexture.Name
            };

            JArray colorDataArray = new();
            foreach (var colorData in ColorData)
            {
                JObject colorDataObject = new();

                if (colorData.HasColorFunction)
                {
                    if (includeColorFunctions)
                    {
                        colorDataObject["colorFunction"] = colorData.ColorFunction.Name;
                        colorDataObject["parameters"] = new JArray(colorData.ColorFunction.Parameters);
                    }
                }
                else
                {
                    if (includeNonModifiableColors || colorData.IsUserModifiable)
                    {
                        colorDataObject["color"] = colorData.Color.GetHex();

                        if (!colorData.IsUserModifiable)
                            colorDataObject["userModifiable"] = false;
                    }
                }

                colorDataArray.Add(colorDataObject);
            }

            if (colorDataArray.Any(c => c.HasValues))
                jObject["colorData"] = colorDataArray;

            return jObject;
        }


        public static Pattern FromJSON(string json) => FromJObject(JObject.Parse(json));

        public static Pattern FromJObject(JObject jObject)
        {
            string context = jObject.Value<string>("context");
            string name = jObject.Value<string>("patternName");
            string texturePath = jObject.Value<string>("texturePath");
            string iconPath = jObject.Value<string>("iconPath");

            if (string.IsNullOrEmpty(context) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(texturePath))
                throw new SerializationException("Pattern JSON is missing required fields: 'context', 'name', or 'texturePath'.");

            if (!PatternManager.TryGet(context, name, out Pattern pattern))
                throw new SerializationException($"Pattern '{name}' in context '{context}' is not recognized.");

            JArray colorDataArray = jObject.Value<JArray>("colorData");
            if (colorDataArray != null)
            {
                var colorDatas = colorDataArray.Select(ParseColorData).ToArray();
                return new Pattern(context, name, texturePath, iconPath, colorDatas);
            }

            return new Pattern(context, name, texturePath, iconPath);
        }

        private static PatternColorData ParseColorData(JToken colorDataToken)
        {
            if (colorDataToken is not JObject colorDataObject)
                return new PatternColorData(); 

            string colorHex = colorDataObject.Value<string>("color");
            if (!string.IsNullOrEmpty(colorHex) && Utility.TryGetColorFromHex(colorHex, out Color color))
            {
                bool userModifiable = colorDataObject.Value<bool?>("userModifiable") ?? true;
                return new PatternColorData(color, userModifiable);
            }

            string colorFunctionName = colorDataObject.Value<string>("colorFunction");
            if (!string.IsNullOrEmpty(colorFunctionName))
            {
                var parametersArray = colorDataObject["parameters"] as JArray;
                var parameters = parametersArray?.ToObjectRecursive<object>() ?? Array.Empty<object>();

                return new PatternColorData(ColorFunction.CreateByName(colorFunctionName, parameters));
            }

            return new PatternColorData();
        }


        public static readonly Func<TagCompound, Pattern> DESERIALIZER = DeserializeData;

        public TagCompound SerializeData()
        {
            TagCompound tag = new()
            {
                [nameof(Name)] = Name,
                [nameof(Context)] = Context,
                [nameof(ColorData)] = ColorData.ToList()
            };

            return tag;
        }

        public static Pattern DeserializeData(TagCompound tag)
        {
            string name = "";
            string moduleName = "";
            Pattern pattern;

            try
            {
                if (tag.ContainsKey(nameof(Name)))
                    name = tag.GetString(nameof(Name));

                if (tag.ContainsKey(nameof(Context)))
                    moduleName = tag.GetString(nameof(Context));

                pattern = PatternManager.Get(moduleName, name);

                if (tag.ContainsKey(nameof(ColorData)))
                    pattern = pattern.WithColorData(tag.GetList<PatternColorData>(nameof(ColorData)).ToArray());

                return pattern;
            }
            catch
            {
                string message = "Failed to deserialize pattern:\n";
                message += "\tName: " + (string.IsNullOrEmpty(name) ? "Unknown" : name) + ", ";
                message += "\tModule: " + (string.IsNullOrEmpty(moduleName) ? "Unknown" : moduleName);
                throw new SerializationException(message);
            }
        }

    }
}
