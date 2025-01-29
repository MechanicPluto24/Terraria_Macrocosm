using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Systems;
using Macrocosm.Content.Tiles.Trees;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class TreeHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            On_WorldGen.GetTreeType += On_WorldGen_GetTreeType;
            On_WorldGen.AttemptToGrowTreeFromSapling += On_WorldGen_AttemptToGrowTreeFromSapling;
            On_WorldGen.TryGrowingTreeByType += On_WorldGen_TryGrowingTreeByType;
        }

        public void Unload()
        {
            On_WorldGen.GetTreeType -= On_WorldGen_GetTreeType;
            On_WorldGen.AttemptToGrowTreeFromSapling -= On_WorldGen_AttemptToGrowTreeFromSapling;
            On_WorldGen.TryGrowingTreeByType -= On_WorldGen_TryGrowingTreeByType;
        }

        private TreeTypes On_WorldGen_GetTreeType(On_WorldGen.orig_GetTreeType orig, int tileType)
        {
            if (TileLoader.GetTile(tileType) is CustomTree customTree)
                return customTree.CountsAsTreeType;

            return orig(tileType);
        }

        private bool On_WorldGen_AttemptToGrowTreeFromSapling(On_WorldGen.orig_AttemptToGrowTreeFromSapling orig, int x, int y, bool underground)
        {
            if(SubworldSystem.AnyActive<Macrocosm>() && !RoomOxygenSystem.IsRoomPressurized(x, y))
                return false;

            int treeType = TileSets.SaplingTreeGrowthType[Main.tile[x, y].TileType];
            if (treeType > 0)
                return WorldGen.TryGrowingTreeByType(treeType, x, y);

            return orig(x, y, underground);
        }

        private bool On_WorldGen_TryGrowingTreeByType(On_WorldGen.orig_TryGrowingTreeByType orig, int treeTileType, int checkedX, int checkedY)
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && !RoomOxygenSystem.IsRoomPressurized(checkedX, checkedY))
                return false;

            if (TileLoader.GetTile(treeTileType) is CustomTree customTree)
                return customTree.GrowTree(checkedX, checkedY);

            return orig(treeTileType, checkedX, checkedY);
        }
    }
}
