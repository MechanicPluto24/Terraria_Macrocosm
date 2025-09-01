using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Ores
{
    public class OilShale : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
            Main.tileOreFinderPriority[Type] = 150; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            Main.tileMerge[TileID.Sandstone][Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(45, 46, 45), name);

            DustType = 84;
            HitSound = SoundID.Tink;

            MinPick = 35;
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, TileID.Sandstone, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
    }
}