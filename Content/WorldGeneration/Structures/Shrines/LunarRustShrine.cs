using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Structures.Shrines
{
    public class LunarRustShrine : Structure
    {
        public override bool PrePlace(Point16 origin)
        {
            return true;
        }

        public override void PostPlace(Point16 origin)
        {
            WorldUtils.Gen(new Point(origin.X - Size.X / 4, origin.Y + 10), new CustomShapes.Chasm(65, 1, 10, 2, 0, dir: false), Actions.Chain(new CustomActions.ClearTileSafelyPostGen()));
            WorldUtils.Gen(new Point(origin.X - Size.X / 4, origin.Y + 10), new CustomShapes.Chasm(65, 10, 180, 2, 0, dir: true), Actions.Chain(new CustomActions.ClearTileSafelyPostGen()));

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
                    type: TileID.LunarBlockVortex
                );
            }
        }
    }
}
