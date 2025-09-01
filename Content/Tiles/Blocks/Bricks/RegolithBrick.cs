using Macrocosm.Content.Dusts;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Bricks
{
    public class RegolithBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            MinPick = 225;
            MineResist = 3f;

            AddMapEntry(new Color(165, 165, 165));

            DustType = ModContent.DustType<RegolithDust>();
            HitSound = SoundID.Tink;
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, ModContent.TileType<Regolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
    }
}