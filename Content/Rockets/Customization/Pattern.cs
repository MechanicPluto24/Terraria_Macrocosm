using System;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Customization
{
    public class Pattern : TagSerializable
    {
		public string Name { get; }
		public string ModuleName { get; }

		public bool Unlocked { get; set; } = true;

		public int ColorCount { get; }

		private PatternColorData[] colorData;
		private Color[] defaultColors;
		private Color[] colors;

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
				if (ModContent.RequestIfExists(TexturePath, out Asset<Texture2D> paintMask))
					return paintMask.Value;
				else
					return Macrocosm.EmptyTex;
			}
		}

		public Pattern(string moduleName, string patternName, bool unlocked, params PatternColorData[] colorData)
		{
			ModuleName = moduleName;
			Name = patternName;
			Unlocked = unlocked;

			ColorCount = (int)MathHelper.Clamp(colorData.Length, 0, MaxColorCount);

			defaultColors = new Color[MaxColorCount];
			Array.Fill(defaultColors, Color.Transparent);

			this.colorData = new PatternColorData[MaxColorCount];
			for (int i = 0; i < ColorCount; i++)
				this.colorData[i] = colorData[i];

			for (int i = 0; i < ColorCount; i++)
			{
				if (!this.colorData[i].IsUserChangeable)
					defaultColors[i] = this.colorData[i].DefaultColor;
				else if (this.colorData[i].ColorFunction != null)
					defaultColors[i] = this.colorData[i].ColorFunction(defaultColors);
			}

			colors = defaultColors;
		}

		public Color GetColor(int index)
		{
			if (index >= 0 && index < ColorCount)
			{
				if (!colorData[index].IsUserChangeable)
					return colorData[index].DefaultColor;
				else if (colorData[index].ColorFunction != null)
				{
					Color[] tempColors = new Color[ColorCount];
					Array.Copy(colors, tempColors, ColorCount);
					return colorData[index].ColorFunction(tempColors);
				}
				else 
					return colors[index];
			}
			return Color.Transparent;
		}

		public bool TrySetColor(int index, Color color)
		{
			if (index < 0 || index >= ColorCount || colorData[index].IsUserChangeable || colorData[index].ColorFunction is not null)
				return false;

			colors[index] = color;
			return true;
		}

		public Color GetDefaultColor(int index)
		{
			if (index >= 0 && index < ColorCount)
			{
				if (colorData[index].ColorFunction != null)
				{
					Color[] tempColors = new Color[ColorCount];
					Array.Copy(colors, tempColors, ColorCount);
					return colorData[index].ColorFunction(tempColors);
				}
				else
					return colorData[index].DefaultColor;
			}
			return Color.Transparent;
		}

		//public void DrawIconTexture(Vector2 position)
		//{
		//
		//}

		/// <summary> Color mask keys </summary>
		public static readonly Vector3[] ColorKeys = {
			new Vector3(0f, 1f, 1f),     // Cyan (Rocket tip, booster tips, etc.)
			new Vector3(1f, 0f, 1f),     // Magenta (The "background" of the pattern)
			new Vector3(1f, 1f, 0f),     // Yellow  
			new Vector3(0f, 1f, 0f),     // Green   
			new Vector3(1f, 0f, 0f),     // Red
			new Vector3(0f, 0f, 1f),     // Blue   
			new Vector3(1f,.5f, 0f),     // Orange
			new Vector3(0f,.5f, 1f)      // Azure
		};

		public static readonly Func<TagCompound, Pattern> DESERIALIZER = DeserializeData;

		public TagCompound SerializeData()
		{
			TagCompound tag = new()
			{
				["Name"] = Name,
				["ModuleName"] = ModuleName,
				["Unlocked"] = Unlocked
			};

			// Save the user-changed colors
			for (int i = 0; i < ColorCount; i++)
 				if (colorData[i].IsUserChangeable && colorData[i].ColorFunction is null)
 					tag[$"Color{i}"] = colors[i];  
 
			return tag;
		}

		public static Pattern DeserializeData(TagCompound tag)
		{
			string name = tag.GetString("Name");
			string moduleName = tag.GetString("ModuleName");

			Pattern pattern = CustomizationStorage.GetPattern(moduleName, name);
			bool unlocked = tag.GetBool("Unlocked");
			pattern.Unlocked = unlocked;
		
			// Load the user-changed colors
			for (int i = 0; i < pattern.ColorCount; i++)
				if (pattern.colorData[i].IsUserChangeable && pattern.colorData[i].ColorFunction is null)
					pattern.colors[i] = tag.Get<Color>($"Color{i}");

			return pattern;
		}
	}
}
