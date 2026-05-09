using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Terrain;

public class Basalt : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLighted[Type] = true;

        TileID.Sets.ChecksForMerge[Type] = true;
        Regolith.TileMerge[Type] = true;
        Main.tileMerge[Type][ModContent.TileType<Chalcedony>()] = true;
        Main.tileMerge[ModContent.TileType<Chalcedony>()][Type] = true;
        Main.tileMerge[Type][ModContent.TileType<Protolith>()] = true;
        Main.tileMerge[ModContent.TileType<Protolith>()][Type] = true;
        TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

        MinPick = 225;
        MineResist = 3f;

        AddMapEntry(new Color(75, 65, 78));

        HitSound = SoundID.Tink;
        DustType = ModContent.DustType<BasaltDust>();
    }

    public override bool CanExplode(int i, int j)
    {
        return false;
    }

    public override bool HasWalkDust() => Main.rand.NextBool(3) && MacrocosmSubworld.GetGravityMultiplier() != 0f;

    public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
    {
        dustType = ModContent.DustType<BasaltDust>();
    }

    public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
    {
        WorldGen.TileMergeAttempt(-2, ModContent.TileType<Regolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
    }
}
