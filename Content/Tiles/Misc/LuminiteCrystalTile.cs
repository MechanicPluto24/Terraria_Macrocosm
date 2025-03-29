using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System;

namespace Macrocosm.Content.Tiles.Misc
{
    public class LuminiteCrystalTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            AddMapEntry(new Color(0, 200, 100), CreateMapEntryName());
            HitSound = SoundID.Item27;
            DustType = ModContent.DustType<LuminiteBrightDust>();
            Main.tileSpelunker[Type] = true;
            RegisterItemDrop(ModContent.ItemType<Items.Consumables.Throwable.LunarCrystal>(), 0);

        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 5;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            r = (float)Math.Abs(Math.Cos(((i*j)+(Main.time/200)))*0.07f)*0.3f;
            g = ((float)Math.Abs(Math.Cos(((i)+(Main.time/150)))*0.12f)*0.5f)+0.5f;
            b = ((float)Math.Abs(Math.Cos(((j)+(Main.time/250)))*0.03f)*0.3f)+0.4f;
        }
        
        public override bool CanPlace(int i, int j)
        {
            Tile below = Main.tile[i, j + 1];
            Tile above = Main.tile[i, j - 1];
            Tile right = Main.tile[i + 1, j];
            Tile left = Main.tile[i - 1, j];
            if ((below.Slope == SlopeType.Solid && !below.IsHalfBlock && below.HasUnactuatedTile && Main.tileSolid[below.TileType]) ||(above.Slope == SlopeType.Solid && !above.IsHalfBlock && above.HasUnactuatedTile && Main.tileSolid[above.TileType]) ||(right.Slope == SlopeType.Solid && !right.IsHalfBlock && right.HasUnactuatedTile && Main.tileSolid[right.TileType]) ||(left.Slope == SlopeType.Solid && !left.IsHalfBlock && left.HasUnactuatedTile && Main.tileSolid[left.TileType]))
                return true;
            return false;
        }
        
        
        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile below = Main.tile[i, j + 1];
            Tile above = Main.tile[i, j - 1];
            Tile right = Main.tile[i + 1, j];
            Tile left = Main.tile[i - 1, j];
            if (below.Slope == SlopeType.Solid && !below.IsHalfBlock && below.HasUnactuatedTile && Main.tileSolid[below.TileType])
                Main.tile[i, j].TileFrameY = 0;
            else if (above.Slope == SlopeType.Solid && !above.IsHalfBlock && above.HasUnactuatedTile && Main.tileSolid[above.TileType])
                Main.tile[i, j].TileFrameY = 18;
            else if (right.Slope == SlopeType.Solid && !right.IsHalfBlock && right.HasUnactuatedTile && Main.tileSolid[right.TileType])
                Main.tile[i, j].TileFrameY = 36;
            else if (left.Slope == SlopeType.Solid && !left.IsHalfBlock && left.HasUnactuatedTile && Main.tileSolid[left.TileType])
                Main.tile[i, j].TileFrameY = 54;

            Main.tile[i, j].TileFrameX = (short)(WorldGen.genRand.Next(20) * 18);
        }
        
    }
}