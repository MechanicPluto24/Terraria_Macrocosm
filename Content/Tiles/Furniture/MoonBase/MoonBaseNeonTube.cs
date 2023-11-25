using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.MoonBase
{
    internal class MoonBaseNeonTube : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Chandeliers, 0));
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.addTile(Type);

            AdjTiles = new int[] { TileID.Chandeliers };
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            DustType = ModContent.DustType<MoonBasePlatingDust>();

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(253, 221, 3), name);
        }

        public override void HitWire(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;

            for (int x = left; x < left + 3; x++)
            {
                if (Main.tile[x, j].TileFrameX >= 54)
                    Main.tile[x, j].TileFrameX -= 54;
                else
                    Main.tile[x, j].TileFrameX += 54;
            }

            if (Wiring.running)
            {
                Wiring.SkipWire(left, j);
                Wiring.SkipWire(left + 1, j);
                Wiring.SkipWire(left + 2, j);
            }

            NetMessage.SendTileSquare(-1, left, j, 3);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX == 0)
            {
                r = 1f;
                g = 1f;
                b = 1f;
            }
        }
    }
}
