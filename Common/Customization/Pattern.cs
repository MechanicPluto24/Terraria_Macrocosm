using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Macrocosm.Common.Customization
{
    public class Pattern : TagSerializable
    {
        public string Name { get; set; }
        public string Context { get; set; }
        public Dictionary<Color, PatternColorData> ColorData { get; set; } = new();

        public Asset<Texture2D> Texture { get; set; }
        private readonly string texturePath;

        public const int MaxColors = 64;
        public const float ColorDistance = 0.1f;
        public int ColorCount => Math.Min(ColorData.Count, MaxColors);

        public Pattern()
        {
            Name = "";
            Context = "";
            ColorData = new();
            texturePath = Macrocosm.EmptyTexPath;
            Texture = Macrocosm.EmptyTex;
        }

        public Pattern(string name, string context, string texturePath, Dictionary<Color, PatternColorData> defaultColorData)
        {
            Name = name;
            Context = context;

            ColorData = defaultColorData.OrderBy(kvp => kvp.Key.PackedValue).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            this.texturePath = texturePath;
            Texture = ModContent.RequestIfExists<Texture2D>(texturePath, out var t) ? t : Macrocosm.EmptyTex;
        }

        public Pattern Clone() => new(Name, Context, texturePath, new Dictionary<Color, PatternColorData>(ColorData));
        public Color GetColor(Color key)
        {
            if (ColorData.TryGetValue(key, out var data))
            {
                if (data.HasColorFunction)
                    return data.ColorFunction.Invoke(ColorData.Where(kvp => !kvp.Value.HasColorFunction).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Color));

                return data.Color;
            }

            return Color.Transparent;
        }


        public void SetColor(Color key, Color color, bool evenIfNotUserModifiable = false)
        {
            if (ColorData.TryGetValue(key, out var data) && (evenIfNotUserModifiable || data.IsUserModifiable))
                ColorData[key] = data.WithUserColor(color);
        }

        public void SetColorFunction(Color key, ColorFunction colorFunction)
        {
            if (ColorData.TryGetValue(key, out PatternColorData value))
            {
                var data = value;
                if (data.IsUserModifiable)
                {
                    var updatedData = new Dictionary<Color, PatternColorData>(ColorData)
                    {
                        [key] = data.WithColorFunction(colorFunction)
                    };
                    ColorData = updatedData;
                }
            }
        }

        public Effect PrepareEffect()
        {
            var effect = Macrocosm.GetShader("ColorMaskShading");
            effect.Parameters["uColorCount"].SetValue(ColorCount);
            effect.Parameters["uColorKey"].SetValue(ColorData.Keys.Select(c => c.ToVector3()).ToArray());
            effect.Parameters["uColor"].SetValue(ColorData.Select(kvp => GetColor(kvp.Key).ToVector4()).ToArray());
            effect.Parameters["uColorDistance"].SetValue(ColorDistance);
            return effect;
        }

        /// <summary> Set the <see cref="SpriteBatch"/> to <see cref="SpriteSortMode.Immediate"/> and <see cref="SamplerState.PointClamp"/> before applying </summary>
        public void Apply(int index = 1)
        {
            Main.graphics.GraphicsDevice.Textures[index] = Texture.Value;
            Main.graphics.GraphicsDevice.SamplerStates[index] = Main.graphics.GraphicsDevice.SamplerStates[0];

            Effect effect = PrepareEffect();
            foreach (var pass in effect.CurrentTechnique.Passes)
                pass.Apply();
        }

        public override bool Equals(object obj)
        {
            if (obj is not Pattern pattern)
                return false;

            if (Name != pattern.Name || Context != pattern.Context)
                return false;

            return ColorData.SequenceEqual(pattern.ColorData);
        }

        public override int GetHashCode() => HashCode.Combine(Name, Context, ColorData);

        public string ToJSON() => ToJObject().ToString(Formatting.Indented);
        public JObject ToJObject()
        {
            return new JObject
            {
                ["name"] = Name,
                ["context"] = Context,
                ["texturePath"] = texturePath,
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
                string texturePath = jObject["texturePath"]?.Value<string>();

                Dictionary<Color, PatternColorData> colorData = new();
                if (jObject["colorData"] is JObject colorDataJson)
                    foreach (var entry in colorDataJson.Properties())
                        if (Utility.TryGetColorFromHex(entry.Name, out Color key) && entry.Value is JObject value && value != null)
                            colorData[key] = PatternColorData.FromJObject(value);

                return new Pattern(name, context, texturePath, colorData);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Error deserializing Pattern from JObject.", ex);
            }
        }

        public TagCompound SerializeData()
        {
            return new TagCompound
            {
                ["JSON"] = ToJSON()
            };
        }

        public static readonly Func<TagCompound, Pattern> DESERIALIZER = DeserializeData;
        public static Pattern DeserializeData(TagCompound tag)
        {
            if (tag.ContainsKey("JSON"))
                return FromJSON(tag.GetString("JSON"));

            return new();
        }
    }
}
