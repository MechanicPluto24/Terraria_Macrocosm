using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Terrain
{
    public class Protolith : ModTile, IModifyTileFrame
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            MinPick = 225;
            MineResist = 3f;
            AddMapEntry(new Color(65, 65, 65));
            HitSound = SoundID.Tink;
            DustType = ModContent.DustType<ProtolithDust>();
        }

        public void ModifyTileFrame(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, ModContent.TileType<Regolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
            WorldGen.TileMergeAttemptFrametest(i, j, Type, ModContent.TileType<IrradiatedRock>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
    }
}