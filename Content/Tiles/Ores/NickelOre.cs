using Macrocosm.Content.Dusts;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Ores;

public class NickelOre : ModTile
{
    public override void SetStaticDefaults()
    {
        TileID.Sets.Ore[Type] = true;
        Main.tileSpelunker[Type] = true;
        Main.tileOreFinderPriority[Type] = 250;
        Main.tileShine2[Type] = true;
        Main.tileShine[Type] = 975;
        Main.tileMergeDirt[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        TileID.Sets.ChecksForMerge[Type] = true;
        Regolith.TileMerge[Type] = true;

        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(220, 215, 205), name);

        DustType = ModContent.DustType<RegolithDust>();
        HitSound = SoundID.Tink;

        MinPick = 225;
        MineResist = 3f;
    }

    public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
    {
        WorldGen.TileMergeAttempt(-2, ModContent.TileType<Regolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
    }

    public override bool CanExplode(int i, int j)
    {
        return false;
    }
}
