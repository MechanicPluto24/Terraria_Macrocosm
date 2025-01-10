using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.GameContent;
using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Sets;

namespace Macrocosm.Common.Hooks
{
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
}
