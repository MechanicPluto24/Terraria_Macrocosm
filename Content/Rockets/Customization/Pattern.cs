using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Customization
{
    public class Pattern : TagSerializable, IUnlockable
    {
		public string Name { get; }
		public string ModuleName { get; }

		public string GetKey() => ModuleName + "_" + Name;

		public bool Unlocked { get; set; }
		public bool UnlockedByDefault { get; private set; }

		public bool IsDefault => Name == "Basic";
		public bool HasDefaultColors { get; private set; } = true;

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
				if (ModContent.RequestIfExists(IconTexturePath, out Asset<Texture2D> paintMask))
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
					return colorData[index].ColorFunction.Invoke(colorData.Select((c, i) => i == index ? Color.Transparent : GetColor(i)).ToArray());
				else if (!colorData[index].IsUserModifiable)
					return colorData[index].DefaultColor;
 				else 
					return colorData[index].UserColor;
			}
			return Color.Transparent;
		}

		public List<int> GetUserModifiableIndexes()
		{
			List<int> result = new();

			for (int i = 0; i < ColorCount; i++)
				if (colorData[i].IsUserModifiable && !colorData[i].HasColorFunction)
					result.Add(i);

			return result;
		}

		public void SetColor(int index, Color color)
		{
			if (index < 0 || index >= ColorCount || !colorData[index].IsUserModifiable)
				return;

			HasDefaultColors = false;
			colorData[index].ColorFunction = null;
			colorData[index].UserColor = color;
		}

		public bool TrySetColor(int index, Color color)
		{
			if (index < 0 || index >= ColorCount || !colorData[index].IsUserModifiable)
				return false;

			HasDefaultColors = false;
			colorData[index].ColorFunction = null;
			colorData[index].UserColor = color;
            return true;
		}

		public void SetColor(int index, PatternColorFunction colorFunction)
		{
			if (index < 0 || index >= ColorCount || !colorData[index].IsUserModifiable)
				return;

			HasDefaultColors = false;
			colorData[index].ColorFunction = colorFunction;
		}

		public bool TrySetColor(int index, PatternColorFunction colorFunction)
		{
			if (index < 0 || index >= ColorCount || !colorData[index].IsUserModifiable)
				return false;

			colorData[index].ColorFunction = colorFunction;
			return true;
		}

		/*
		public Color GetDefaultColor(int index)
		{
			if (index >= 0 && index < ColorCount)
			{
				if (colorData[index].HasColorFunction)
					return colorData[index].ColorFunction.Invoke(colorData.Select(c => c.DefaultColor).ToArray());
				else
					return colorData[index].DefaultColor;
			}
			return Color.Transparent;
		}
		*/

		public UIPatternIcon ProvideUI()
		{
			UIPatternIcon icon = new(this);
			return icon;
		}

		public void DrawIcon(SpriteBatch spriteBatch, Vector2 position)
		{
			// Load the coloring shader
			Effect effect = ModContent.Request<Effect>(Macrocosm.EffectAssetsPath + "SimpleColorMaskShading", AssetRequestMode.ImmediateLoad).Value;

			// Pass the pattern icon to the shader via the S1 register
			Main.graphics.GraphicsDevice.Textures[1] = IconTexture;

			// Change sampler state for proper alignment at all UI scales 
			SamplerState samplerState = spriteBatch.GraphicsDevice.SamplerStates[1];
			Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

			//Pass the color mask keys as Vector3s and configured colors as Vector4s
			//Note: parameters are scalars intentionally, I manually unrolled the loop in the shader to reduce number of branch instructions -- Feldy
			for (int i = 0; i < MaxColorCount; i++)
			{
				effect.Parameters["uColorKey" + i.ToString()].SetValue(ColorKeys[i]);
				effect.Parameters["uColor" + i.ToString()].SetValue(GetColor(i).ToVector4());
			}

			var state = spriteBatch.SaveState();
			spriteBatch.End();
			spriteBatch.Begin(state.SpriteSortMode, state.BlendState, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);
 
			spriteBatch.Draw(IconTexture, position, null, Color.White, 0f, Vector2.Zero, 0.995f, SpriteEffects.None, 0f);

			spriteBatch.End();
			spriteBatch.Begin(state);

			// Clear the tex registers  
			Main.graphics.GraphicsDevice.Textures[1] = null;

			// Restore the sampler states
			Main.graphics.GraphicsDevice.SamplerStates[1] = samplerState;
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
 				if (colorData[i].IsUserModifiable && !colorData[i].HasColorFunction)
					tag[$"UserColor{i}"] = colorData[i].UserColor;  
 
			return tag;
		}

		public static Pattern DeserializeData(TagCompound tag)
		{
			string name = tag.GetString("Name");
			string moduleName = tag.GetString("ModuleName");

			Pattern originalPatternData = CustomizationStorage.GetPatternReference(moduleName, name);
			PatternColorData[] colorData = originalPatternData.colorData.Select(data => data.Clone()).ToArray();
			Pattern pattern = new(moduleName, name, originalPatternData.UnlockedByDefault, colorData);

			// Load the user-changed colors
			for (int i = 0; i < pattern.ColorCount; i++)
				if (tag.ContainsKey($"UserColor{i}") && pattern.colorData[i].IsUserModifiable && !pattern.colorData[i].HasColorFunction)
					pattern.colorData[i].UserColor = tag.Get<Color>($"UserColor{i}");

			return pattern;
		}
	}
}
