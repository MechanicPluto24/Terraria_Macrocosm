using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Customization
{
    public partial class Pattern : TagSerializable
    {
        public string ToJSON() => ToJObject().ToString(Formatting.Indented);
        public JObject ToJObject()
        {
            return new JObject
            {
                ["name"] = Name,
                ["context"] = Context,
                ["profile"] = Profile,
                ["texturePath"] = texturePath,
                ["iconPath"] = iconPath,
                ["colorData"] = new JObject(ColorData.Select(kvp => new JProperty(kvp.Key.GetHex(), kvp.Value.ToJObject())))
            };
        }

        public static Pattern FromJSON(string json) => FromJObject(JObject.Parse(json));
        public static Pattern FromJObject(JObject jObject)
        {
            try
            {
                string name = jObject["name"]?.Value<string>() ?? throw new ArgumentException("Missing 'name' field.");
                string context = jObject["context"]?.Value<string>() ?? throw new ArgumentException("Missing 'context' field.");
                string profile = jObject["profile"]?.Value<string>() ?? throw new ArgumentException("Missing 'profile' field.");
                string texturePath = jObject["texturePath"]?.Value<string>();
                string iconPath = jObject["iconPath"]?.Value<string>();

                // Parse colorData as JObject
                var colorDataJson = jObject["colorData"] as JObject ?? new JObject();
                var colorData = colorDataJson.Properties()
                    .Where(entry => Utility.TryGetColorFromHex(entry.Name, out var key))
                    .Select(entry => new KeyValuePair<Color, PatternColorData>(Utility.GetColorFromHex(entry.Name), PatternColorData.FromJObject(entry.Value as JObject)))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                return new Pattern(name, context, profile, texturePath, iconPath, colorData);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Error deserializing Pattern from JObject.", ex);
            }
        }


        public static readonly Func<TagCompound, Pattern> DESERIALIZER = DeserializeData;

        public TagCompound SerializeData()
        {
            return new TagCompound
            {
                ["JSON"] = ToJSON()
            };
        }

        public static Pattern DeserializeData(TagCompound tag)
        {
            if (tag.ContainsKey("JSON"))
                return FromJSON(tag.GetString("JSON"));

            return new();
        }
    }
}
