using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Misc
{
    public class LuminiteCrystal : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileFrameImportant[Type] = true;
            AddMapEntry(new Color(0, 200, 100), CreateMapEntryName());

            HitSound = SoundID.Item27;
            DustType = ModContent.DustType<LuminiteBrightDust>();

            RegisterItemDrop(ModContent.ItemType<Items.Consumables.Throwable.LunarCrystal>(), 0);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 5;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = (float)Math.Abs(Math.Cos((i * j) + (Main.time / 200)) * 0.07f) * 0.3f;
            g = ((float)Math.Abs(Math.Cos(i + (Main.time / 150)) * 0.12f) * 0.5f) + 0.5f;
            b = ((float)Math.Abs(Math.Cos(j + (Main.time / 250)) * 0.03f) * 0.3f) + 0.4f;
        }

        private static bool CheckTile(Tile neighbor) => false;
        public override bool CanPlace(int i, int j)
        {
            Tile below = Main.tile[i, j + 1];
            Tile above = Main.tile[i, j - 1];
            Tile right = Main.tile[i + 1, j];
            Tile left = Main.tile[i - 1, j];
            return WorldGen.SolidTile(below) || WorldGen.SolidTile(above) || WorldGen.SolidTile(right) || WorldGen.SolidTile(left);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if(!CanPlace(i, j))
            {
                WorldGen.KillTile(i, j);
                return false;
            }

            Tile below = Main.tile[i, j + 1];
            Tile above = Main.tile[i, j - 1];
            Tile right = Main.tile[i + 1, j];
            Tile left = Main.tile[i - 1, j];

            if (WorldGen.SolidTile(below))
                Main.tile[i, j].TileFrameY = 0;
            else if (WorldGen.SolidTile(above))
                Main.tile[i, j].TileFrameY = 18;
            else if (WorldGen.SolidTile(right))
                Main.tile[i, j].TileFrameY = 36;
            else if (WorldGen.SolidTile(left))
                Main.tile[i, j].TileFrameY = 54;

            Main.tile[i, j].TileFrameX = (short)(WorldGen.genRand.Next(20) * 18);

            return false;
        }
    }
}