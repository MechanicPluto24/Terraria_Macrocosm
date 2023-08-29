using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Macrocosm.Content.UI.Themes
{
	public class UIThemeStorage : ILoadable
    {
        private static Dictionary<string, UITheme> themeStorage;

        public void Load(Mod mod)
        {
            themeStorage = new Dictionary<string, UITheme>();
            LoadData();
        }

        public void Unload()
        {
            themeStorage = null;
        }

        public static void Add(string key, UITheme theme)
            => themeStorage.Add(key, theme);

        public static UITheme GetValue(string key) => themeStorage[key];

        private void LoadData()
        {
            Add("DefaultTerraria", new UITheme(new Color(63, 82, 151) * 0.7f, Color.Black));
            Add("DefaultMacrocosm", new UITheme(new Color(53, 72, 135), new Color(89, 116, 213, 255)));
		}
    }
}
