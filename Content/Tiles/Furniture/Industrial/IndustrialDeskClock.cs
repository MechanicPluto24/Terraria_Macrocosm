using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Industrial
{
    [LegacyName("MoonBaseDeskClock")]
    public class IndustrialDeskClock : ModTile, IToggleableTile
    {
        private static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.Clock[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.Table, 2, 0);
            TileObjectData.addTile(Type);

            DustType = ModContent.DustType<IndustrialPlatingDust>();

            AddMapEntry(new Color(200, 200, 200), CreateMapEntryName());
        }

        public void ToggleTile(int i, int j, bool skipWire = false)
        {
            Tile tile = Main.tile[i, j];
            int leftX = i - tile.TileFrameX / 18 % 2;
            short frameAdjustment = (short)(tile.TileFrameY > 0 ? -18 : 18);

            for (int x = leftX; x < leftX + 2; x++)
            {
                Main.tile[x, j].TileFrameY += frameAdjustment;

                if (Wiring.running)
                    Wiring.SkipWire(x, j);
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, leftX, j, 1, 2);
        }

        public override void HitWire(int i, int j)
        {
            ToggleTile(i, j, true);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
        }

        public override bool RightClick(int x, int y)
        {
            if (Main.tile[x, y].TileFrameY >= 18)
            {
                Utility.PrintTime();
                return true;
            }

            return false;
        }

        public override void MouseOver(int i, int j)
        {
            if (Main.tile[i, j].TileFrameY >= 18)
            {
                Player player = Main.LocalPlayer;
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY >= 18)
            {
                r = 0f;
                g = 0.1f;
                b = 0f;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            Utility.DrawTileExtraTexture(i, j, spriteBatch, glowmask);
        }
    }
}
