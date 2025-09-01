using Macrocosm.Common.Drawing;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines
{
    public class ACUnit : MachineTile
    {
        private static Asset<Texture2D> glowmask;

        public override short Width => 2;
        public override short Height => 1;
        public override MachineTE MachineTE => ModContent.GetInstance<ACUnitTE>();

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = false;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.Origin = new Point16(0, 0);

            TileObjectData.newTile.CoordinateHeights = [16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.newTile.LavaDeath = true;

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(MachineTE.Hook_AfterPlacement, -1, 0, false);

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);

            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(201, 201, 179), CreateMapEntryName());
        }

        public override bool IsPoweredOnFrame(int i, int j) => Main.tile[i, j].TileFrameY >= 18;

        public override void OnToggleStateFrame(int i, int j, bool skipWire = false)
        {
            if (Main.tile[i, j].TileFrameY >= 18)
                Main.tile[i, j].TileFrameY -= 18;
            else
                Main.tile[i, j].TileFrameY += 18;

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, i, j, 1, 1);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            TileRendering.DrawTileExtraTexture(i, j, spriteBatch, glowmask, applyPaint: true, Color.White);
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY >= 18)
                frameYOffset = 18 * Main.tileFrame[type];
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            int ticksPerFrame = 15;
            int frameCount = 4;
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
            if (tile.TileFrameY > 18)
                tile.GetEmmitedLight(Color.Green, applyPaint: true, out r, out g, out b);
        }
    }
}