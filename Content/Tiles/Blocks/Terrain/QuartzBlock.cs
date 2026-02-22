using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Tiles.Blocks.Terrain;

public class QuartzBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBrick[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileShine2[Type] = true;
        Main.tileShine[Type] = 975;

        TileID.Sets.ChecksForMerge[Type] = true;
        TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

        Chalcedony.TileMerge[Type] = true;

        MinPick = 225;
        MineResist = 0.5f;
        AddMapEntry(new Color(255, 255, 255));
        HitSound = SoundID.Tink;
        DustType = ModContent.DustType<QuartzDust>();
    }

    public override bool CanExplode(int i, int j)
    {
        return false;
    }

    public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
    {
        WorldGen.TileMergeAttempt(-2, ModContent.TileType<Chalcedony>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
    }
}