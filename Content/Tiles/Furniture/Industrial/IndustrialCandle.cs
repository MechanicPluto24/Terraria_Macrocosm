using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Industrial
{
    [LegacyName("MoonBaseCandle")]
    public class IndustrialCandle : ModTile, IToggleableTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Candles, 0));
            TileObjectData.addTile(Type);

            AdjTiles = [TileID.Candles];
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            DustType = ModContent.DustType<IndustrialPlatingDust>();

            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Candle"));

            TileSets.RandomStyles[Type] = 2;

            // All styles
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.Industrial.IndustrialCandle>());
        }

        public void ToggleTile(int i, int j, bool skipWire = false)
        {
            if (Main.tile[i, j].TileFrameX >= 18)
                Main.tile[i, j].TileFrameX -= 18;
            else
                Main.tile[i, j].TileFrameX += 18;

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, i, j, 1, 1);
        }

        public override void HitWire(int i, int j)
        {
            ToggleTile(i, j, skipWire: true);
        }

        public override bool RightClick(int i, int j)
        {
            ToggleTile(i, j, skipWire: false);
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX < 18 && tile.TileFrameY < 18)
            {
                r = 1f;
                g = 1f;
                b = 1f;
            }
        }
    }
}
