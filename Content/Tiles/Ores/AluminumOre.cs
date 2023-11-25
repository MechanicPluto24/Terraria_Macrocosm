using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Ores
{
    public class AluminumOre : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 320;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 975;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(225, 217, 228), name);

            DustType = 84;
            HitSound = SoundID.Tink;

            MinPick = 40;
            MineResist = 5f;
        }
    }
}