using Macrocosm.Common.Drawing;
using Macrocosm.Common.TileFrame;
using Macrocosm.Common.Utils;
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

        private static readonly int[] ticksPerFrameByStyle = [
            1,
            1,
            1,
            8,
            8,
            8,
            1,
            4
        ];

        private static readonly int[] maxFramesByStyle = [
            1,
            1,
            1,
            12,
            8,
            8,
            1,
            7
        ];

        private int StyleCount => maxFramesByStyle.Length;
        private int MaxVisible => StyleCount - 2;

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

            RegisterItemDrop(ModContent.ItemType<Items.Furniture.DisplayScreen>(), 0, 1, 2, 3, 4, 5, 6, 7);
        }

        public override void HitWire(int i, int j)
        {
            int style = Main.tile[i, j].TileFrameX / (18 * 6);
            int leftX = i - Main.tile[i, j].TileFrameX / 18 % 6;
            int topY = j - Main.tile[i, j].TileFrameY / 18 % 4;

            for (int x = leftX; x < leftX + 6; x++)
            {
                for (int y = topY; y < topY + 4; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (style == MaxVisible - 1)
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
            int style = Main.tile[i, j].TileFrameX / (18 * 6);
            int leftX = i - Main.tile[i, j].TileFrameX / 18 % 6;
            int topY = j - Main.tile[i, j].TileFrameY / 18 % 4;

            for (int x = leftX; x < leftX + 6; x++)
            {
                for (int y = topY; y < topY + 4; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (style == MaxVisible - 1)
                        tile.TileFrameX = (short)(tile.TileFrameX % (6 * 18));
                    else
                        tile.TileFrameX += 6 * 18;

                    TileAnimation.NewTemporaryAnimation(new AnimationData(4, 3, [0, 1, 2, 3, 4, 5, 6, 7]), x, y, Type);
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
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            Tile tile = Main.tile[i, j];

            if (TileAnimation.GetTemporaryFrame(i, j, out int tempFrame))
            {
                frameXOffset = (18 * 6 * (StyleCount - 1)) - tile.TileFrameX + (tile.TileFrameX % (18 * 6));
                frameYOffset = 18 * 4 * tempFrame;
            }
            else
            {
                int style = tile.TileFrameX / (18 * 6);
                int frame = Main.tileFrame[type] / ticksPerFrameByStyle[style];
                frameYOffset = 18 * 4 * (frame % maxFramesByStyle[style]);
            }
        }

        // Simple frame counter, divided differently for each style in AnimateIndividualTile
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frame >= int.MaxValue)
                frame = 0;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            int style = tile.TileFrameX / (18 * 6); // Determine the style based on frame

            if (style > 0)
            {
                // Green LED
                if (tile.TileFrameX / 18 % 4 == 3)
                {
                    tile.GetEmmitedLight(Color.Green, applyPaint: true, out r, out g, out b);
                }
                else
                {
                    if (!TileAnimation.GetTemporaryFrame(i, j, out _))
                    {
                        // Assign different colors based on style
                        Color lightColor = style switch
                        {
                            1 => new Color(20, 82, 110),   // Earth screen
                            2 => new Color(64, 64, 64),    // Moon screen
                            3 => new Color(25, 115, 64),   // Earth - Moon transfer
                            4 => (tile.TileFrameY / 18 % 4 > 2)
                                ? new Color(77, 0, 77)     // Earth Corruption (purple)
                                : new Color(0, 102, 0),    // Earth Corruption (green)
                            5 => (tile.TileFrameX / 18 % 6 > 3)
                                ? new Color(0, 102, 0)     // Earth Crimson (green)
                                : new Color(128, 0, 0),    // Earth Crimson (red)
                            6 => new Color(102, 102, 102), // Test card
                            7 => new Color(77, 77, 77),    // Static
                            _ => Color.Black               // Default fallback
                        };

                        // Apply the light color
                        tile.GetEmmitedLight(lightColor, applyPaint: true, out r, out g, out b);
                    }
                    else
                    {
                        // Temporary animation static frame
                        tile.GetEmmitedLight(new Color(77, 77, 77), applyPaint: true, out r, out g, out b);
                    }
                }
            }
            else
            {
                // Red LED for OFF state
                if (tile.TileFrameX / 18 % 4 == 3)
                {
                    tile.GetEmmitedLight(Color.Red, applyPaint: true, out r, out g, out b);
                }
            }
        }


        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            TileRendering.DrawTileExtraTexture(i, j, spriteBatch, glowmask, applyPaint: true, Color.White);
        }
    }
}