using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Systems;
using Macrocosm.Content.Tiles.Trees;
using SubworldLibrary;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks;

public class TreeHooks : ILoadable
{
    private const int HeveaTreeChance = 5;

    public void Load(Mod mod)
    {
        On_WorldGen.GetTreeType_int += On_WorldGen_GetTreeType;
        On_WorldGen.GrowTree += On_WorldGen_GrowTree;
        On_WorldGen.AttemptToGrowTreeFromSapling += On_WorldGen_AttemptToGrowTreeFromSapling;
        On_WorldGen.TryGrowingTreeByType += On_WorldGen_TryGrowingTreeByType;
    }

    public void Unload()
    {
        On_WorldGen.GetTreeType_int -= On_WorldGen_GetTreeType;
        On_WorldGen.GrowTree -= On_WorldGen_GrowTree;
        On_WorldGen.AttemptToGrowTreeFromSapling -= On_WorldGen_AttemptToGrowTreeFromSapling;
        On_WorldGen.TryGrowingTreeByType -= On_WorldGen_TryGrowingTreeByType;
    }

    private TreeTypes On_WorldGen_GetTreeType(On_WorldGen.orig_GetTreeType_int orig, int tileType)
    {
        if (TileLoader.GetTile(tileType) is CustomTree customTree)
            return customTree.CountsAsTreeType;

        return orig(tileType);
    }

    private bool On_WorldGen_GrowTree(On_WorldGen.orig_GrowTree orig, int i, int y, int treeHeightAddon, bool ignoreWalls)
    {
        if (Main.tile[i, y].TileType == TileID.JungleGrass && WorldGen.genRand.NextBool(HeveaTreeChance))
        {
            if (WorldGen.TryGrowingTreeByType(ModContent.TileType<HeveaTree>(), i, y, treeHeightAddon, ignoreWalls))
                return true;
        }

        return orig(i, y, treeHeightAddon, ignoreWalls);
    }

    private bool On_WorldGen_AttemptToGrowTreeFromSapling(On_WorldGen.orig_AttemptToGrowTreeFromSapling orig, int x, int y, bool underground, int treeHeightAddon, bool ignoreWalls)
    {
        if (SubworldSystem.AnyActive<Macrocosm>() && !RoomOxygenSystem.CheckRoomOxygen(x, y))
            return false;

        int treeType = TileSets.SaplingTreeGrowthType[Main.tile[x, y].TileType];
        if (treeType > 0)
            return WorldGen.TryGrowingTreeByType(treeType, x, y, treeHeightAddon, ignoreWalls);

        return orig(x, y, underground, treeHeightAddon, ignoreWalls);
    }

    private bool On_WorldGen_TryGrowingTreeByType(On_WorldGen.orig_TryGrowingTreeByType orig, int treeTileType, int checkedX, int checkedY, int treeHeightAddon, bool ignoreWalls)
    {
        if (SubworldSystem.AnyActive<Macrocosm>() && !RoomOxygenSystem.CheckRoomOxygen(checkedX, checkedY))
            return false;

        if (TileLoader.GetTile(treeTileType) is CustomTree customTree)
            return customTree.GrowTree(checkedX, checkedY, treeHeightAddon, ignoreWalls);

        return orig(treeTileType, checkedX, checkedY, treeHeightAddon, ignoreWalls);
    }
}
