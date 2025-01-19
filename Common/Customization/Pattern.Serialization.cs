using Macrocosm.Common.Customization;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
                ["colorData"] = new JArray(ColorData.Select(kvp =>
                {
                    return new JObject
                    {
                        ["key"] = kvp.Key.GetHex(),
                        ["value"] = kvp.Value.ToJObject()
                    };
                }))
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

                var colorData = jObject["colorData"]?.OfType<JObject>().Select(entry =>
                {
                    string keyHex = entry["key"]?.Value<string>() ?? throw new ArgumentException("Missing 'key' in colorData.");
                    if (!Utility.TryGetColorFromHex(keyHex, out Color key))
                        throw new ArgumentException($"Invalid color key: {keyHex}");

                    var value = PatternColorData.FromJObject(entry["value"] as JObject);
                    return new KeyValuePair<Color, PatternColorData>(key, value);

                }).ToDictionary() ?? new Dictionary<Color, PatternColorData>();

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
                ["Name"] = Name,
                ["Context"] = Context,
                ["Profile"] = Profile,
                ["TexturePath"] = texturePath,
                ["IconPath"] = iconPath,
                ["ColorData"] = ColorData.Select(kvp => new TagCompound
                {
                    ["Key"] = kvp.Key.PackedValue,
                    ["Value"] = kvp.Value
                }).ToList()
            };
        }

        public static Pattern DeserializeData(TagCompound tag)
        {
            try
            {
                string name = tag.GetString("Name");
                string context = tag.GetString("Context");
                string profile = tag.GetString("Profile");
                string texturePath = tag.GetString("TexturePath");
                string iconPath = tag.GetString("IconPath");

                var colorData = tag.GetList<TagCompound>("ColorData")
                    .ToDictionary(
                        entry => new Color { PackedValue = entry.Get<uint>("Key") },
                        entry => entry.Get<PatternColorData>("Value")
                    );

                return new Pattern(name, context, profile, texturePath, iconPath, colorData);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Error deserializing Pattern from TagCompound.", ex);
            }
        }
    }
}
