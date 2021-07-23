using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;

namespace Macrocosm.Content.Tiles
{
    public class DianiteOre : ModTile
    {
        public override void SetDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
            Main.tileValue[Type] = 410; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
            Main.tileShine2[Type] = true; // Modifies the draw color slightly.
            Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            minPick = 225;
            mineResist = 5f;

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Dianite Ore");
            AddMapEntry(new Color(210, 116, 75), name);

            dustType = 84;
            drop = ModContent.ItemType<Items.Materials.DianiteOre>();
            soundType = SoundID.Tink;
            soundStyle = 1;
            //mineResist = 4f;
            //minPick = 200;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.8f;
            g = 0.25f;
            b = 0.2f;
        }
        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Dust.NewDust(new Vector2(i, j).ToWorldCoordinates(), 16, 16, ModContent.DustType<DianiteDust>());
            return false;
        }
    }
}