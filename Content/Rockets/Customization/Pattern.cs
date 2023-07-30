using System;
using System.Linq;
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

		public bool Unlocked { get; set; }
		public bool UnlockedByDefault { get; private set; }

		public int ColorCount { get; }
		public const int MaxColorCount = 8;

		private PatternColorData[] colorData;

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

		public Pattern(string moduleName, string patternName, bool unlockedByDefault, params PatternColorData[] colorData)
		{
			ModuleName = moduleName;
			Name = patternName;

			UnlockedByDefault = unlockedByDefault;
			Unlocked = unlockedByDefault;

			ColorCount = (int)MathHelper.Clamp(colorData.Length, 0, MaxColorCount);

			this.colorData = new PatternColorData[MaxColorCount];
			Array.Fill(this.colorData, new PatternColorData());
			for (int i = 0; i < ColorCount; i++)
				this.colorData[i] = colorData[i];
		}

		public Color GetColor(int index)
		{
			if (index >= 0 && index < ColorCount)
			{
				if (colorData[index].HasColorFunction)
					return colorData[index].ColorFunction.Invoke(colorData.Select(c => c.UserColor).ToArray());
				else if (!colorData[index].IsUserChangeable)
					return colorData[index].DefaultColor;
 				else 
					return colorData[index].UserColor;
			}
			return Color.Transparent;
		}

		public bool TrySetColor(int index, Color color)
		{
			if (index < 0 || index >= ColorCount || !colorData[index].IsUserChangeable)
				return false;

            colorData[index].ColorFunction = null;
			colorData[index].UserColor = color;
            return true;
		}

		public bool TrySetColor(int index, PatternColorFunction colorFunction)
		{
			if (index < 0 || index >= ColorCount || !colorData[index].IsUserChangeable)
				return false;

			colorData[index].ColorFunction = colorFunction;
			return true;
		}

		public Color GetDefaultColor(int index)
		{
			if (index >= 0 && index < ColorCount)
			{
				if (colorData[index].HasColorFunction)
					return colorData[index].ColorFunction.Invoke(colorData.Select(c => c.UserColor).ToArray());
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

		public Pattern Clone() => DeserializeData(SerializeData());

        public static readonly Func<TagCompound, Pattern> DESERIALIZER = DeserializeData;

		public TagCompound SerializeData()
		{
			TagCompound tag = new()
			{
				["Name"] = Name,
				["ModuleName"] = ModuleName
			};

			// Save the user-changed colors
			for (int i = 0; i < ColorCount; i++)
 				if (colorData[i].IsUserChangeable && !colorData[i].HasColorFunction)
					tag[$"UserColor{i}"] = colorData[i].UserColor;  
 
			return tag;
		}

		public static Pattern DeserializeData(TagCompound tag)
		{
			string name = tag.GetString("Name");
			string moduleName = tag.GetString("ModuleName");

			Pattern originalPatternData = CustomizationStorage.GetPattern(moduleName, name);
            Pattern pattern = new(moduleName, name, originalPatternData.UnlockedByDefault, originalPatternData.colorData);

			// Load the user-changed colors
			for (int i = 0; i < pattern.ColorCount; i++)
				if (tag.ContainsKey($"UserColor{i}") && pattern.colorData[i].IsUserChangeable && !pattern.colorData[i].HasColorFunction)
					pattern.colorData[i].UserColor = tag.Get<Color>($"UserColor{i}");

			return pattern;
		}
	}
}
