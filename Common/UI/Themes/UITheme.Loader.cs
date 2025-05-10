using Macrocosm.Common.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Macrocosm.Common.UI.Themes
{
    public readonly partial struct UITheme : ILoadable
    {
        private static readonly Dictionary<string, UITheme> storage = new();
        public static UITheme Current => Get(ClientConfig.Instance.SelectedUITheme);
        public static UITheme Get(string name) => storage.TryGetValue(name, out var theme) ? theme : Terraria;

        public void Load(Mod mod)
        {
            storage.Add(Terraria.Name, Terraria);
            storage.Add(Macrocosm.Name, Macrocosm);
        }

        public void Unload()
        {
        }
    }
}
