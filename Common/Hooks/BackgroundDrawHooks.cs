using Macrocosm.Common.Subworlds;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class BackgroundDrawHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            On_Main.DrawBackgroundBlackFill += On_Main_DrawBackgroundBlackFill;
        }

        public void Unload()
        {
            On_Main.DrawBackgroundBlackFill -= On_Main_DrawBackgroundBlackFill;
        }

        private void On_Main_DrawBackgroundBlackFill(On_Main.orig_DrawBackgroundBlackFill orig, Main self)
        {
            if (SubworldSystem.Current is MultiSubworld)
                return;

            orig(self);
        }
    }
}
