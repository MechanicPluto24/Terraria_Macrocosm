using Macrocosm.Common.Sets;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks;

internal class PaintSystemHooks : ILoadable
{
    public void Load(Mod mod)
    {
        On_TreePaintSystemData.GetTileSettings += On_TreePaintSystemData_GetTileSettings;
    }

    public void Unload()
    {
        On_TreePaintSystemData.GetTileSettings -= On_TreePaintSystemData_GetTileSettings;
    }

    private TreePaintingSettings On_TreePaintSystemData_GetTileSettings(On_TreePaintSystemData.orig_GetTileSettings orig, int tileType, int tileStyle)
    {
        if (TileSets.PaintingSettings[tileType] is TreePaintingSettings settings)
            return settings;

        return orig(tileType, tileStyle);
    }
}
