using Macrocosm.Common.Utils;
using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Macrocosm.Content.Tiles.Furniture.Luminite;

namespace Macrocosm.Content.WorldGeneration.Structures.Shrines;

public class StarRoyaleShrine : Structure
{
    public override bool PrePlace(Point16 origin)
    {
        return true;
    }

    public override void PostPlace(Point16 origin)
    {
        for (int i = origin.X; i < origin.X + Size.X; i++)
        {
            for (int j = origin.Y; j < origin.Y + Size.Y; j++)
            {
                Tile tile = Main.tile[i, j];
                if (tile.TileType == ModContent.TileType<LuminiteChest>())
                    Utility.SetTileStyle(i, j, 13);
            }
        }
        WorldUtils.Gen(new Point(origin.X + Size.X / 2, origin.Y - Size.Y / 4), new CustomShapes.ChasmSideways(30, 10, 100, 3, 0, dir: true), new CustomActions.ClearTileSafelyPostGen());
        WorldUtils.Gen(new Point(origin.X + Size.X / 2, origin.Y - Size.Y / 4), new CustomShapes.ChasmSideways(30, 10, 100, 3, 0, dir: false), new CustomActions.ClearTileSafelyPostGen());
        Utility.SafeTileRunner(origin.X + Size.X / 2, origin.Y + Size.Y / 4, Size.X * 1.2, 2, -1);

        int max = WorldGen.genRand.Next(5, 16);
        for (int vein = 0; vein < max; vein++)
        {
            WorldGen.OreRunner(
                i: origin.X + (int)(Size.X * WorldGen.genRand.NextFloat(1f)),
                j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(2f)),
                strength: WorldGen.genRand.Next(3, 6),
                steps: WorldGen.genRand.Next(8, 12),
                type: (ushort)ModContent.TileType<ArtemiteOre>()
            );

            WorldGen.OreRunner(
                i: origin.X + (int)(Size.X * WorldGen.genRand.NextFloat(1f)),
                j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(2f)),
                strength: WorldGen.genRand.Next(3, 6),
                steps: WorldGen.genRand.Next(8, 12),
                type: TileID.LunarBlockVortex
            );
        }
    }
}
