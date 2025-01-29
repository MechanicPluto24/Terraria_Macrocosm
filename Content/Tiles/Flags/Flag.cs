using Macrocosm.Common.Customization;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Flags
{
    public class Flag : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.MultiTileSway[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 5;

            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;

            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = true;

            TileObjectData.newTile.Origin = new(0, TileObjectData.newTile.Height - 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;

            TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, 1, TileObjectData.newTile.Width - 1);
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200), CreateMapEntryName());
            DustType = -1;
        }

        public override bool RightClick(int i, int j)
        {
            Main.mouseRightRelease = false;
            Utility.UICloseOthers();

            Point16 origin = Utility.GetMultitileTopLeft(i, j);
            // Open UI
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!player.mouseInterface)
            {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int frame = Main.tileFrame[type];
            var topLeft = Utility.GetMultitileTopLeft(i, j);
            if (frame > 0 && WorldGen.InAPlaceWithWind(topLeft.X, topLeft.Y, 3, 2))
            {
                frame = 1 + (topLeft.X + frame) % 4;
                bool direction = Main.tile[i, j].TileFrameX / (18 * 3) > 0;
                frameYOffset = 18 * 5 * (direction ? frame : 5 - frame);
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            float speed = Math.Abs(Utility.WindSpeedScaled);

            if (speed > 0.1f)
            {
                int ticksPerFrame = Math.Clamp((int)(10 * (1f - speed)), 4, 10);
                int frameCount = 5;
                if (++frameCounter >= ticksPerFrame)
                {
                    frameCounter = 0;
                    if (Utility.WindSpeedScaled < 0f)
                        frame++;
                    else
                        frame--;

                    if (frame >= frameCount)
                        frame = 1;

                    if (frame < 1)
                        frame = frameCount - 1;
                }
            }
            else
            {
                frame = 0;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (TileObjectData.IsTopLeft(tile))
                TileRendering.AddCustomSpecialPoint(i, j, CustomSpecialDraw);

            return false; // We must return false here to prevent the normal tile drawing code from drawing the default static tile. Without this a duplicate tile will be drawn.
        }

        private SpriteBatchState state;
        public void CustomSpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, state.BlendState, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, null, state.Matrix);

            Pattern[] patterns = PatternManager.GetAll(context: "FlagTile").ToArray();
            if (patterns.Length > 0)
            {
                var pattern = patterns[i % patterns.Length];
                pattern.Apply();
            }

            TileRendering.DrawMultiTileInWindBottomAnchor(i, j, perTileLighting: false, windSensitivity: 0.07f);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}