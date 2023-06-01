using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rocket.Modules
{
	public class Pattern
	{
		public string PatternName { get; set; }
		public int ColorCount { get; set; }
		public Color[] DefaultColors { get; set; }

		//public Texture2D IconTexture { get; set; }
		//public int ItemType{ get; set; }

		public Pattern(string patternName, params Color[] defaultColors)
		{
			PatternName = patternName;
			ColorCount = defaultColors.Length;
			DefaultColors = defaultColors;
		}

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
