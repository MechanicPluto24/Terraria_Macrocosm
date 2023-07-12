using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria.ModLoader;
using ReLogic.Content;
using System;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.Rockets.Customization
{
    public class Pattern
    {
        public string Name { get; set; }
        public string ModuleName { get; set; }

		public int ColorCount { get; }
        public Color[] DefaultColors { get; }
		public Color[] Colors { get; set; }

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

		//public Texture2D IconTexture { get; set; }
		//public int ItemType{ get; set; }

		public Pattern(string moduleName, string patternName, params Color[] defaultColors)
        {
			ModuleName = moduleName;
            Name = patternName;

			DefaultColors = new Color[MaxColorCount];
			Array.Fill(DefaultColors, Color.White.NewAlpha(0));

			ColorCount = (int)MathHelper.Clamp(defaultColors.Length, 0, MaxColorCount);
			
			for(int i = 0; i < ColorCount; i++)
 				DefaultColors[i] = defaultColors[i];

			Colors = DefaultColors;
        }

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

		#region Autoload
		/*
		public void AutoloadPatterns()
		{
			// Find all existing patters for this module
			string lookupString = (HERE + MODULES[n] + "_Pattern_").Replace("Macrocosm/", "");
			PatternPaths = Macrocosm.Instance.RootContentSource.GetAllAssetsStartingWith(lookupString).ToList();

			// Log the pattern list
			string logstring = "Found " + PatternPaths.Count.ToString() + " pattern" + (PatternPaths.Count == 1 ? "" : "s") + " for rocket module " + GetType().Name + ": ";
			foreach (var pattern in PatternPaths)
				logstring += pattern.Replace(lookupString, "").Replace(".rawimg", "") + " ";
			Macrocosm.Instance.Logger.Info(logstring);
		}
		*/
		#endregion
	}
}
