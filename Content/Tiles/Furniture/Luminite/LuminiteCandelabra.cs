using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Luminite
{
    public class LuminiteCandelabra : ModTile
    {
        private static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.Table, 2, 0);
            TileObjectData.newTile.CoordinateHeights = [16, 16];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 2;

            TileObjectData.addTile(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = [TileID.Candelabras];

            DustType = DustID.LunarOre;

            foreach (LuminiteStyle style in Enum.GetValues(typeof(LuminiteStyle)))
                AddMapEntry(Utility.GetTileColorFromLuminiteStyle(style), Language.GetText("ItemName.Candelabra"));

            // To complete direction-dependent item drops
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.Luminite.LuminiteCandelabra>(), 0, 1);
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.Heavenforge.HeavenforgeCandelabra>(), 2, 3);
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.LunarRust.LunarRustCandelabra>(), 4, 5);
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.Astra.AstraCandelabra>(), 6, 7);
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.DarkCelestial.DarkCelestialCandelabra>(), 8, 9);
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.Mercury.MercuryCandelabra>(), 10, 11);
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.StarRoyale.StarRoyaleCandelabra>(), 12, 13);
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.Cryocore.CryocoreCandelabra>(), 14, 15);
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.CosmicEmber.CosmicEmberCandelabra>(), 16, 17);
        }
        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Utility.GetDustTypeFromLuminiteStyle((LuminiteStyle)(Main.tile[i, j].TileFrameY / (18 * 2)));
            return true;
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameY / (18 * 2));


        public override void HitWire(int i, int j)
        {
            int leftX = i - Main.tile[i, j].TileFrameX / 18 % 2;
            int topY = j - Main.tile[i, j].TileFrameY / 18 % 2;

            for (int x = leftX; x < leftX + 2; x++)
            {
                for (int y = topY; y < topY + 2; y++)
                {
                    if (Main.tile[x, y].TileFrameX / 18 / 2 == 1)
                        Main.tile[x, y].TileFrameX -= 36;
                    else
                        Main.tile[x, y].TileFrameX += 36;

                    if (Wiring.running)
                        Wiring.SkipWire(x, y);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, leftX, topY, 2, 2);
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
            Utility.DrawTileExtraTexture(i, j, spriteBatch, glowmask);
        }
    }
}
