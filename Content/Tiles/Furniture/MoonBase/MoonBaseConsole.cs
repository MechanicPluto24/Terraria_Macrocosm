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

namespace Macrocosm.Content.Tiles.Furniture.MoonBase
{
    public class MoonBaseConsole : ModTile
    {
        private static Asset<Texture2D> glowmask;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.Table, 2, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(3);
            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;

            DustType = ModContent.DustType<MoonBasePlatingDust>();

            AddMapEntry(new Color(200, 200, 200), CreateMapEntryName());

            RegisterItemDrop(ModContent.ItemType<Items.Furniture.MoonBase.MoonBaseConsole>(), 0, 1);
        }

        public override void HitWire(int i, int j)
        {
            int leftX = i - Main.tile[i, j].TileFrameX / 18 % 2;
            int topY = j - Main.tile[i, j].TileFrameY / 18 % 2;

            for (int x = leftX; x < leftX + 2; x++)
            {
                for (int y = topY; y < topY + 2; y++)
                {
                    if (Main.tile[x, y].TileFrameY < 36)
                        Main.tile[x, y].TileFrameY += 36;
                    else
                        Main.tile[x, y].TileFrameY -= 36;

                    if (Wiring.running)
                        Wiring.SkipWire(x, y);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, leftX, topY, 2, 2);
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY >= 18 * 2)
            {
                frameYOffset = 18 * 2 * Main.tileFrame[type];
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            int ticksPerFrame = 10;
            int frameCount = 6;
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
            if (tile.TileFrameY >= 18 * 2)
            {
                r = 0f;
                g = 0.1f;
                b = 0f;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            Utility.DrawTileGlowmask(i, j, spriteBatch, glowmask);
        }
    }
}
