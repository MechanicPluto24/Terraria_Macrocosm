using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Luminite
{
    public class LuminiteChandelier : ModTile, IToggleableTile
    {
        private static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Chandeliers, 0));
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = [16, 16];
            TileObjectData.addTile(Type);

            AdjTiles = [TileID.Chandeliers];
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            DustType = DustID.LunarOre;

            foreach (LuminiteStyle style in Enum.GetValues(typeof(LuminiteStyle)))
                AddMapEntry(Utility.GetTileColorFromLuminiteStyle(style), Language.GetText("ItemName.Chandelier"));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Utility.GetDustTypeFromLuminiteStyle((LuminiteStyle)(Main.tile[i, j].TileFrameY / (18 * 2)));
            return true;
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameY / (18 * 2));

        public void ToggleTile(int i, int j, bool skipWire = false)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 2;
            for (int x = left; x < left + 3; x++)
            {
                for (int y = top; y < top + 2; y++)
                {
                    if (Main.tile[x, y].TileFrameX >= 54)
                        Main.tile[x, y].TileFrameX -= 54;
                    else
                        Main.tile[x, y].TileFrameX += 54;

                    if (skipWire && Wiring.running)
                        Wiring.SkipWire(x, y);
                }
            }

            NetMessage.SendTileSquare(-1, left, top, 3, 2);
        }

        public override void HitWire(int i, int j)
        {
            ToggleTile(i, j, skipWire: true);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            Vector3 color = Utility.GetLightColorFromLuminiteStyle((LuminiteStyle)(Main.tile[i, j].TileFrameY / (18 * 2))).ToVector3();
            if (tile.TileFrameX == 0)
            {
                r = color.X;
                g = color.Y;
                b = color.Z;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            Utility.DrawTileExtraTexture(i, j, spriteBatch, glowmask, new Vector2(0, 0));
        }
    }
}
