using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Macrocosm.Content.Tiles.Furniture.Luminite;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.WorldGeneration.Structures.Shrines;

public class MercuryShrine : Structure
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
                    Utility.SetTileStyle(i, j, 11);
            }
        }
        WorldUtils.Gen(new Point(origin.X - Size.Y / 4, origin.Y + 10), new CustomShapes.Chasm(30, 1, 10, 2, 0, dir: false), Actions.Chain(new CustomActions.ClearTileSafelyPostGen()));
        WorldUtils.Gen(new Point(origin.X - Size.Y / 4, origin.Y + 10), new CustomShapes.Chasm(30, 10, 140, 2, 0, dir: true), Actions.Chain(new CustomActions.ClearTileSafelyPostGen()));

        int max = WorldGen.genRand.Next(20, 31);
        for (int vein = 0; vein < max; vein++)
        {
            WorldGen.OreRunner(
                i: origin.X - (int)(Size.X * WorldGen.genRand.NextFloat(0.5f)) + (int)(Size.X * WorldGen.genRand.NextFloat(2.5f)),
                j: origin.Y - (int)(Size.Y * WorldGen.genRand.NextFloat(0.3f)) + (int)(Size.Y * WorldGen.genRand.NextFloat(1.3f)),
                strength: WorldGen.genRand.Next(3, 6),
                steps: WorldGen.genRand.Next(8, 12),
                type: (ushort)ModContent.TileType<SeleniteOre>()
            );

            WorldGen.OreRunner(
                i: origin.X - (int)(Size.X * WorldGen.genRand.NextFloat(0.5f)) + (int)(Size.X * WorldGen.genRand.NextFloat(2.5f)),
                j: origin.Y - (int)(Size.Y * WorldGen.genRand.NextFloat(0.3f)) + (int)(Size.Y * WorldGen.genRand.NextFloat(1.3f)),
                strength: WorldGen.genRand.Next(3, 6),
                steps: WorldGen.genRand.Next(8, 12),
                type: TileID.LunarBlockSolar
            );
        }
    }
}
