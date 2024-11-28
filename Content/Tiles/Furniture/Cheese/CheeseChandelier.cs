using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Cheese
{
    public class CheeseChandelier : ModTile, IToggleableTile
    {
        private static Asset<Texture2D> flameTexture;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Chandeliers, 0));
            TileObjectData.addTile(Type);

            AdjTiles = [TileID.Chandeliers];
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            DustType = ModContent.DustType<CheeseDust>();

            AddMapEntry(new Color(220, 216, 121), Language.GetText("ItemName.Chandelier"));
        }

        public void ToggleTile(int i, int j, bool skipWire = false)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
            for (int x = left; x < left + 3; x++)
            {
                for (int y = top; y < top + 3; y++)
                {
                    if (Main.tile[x, y].TileFrameX >= 54)
                        Main.tile[x, y].TileFrameX -= 54;
                    else
                        Main.tile[x, y].TileFrameX += 54;

                    if (skipWire && Wiring.running)
                        Wiring.SkipWire(x, y);
                }
            }

            NetMessage.SendTileSquare(-1, left, top, 3, 3);
        }

        public override void HitWire(int i, int j)
        {
            ToggleTile(i, j, skipWire: true);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX == 0)
            {
                r = 0.34f;
                g = 0.9f;
                b = 0.62f;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            flameTexture ??= ModContent.Request<Texture2D>(Texture + "_Flame");
            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);

            for (int k = 0; k < 7; k++)
            {
                float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

                Utility.DrawTileExtraTexture(i, j, spriteBatch, flameTexture, drawOffset: new Vector2(xx, yy), drawColor: new Color(100, 100, 100, 0));
            }
        }
    }
}
