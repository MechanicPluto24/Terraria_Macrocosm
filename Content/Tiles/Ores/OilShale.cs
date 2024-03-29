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
            Main.tileOreFinderPriority[Type] = 320; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            MinPick = 40;
            MineResist = 5f;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(45, 46, 45), name);

            DustType = 84;
            HitSound = SoundID.Tink;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
            => true;
    }
}