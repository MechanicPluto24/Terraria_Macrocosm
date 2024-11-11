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
    public class HeavenforgeShrine : Structure
    {
        public override bool PrePlace(Point16 origin)
        {
            return true;
        }

        public override void PostPlace(Point16 origin)
        {
            WorldUtils.Gen(new Point(origin.X + Size.X / 2, origin.Y - 2 + Size.Y / 4), new CustomShapes.ChasmSideways(12, 8, 80, 2, 0, dir: true), new CustomActions.ClearTileSafelyPostGen());
            WorldUtils.Gen(new Point(origin.X + Size.X / 2, origin.Y - 2 + Size.Y / 4), new CustomShapes.ChasmSideways(12, 8, 80, 2, 0, dir: false), new CustomActions.ClearTileSafelyPostGen());

            int max = WorldGen.genRand.Next(15, 26);
            for (int vein = 0; vein < max; vein++)
            {
                WorldGen.OreRunner(
                    i: origin.X + (int)(Size.X * WorldGen.genRand.NextFloat(1f)),
                    j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(2f)),
                    strength: WorldGen.genRand.Next(3, 6),
                    steps: WorldGen.genRand.Next(8, 12),
                    type: (ushort)ModContent.TileType<SeleniteOre>()
                );

                WorldGen.OreRunner(
                    i: origin.X + (int)(Size.X * WorldGen.genRand.NextFloat(1f)),
                    j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(2f)),
                    strength: WorldGen.genRand.Next(3, 6),
                    steps: WorldGen.genRand.Next(8, 12),
                    type: TileID.LunarBlockSolar
                );
            }
        }
    }
}
