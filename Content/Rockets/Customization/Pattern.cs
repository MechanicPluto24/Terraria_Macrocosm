using Macrocosm.Content.Rockets.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Customization
{
	public readonly partial struct Pattern
    {
		public readonly string Name { get; }

		public readonly string ModuleName { get; }

		public bool IsDefault => Name == "Basic";

		public readonly ImmutableArray<PatternColorData> ColorData { get; init; }
		public readonly List<int> UserModifiableIndexes { get; init; } = new();

		public int UserModifiableColorCount => UserModifiableIndexes.Count;

        public const int MaxColorCount = 8;

        public string TexturePath => GetType().Namespace.Replace('.', '/') + "/Patterns/" + ModuleName + "/" + Name;
        public Texture2D Texture
        {
            get
            {
                if (ModContent.RequestIfExists(TexturePath, out Asset<Texture2D> paintMask))
                    return paintMask.Value;
                else
                    return Macrocosm.EmptyTex;
            }
        }

        public string IconTexturePath => GetType().Namespace.Replace('.', '/') + "/Patterns/Icons/" + Name;
        public Texture2D IconTexture
        {
            get
            {
                if (ModContent.RequestIfExists(IconTexturePath, out Asset<Texture2D> paintMask))
                    return paintMask.Value;
                else
                    return Macrocosm.EmptyTex;
            }
        }

        public Pattern(string moduleName, string patternName, params PatternColorData[] defaultColorData)
        {
            ModuleName = moduleName;
            Name = patternName;

            var colorData = new PatternColorData[MaxColorCount];
            Array.Fill(colorData, new PatternColorData());

            for (int i = 0; i < defaultColorData.Length; i++)
            {
                if (defaultColorData[i].HasColorFunction)
                {
                    colorData[i] = new PatternColorData(defaultColorData[i].ColorFunction);
                }
                else
                {
                    colorData[i] = new PatternColorData(defaultColorData[i].Color, defaultColorData[i].IsUserModifiable);

                    if (defaultColorData[i].IsUserModifiable)
                    {
                        UserModifiableIndexes.Add(i);
                    }
                }
            }

            ColorData = ImmutableArray.Create(colorData);
        }


		public Color GetColor(int index)
		{
			if (index >= 0 && index < MaxColorCount)
			{
				if (ColorData[index].HasColorFunction)
				{
					var copy = this;
					Color[] otherColors = ColorData.Select((c, i) => (i == index || c.HasColorFunction) ? Color.Transparent : copy.GetColor(i)).ToArray();
					return ColorData[index].ColorFunction.Invoke(otherColors);
				}
 				else
				{
					return ColorData[index].Color;
				}
			}
			return Color.Transparent;
		}

        public Pattern WithColor(int index, Color color, bool evenIfNotUserModifiable = false)
        {
            if (index < 0 || index >= MaxColorCount || (!evenIfNotUserModifiable && !ColorData[index].IsUserModifiable))
                return this;

            var updatedColorData = ColorData.ToArray();  
            updatedColorData[index] = updatedColorData[index].WithUserColor(color);
            return this with { ColorData = ImmutableArray.Create(updatedColorData) };  
        }

        public Pattern WithColorFunction(int index, ColorFunction colorFunction)
        {
            if (index < 0 || index >= MaxColorCount || !ColorData[index].IsUserModifiable)
                return this;

            var updatedColorData = ColorData.ToArray(); 
            updatedColorData[index] = updatedColorData[index].WithColorFunction(colorFunction);
            return this with { ColorData = ImmutableArray.Create(updatedColorData) }; 
        }

        public Pattern WithColorData(ImmutableArray<PatternColorData> colorData)
        {
            return this with { ColorData = colorData };
        }

        public Pattern WithColorData(params PatternColorData[] colorData)
        {
            var updatedColorData = ColorData.ToArray();  

            for (int i = 0; i < colorData.Length; i++)
            {
                if (colorData[i].HasColorFunction)
                    updatedColorData[i] = ColorData[i].WithColorFunction(colorData[i].ColorFunction);
                else
                    updatedColorData[i] = ColorData[i].WithUserColor(colorData[i].Color);
            }

            return this with { ColorData = ImmutableArray.Create(updatedColorData) };  
        }

        public UIPatternIcon ProvideUI()
		{
			UIPatternIcon icon = new(this);
			return icon;
		}

		public override bool Equals(object obj)
		{
			return obj is Pattern pattern &&
				   Name == pattern.Name &&
				   ModuleName == pattern.ModuleName &&
				   ColorData.SequenceEqual(pattern.ColorData);
        }

        public static bool operator ==(Pattern left, Pattern right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Pattern left, Pattern right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, ModuleName, ColorData);
		}

		/// <summary> Color mask keys </summary>
		public static Vector3[] ColorKeys { get; } = 
		{
			new Vector3(0f, 1f, 1f),     // Cyan (Rocket tip, booster tips, etc.)
			new Vector3(1f, 0f, 1f),     // Magenta (The "background" of the pattern)
			new Vector3(1f, 1f, 0f),     // Yellow  
			new Vector3(0f, 1f, 0f),     // Green   
			new Vector3(1f, 0f, 0f),     // Red
			new Vector3(0f, 0f, 1f),     // Blue   
			new Vector3(1f,.5f, 0f),     // Orange
			new Vector3(0f,.5f, 1f)      // Azure
		};
    }
}
