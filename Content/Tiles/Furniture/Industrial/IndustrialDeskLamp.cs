﻿using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Industrial
{
    [LegacyName("MoonBaseDeskLamp")]
    public class IndustrialDeskLamp : ModTile, IToggleableTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.Table, 2, 0);

            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.newTile.StyleMultiplier = 4;
            TileObjectData.newTile.StyleWrapLimit = 4;

            // Place right alternate
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(2); // Skip style 1 "turned off" frame

            TileObjectData.addTile(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = [TileID.Candelabras];

            DustType = ModContent.DustType<IndustrialPlatingDust>();

            AddMapEntry(new Color(200, 200, 200), CreateMapEntryName());

            TileSets.RandomStyles[Type] = 2;

            // All styles
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.Industrial.IndustrialDeskLamp>());
        }

        public void ToggleTile(int i, int j, bool skipWire = false)
        {
            int leftX = i - Main.tile[i, j].TileFrameX / 18 % 2;
            int topY = j - Main.tile[i, j].TileFrameY / 18 % 2;

            for (int x = leftX; x < leftX + 2; x++)
            {
                for (int y = topY; y < topY + 2; y++)
                {
                    // Turn light on and off based on frame.
                    // Each style has 2 "on" and 2 "off" frames per row. 
                    if (Main.tile[x, y].TileFrameX / 18 % 4 is 2 or 3)
                        Main.tile[x, y].TileFrameX -= 36;
                    else
                        Main.tile[x, y].TileFrameX += 36;

                    if (skipWire && Wiring.running)
                        Wiring.SkipWire(x, y);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, leftX, topY, 2, 2);
        }

        public override void HitWire(int i, int j)
        {
            ToggleTile(i, j, skipWire: true);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX / 18 % 4 is 0)
                tile.GetEmmitedLight(Color.White, applyPaint: true, out r, out g, out b);
        }
    }
}
