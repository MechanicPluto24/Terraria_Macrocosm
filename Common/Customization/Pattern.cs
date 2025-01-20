using Macrocosm.Common.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Content.Rockets.Customization
{
    public partial class Pattern
    {
        public string Name { get; set; }
        public string Context { get; set; }
        public string Profile { get; set; }
        public Dictionary<Color, PatternColorData> ColorData { get; set; } = new();

        public Asset<Texture2D> Texture { get; set; }
        private readonly string texturePath;
        public Asset<Texture2D> Icon { get; set; }
        private readonly string iconPath;

        public const int MaxColors = 64;
        public int ColorCount => Math.Min(ColorData.Count, MaxColors);

        private readonly Dictionary<Color, Color> staticColors;
        private static Asset<Effect> shader;

        public Pattern()
        {
            Name = "";
            Context = "";
            Profile = "";
            ColorData = new();
            texturePath = Macrocosm.EmptyTexPath;
            Texture = Macrocosm.EmptyTex;
            iconPath = Macrocosm.EmptyTexPath;
            Icon = Macrocosm.EmptyTex;
        }

        public Pattern(string name, string context, string profile, string texturePath, string iconPath, Dictionary<Color, PatternColorData> defaultColorData)
        {
            Name = name;
            Context = context;
            Profile = profile;

            ColorData = defaultColorData.OrderBy(kv => kv.Key.PackedValue).ToDictionary(kv => kv.Key, kv => kv.Value);
            staticColors = ColorData.Where(kvp => !kvp.Value.HasColorFunction).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Color);

            this.texturePath = texturePath;
            Texture = ModContent.RequestIfExists<Texture2D>(texturePath, out var t) ? t : Macrocosm.EmptyTex;

            this.iconPath = iconPath;
            Icon = ModContent.RequestIfExists<Texture2D>(iconPath, out var it) ? it : Macrocosm.EmptyTex;
        }

        public Pattern Clone() => new(Name, Context, Profile, texturePath, iconPath, new Dictionary<Color, PatternColorData>(ColorData));

        public Color GetColor(Color key)
        {
            if (ColorData.TryGetValue(key, out var data))
            {
                if (data.HasColorFunction)
                    return data.ColorFunction.Invoke(staticColors);

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

        public Effect GetEffect()
        {
            shader ??= ModContent.Request<Effect>(Macrocosm.ShadersPath + "ColorMaskShading", AssetRequestMode.ImmediateLoad);

            var effect = shader.Value;
            effect.Parameters["uColorCount"].SetValue(ColorCount);
            effect.Parameters["uColorKey"].SetValue(ColorData.Keys.Select(c => c.ToVector3()).ToArray());
            effect.Parameters["uColor"].SetValue(ColorData.Select(kvp => GetColor(kvp.Key).ToVector4()).ToArray());
            effect.Parameters["uSampleBrightness"].SetValue(true);
            return effect;
        }


        public override bool Equals(object obj)
        {
            if (obj is not Pattern pattern)
                return false;

            if (Name != pattern.Name || Context != pattern.Context || Profile != pattern.Profile)
                return false;

            return ColorData.SequenceEqual(pattern.ColorData);
        }

        public override int GetHashCode() => HashCode.Combine(Name, Context, Profile, ColorData);
    }
}
