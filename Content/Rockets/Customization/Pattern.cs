using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Macrocosm.Common.UI;
using Newtonsoft.Json;

namespace Macrocosm.Content.Rockets.Customization
{
    public partial class Pattern : IUnlockable
    {
		public string Name { get; }

		public string ModuleName { get; }

		public bool Unlocked { get; set; }
		public bool UnlockedByDefault { get; private set; }

		public bool IsDefault => Name == "Basic";
		public bool HasDefaultColors { get; private set; } = true;

		public List<int> UserModifiableIndexes { get; } = new();
		public int UserModifiableColorCount => UserModifiableIndexes.Count;
		public int DefinitionColorCount { get; private set; }

		public PatternColorData[] ColorData { get; init; }

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

		public Pattern(string moduleName, string patternName, bool unlockedByDefault, params PatternColorData[] defaultColorData)
		{
			ModuleName = moduleName;
			Name = patternName;

			UnlockedByDefault = unlockedByDefault;
			Unlocked = unlockedByDefault;

			ColorData = new PatternColorData[MaxColorCount];
			Array.Fill(ColorData, new PatternColorData());

			for (int i = 0; i < defaultColorData.Length; i++)
			{
				if (defaultColorData[i].HasColorFunction)
				{
					ColorData[i] = new(defaultColorData[i].ColorFunction);
				}
				else
				{
					ColorData[i] = new(defaultColorData[i].Color, defaultColorData[i].IsUserModifiable);

					if(defaultColorData[i].IsUserModifiable)
						UserModifiableIndexes.Add(i);
				}
 			}
		}

		public string GetKey() => ModuleName + "_" + Name;

		public Color GetColor(int index)
		{
			if (index >= 0 && index < MaxColorCount)
			{
				if (ColorData[index].HasColorFunction)
					return ColorData[index].ColorFunction.Invoke(ColorData.Select((c, i) => i == index ? Color.Transparent : GetColor(i)).ToArray());
 				else 
					return ColorData[index].Color;
			}
			return Color.Transparent;
		}

		public bool SetColor(int index, Color color, bool evenIfNotUserModifiable = false)
		{
			if (index < 0 || index >= MaxColorCount || (!evenIfNotUserModifiable && !ColorData[index].IsUserModifiable))
				return false;

			HasDefaultColors = false;
			ColorData[index] = ColorData[index].WithUserColor(color);
			return true;
		}

		public bool SetColorFunction(int index, ColorFunction colorFunction)
		{
			if (index < 0 || index >= MaxColorCount || !ColorData[index].IsUserModifiable)
				return false;

			HasDefaultColors = false; //?
			ColorData[index] = ColorData[index].WithColorFunction(colorFunction);
			return true;
		}

		public PatternColorData[] GetColorData() => ColorData;

		public void SetColorData(params PatternColorData[] colorData)
		{
			for (int i = 0; i < colorData.Length; i++)
				ColorData[i] = colorData[i];
		}

		public UIPatternIcon ProvideUI()
		{
			UIPatternIcon icon = new(this);
			return icon;
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
