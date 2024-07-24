using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
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

        private static readonly int[] maxFramesByStyle = [
            1,  
            1,  
            1,  
            12, 
        ];

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
            TileObjectData.newTile.StyleHorizontal = false;
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
            int frameNumber = Main.tile[i, j].TileFrameX / (18 * 6);
            int leftX = i - Main.tile[i, j].TileFrameX / 18 % 6;
            int topY = j - Main.tile[i, j].TileFrameY / 18 % 4;

            for (int x = leftX; x < leftX + 6; x++)
            {
                for (int y = topY; y < topY + 4; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (frameNumber == 3)
                        tile.TileFrameX = (short)(tile.TileFrameX % (6 * 18));
                    else
                        tile.TileFrameX += 6 * 18;

                    if (Wiring.running)
                        Wiring.SkipWire(x, y);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, leftX, topY, 6, 4);
        }

        public override bool RightClick(int i, int j)
        {
            int frameNumber = Main.tile[i, j].TileFrameX / (18 * 6);
            int leftX = i - Main.tile[i, j].TileFrameX / 18 % 6;
            int topY = j - Main.tile[i, j].TileFrameY / 18 % 4;

            for (int x = leftX; x < leftX + 6; x++)
            {
                for (int y = topY; y < topY + 4; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (frameNumber == 3)
                        tile.TileFrameX = (short)(tile.TileFrameX % (6 * 18));
                    else
                        tile.TileFrameX += 6 * 18;
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

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            Tile tile = Main.tile[i, j];
            int frameNumber = tile.TileFrameX / (18 * 6);
            frameYOffset = 18 * 4 * Math.Clamp(Main.tileFrame[type], 0, maxFramesByStyle[frameNumber] - 1);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            int ticksPerFrame = 8;
            int frameCount = maxFramesByStyle.Max();
            if (++frameCounter >= ticksPerFrame)
            {
                frameCounter = 0;
                if (++frame >= frameCount)
                    frame = 0;
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            int frameNumber = tile.TileFrameX / (18 * 6);

            // Any ON frame
            if (frameNumber > 0) 
            {
                // Green LED
                if (tile.TileFrameX / 18 % 4 == 3) // 4th row of tiles
                {
                    g += 0.1f;
                }
                else
                {
                    // Earth screen frame
                    if (frameNumber == 1)
                    {
                        r += 0.04f;
                        g += 0.22f;
                        b += 0.33f;
                    }
                    // Moon screen frame
                    else if (frameNumber == 2)
                    {
                        r += 0.2f;
                        g += 0.2f;
                        b += 0.2f;
                    }
                    // Earth -> Moon transfer screen frame
                    else if (frameNumber == 3)
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
                if (tile.TileFrameX / 18 % 4 == 3)
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