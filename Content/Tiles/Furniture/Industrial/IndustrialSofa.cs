﻿using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Industrial
{
    [LegacyName("MoonBaseSofa")]
    public class IndustrialSofa : ModTile
    {
        private static Asset<Texture2D> extra;
        private static Asset<Texture2D> extraHighlight;

        public override void Load()
        {
            extra = ModContent.Request<Texture2D>(Texture + "_Extra");
            extraHighlight = ModContent.Request<Texture2D>(Texture + "_Extra_Highlight");
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeSatOnForNPCs[Type] = true;
            TileID.Sets.CanBeSatOnForPlayers[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.Sofa"));

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            AdjTiles = [TileID.Benches];
            DustType = ModContent.DustType<IndustrialPlatingDust>();

            TileSets.RandomStyles[Type] = 2;

            // All styles
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.Industrial.IndustrialSofa>());
        }

        // If on rightmost frame and there's no neighbor sofa to the right, draw extra texture to the right
        // This is to ensure the sofa is symmetrical while also connecting neatly to other sofas 
        // TODO: apply paints and include into tile preview
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Tile tileRight = Main.tile[i + 1, j];
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            if (tile.TileType == Type && tile.TileFrameX / 18 % 3 == 2 && tileRight.TileType != Type && !tile.IsTileInvisible)
            {
                TileObjectData data = TileObjectData.GetTileData(Type, 0);
                Vector2 position = new Vector2((i + 1) * 16f, j * 16f) - Main.screenPosition + zero;
                Color color = Utility.Colorize(tile.GetPaintColor(), Lighting.GetColor(i + 1, j));

                if (tile.TileFrameY / 18 % 2 is 0)
                    spriteBatch.Draw(extra.Value, position, new Rectangle(0, 0, 2, data.CoordinateHeights[0]), color);
                else if (tile.TileFrameY / 18 % 2 is 1)
                    spriteBatch.Draw(extra.Value, position, new Rectangle(0, data.CoordinateHeights[0] + data.CoordinatePadding, 2, data.CoordinateHeights[1]), color);

                // Also draw highlight extra if the actual tile is highlighted
                if (Main.InSmartCursorHighlightArea(i, j, out bool actuallySelected))
                {
                    int light = (color.R + color.G + color.B) / 3;
                    if (light > 10)
                    {

                        Color highlightColor = Colors.GetSelectionGlowColor(actuallySelected, light);

                        if (tile.TileFrameY / 18 % 2 == 0)
                            spriteBatch.Draw(extraHighlight.Value, position, new Rectangle(0, 0, 2, data.CoordinateHeights[0]), highlightColor);
                        else if (tile.TileFrameY / 18 % 2 == 1)
                            spriteBatch.Draw(extraHighlight.Value, position, new Rectangle(0, data.CoordinateHeights[0] + data.CoordinatePadding, 2, data.CoordinateHeights[1]), highlightColor);
                    }
                }
            }

            return true;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);
        }

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            Tile tile = Main.tile[i, j];
            info.TargetDirection = info.RestingEntity.direction;

            // Use this for frame-conditional offsets
            /*
			if ((tile.TileFrameX % 54 == 0 && info.TargetDirection == -1) || (tile.TileFrameX % 54 == 36 && info.TargetDirection == 1))
				info.VisualOffset = new Vector2(-4f, 2f);
			else if ((tile.TileFrameX % 54 == 0 && info.TargetDirection == 1) || (tile.TileFrameX % 54 == 36 && info.TargetDirection == -1))
				info.VisualOffset = new Vector2(4f, 2f);
			else
				info.VisualOffset = new Vector2(0f, 2f);
			*/

            info.VisualOffset = new Vector2(info.TargetDirection == 1 ? 0f : -2f, 2f);
            info.AnchorTilePosition = new(i, j);

            if (tile.TileFrameY / 18 % 2 == 0)
                info.AnchorTilePosition.Y += 1;
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
                return;

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));
        }
    }
}
