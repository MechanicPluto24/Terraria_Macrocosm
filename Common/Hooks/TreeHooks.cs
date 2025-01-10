using Macrocosm.Common.Bases.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class TreeHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            On_WorldGen.TryGrowingTreeByType += On_WorldGen_TryGrowingTreeByType;
            On_WorldGen.GetTreeType += On_WorldGen_GetTreeType;
        }

        public void Unload()
        {
            On_WorldGen.TryGrowingTreeByType -= On_WorldGen_TryGrowingTreeByType;
            On_WorldGen.GetTreeType -= On_WorldGen_GetTreeType;
        }

        private bool On_WorldGen_TryGrowingTreeByType(On_WorldGen.orig_TryGrowingTreeByType orig, int treeTileType, int checkedX, int checkedY)
        {
            if (TileLoader.GetTile(treeTileType) is CustomTree customTree)
                return customTree.GrowTree(checkedX, checkedY);

            return orig(treeTileType, checkedX, checkedY);
        }

        private TreeTypes On_WorldGen_GetTreeType(On_WorldGen.orig_GetTreeType orig, int tileType)
        {
            if (TileLoader.GetTile(tileType) is CustomTree customTree)
                return customTree.CountsAsTreeType;

            return orig(tileType);
        }
    }
}
