using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Tiles.Misc;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Terrain
{
    public class Protolith : ModTile
    {
        /// <summary> Types that merge with this. Use instead of <c>Main.tileFrame[][]</c> </summary>
        public static bool[] TileMerge { get; set; } = TileID.Sets.Factory.CreateBoolSet();

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            Regolith.TileMerge[Type] = true;

            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            MinPick = 225;
            MineResist = 3f;

            AddMapEntry(new Color(65, 65, 65));

            HitSound = SoundID.Tink;
            DustType = ModContent.DustType<ProtolithDust>();
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, ModContent.TileType<Regolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
            WorldGen.TileMergeAttemptFrametest(i, j, Type, TileMerge, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!SubworldSystem.IsActive<Moon>())
                return;

            if (j <= Main.rockLayer)
                return;

            if (!WorldGen.genRand.NextBool(40))
                return;

            if (Utility.GetTileCount(new(i, j), [ModContent.TileType<LuminiteCrystal>()], distance: 10) >= 2)
                return;

            Point[] directions = [
                new Point(0, -1), // up
                new Point(0, 1),  // down
                new Point(-1, 0), // left
                new Point(1, 0),  // right
            ];
            Utility.Shuffle(WorldGen.genRand, directions);

            Tile tile = Main.tile[i, j];
            foreach (var offset in directions)
            {
                int x = i + offset.X;
                int y = j + offset.Y;
                Tile target = Main.tile[x, y];
                if (!target.HasTile && WorldGen.SolidTile(tile))
                {
                    WorldGen.PlaceTile(x, y, ModContent.TileType<LuminiteCrystal>(), mute: true, forced: false);
                    NetMessage.SendTileSquare(-1, x, y, 1, 1);
                    break;
                }
            }

        }
    }
}