using Macrocosm.Common.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class TileSkyColorHook : ILoadable
    {
        public void Load(Mod mod)
        {
            On_Main.SetBackColor += On_Main_SetBackColor;
        }
        public void Unload()
        {
            On_Main.SetBackColor -= On_Main_SetBackColor;
        }

        private void On_Main_SetBackColor(On_Main.orig_SetBackColor orig, Main.InfoToSetBackColor info, out Color sunColor, out Color moonColor)
        {
            orig(info, out sunColor, out moonColor);

            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.ModifyColorOfTheSkies(ref Main.ColorOfTheSkies);
        }
    }
}