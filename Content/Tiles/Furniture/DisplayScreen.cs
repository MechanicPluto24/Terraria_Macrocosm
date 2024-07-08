using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture
{
    public class DisplayScreen : ModTile
    {

        private static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileTable[Type] = false;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.HasOutlines[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Origin = new Point16(3, 2);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
            TileObjectData.addTile(Type);

            DustType = -1;

            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(63, 63, 64), Language.GetText("Painting"));

            RegisterItemDrop(ModContent.ItemType<Items.Furniture.DisplayScreen>(), 0, 1, 2, 3);
        }

        public override void HitWire(int i, int j)
        {
            int leftX = i - Main.tile[i, j].TileFrameX / 18 % 6;
            int topY = j - Main.tile[i, j].TileFrameY / 18 % 4;

            for (int x = leftX; x < leftX + 6; x++)
            {
                for (int y = topY; y < topY + 4; y++)
                {
                    if (Main.tile[x, y].TileFrameY / 18 / 4 == 3)
                        Main.tile[x, y].TileFrameY = (short)(Main.tile[x, y].TileFrameY % (4 * 18));
                    else
                        Main.tile[x, y].TileFrameY += 4 * 18;

                    if (Wiring.running)
                        Wiring.SkipWire(x, y);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, leftX, topY, 6, 4);
        }

        public override bool RightClick(int i, int j)
        {
            int leftX = i - Main.tile[i, j].TileFrameX / 18 % 6;
            int topY = j - Main.tile[i, j].TileFrameY / 18 % 4;

            for (int x = leftX; x < leftX + 6; x++)
            {
                for (int y = topY; y < topY + 4; y++)
                {
                    if (Main.tile[x, y].TileFrameY / 18 / 4 == 3)
                        Main.tile[x, y].TileFrameY = (short)(Main.tile[x, y].TileFrameY % (4 * 18));
                    else
                        Main.tile[x, y].TileFrameY += 4 * 18;
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, leftX, topY, 6, 4);

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Furniture.DisplayScreen>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i,j].TileFrameY / 18 / 4 > 0) 
            {
                // Green LED
                if (Main.tile[i, j].TileFrameY / 18 % 4 == 3)
                {
                    g += 0.1f;
                }
                else
                {
                    // Earth screen
                    if (Main.tile[i, j].TileFrameY / 18 / 4 == 1)
                    {
                        r += 0.04f;
                        g += 0.22f;
                        b += 0.33f;
                    }
                    // Moon screen
                    else if (Main.tile[i, j].TileFrameY / 18 / 4 == 2)
                    {
                        r += 0.2f;
                        g += 0.2f;
                        b += 0.2f;
                    }
                    // Earth -> Moon travel screen
                    else if (Main.tile[i, j].TileFrameY / 18 / 4 == 2)
                    {
                        r += 0.1f;
                        g += 0.45f;
                        b += 0.25f;
                    }
                }
            }
            else 
            {
                // Red LED
                if (Main.tile[i, j].TileFrameY / 18 % 4 == 3)
                    r += 0.1f;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            Utility.DrawTileExtraTexture(i,j, spriteBatch, glowmask);
        }
    }
}