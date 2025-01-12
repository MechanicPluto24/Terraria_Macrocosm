using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines
{
    public class OxygenSystem : MachineTile
    {
        private static Asset<Texture2D> glowmask;

        public override short Width => 4;
        public override short Height => 3;
        public override MachineTE MachineTE => ModContent.GetInstance<OxygenSystemTE>();

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;

            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.StyleLineSkip = 3;
            TileObjectData.newTile.StyleWrapLimit = 3;

            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, Width, 0);

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(MachineTE.Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.addTile(Type);

            DustType = ModContent.DustType<IndustrialPlatingDust>();

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(253, 221, 3), name);
        }

        public override bool IsPoweredOnFrame(int i, int j) => Main.tile[i, j].TileFrameX >= Width * 18;
        public override void OnToggleStateFrame(int i, int j, bool skipWire = false)
        {
            int leftX = i - Main.tile[i, j].TileFrameX / 18 % Width;
            int topY = j - Main.tile[i, j].TileFrameY / 18 % Height;

            for (int x = leftX; x < leftX + Width; x++)
            {
                for (int y = topY; y < topY + Height; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.TileFrameX >= Width * 18)
                        tile.TileFrameX -= (short)(Width * 18);
                    else
                        tile.TileFrameX += (short)(Width * 18);

                    if (skipWire && Wiring.running)
                        Wiring.SkipWire(x, y);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, leftX, topY, Width, Height);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            Utility.DrawTileExtraTexture(i, j, spriteBatch, glowmask, applyPaint: true);
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX >= Width * 18)
            {
                frameXOffset = Width * 18 * Main.tileFrame[type];
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            int ticksPerFrame = 60;
            int frameCount = 2;
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
            if (tile.TileFrameX >= 72)
            {
                float mult = 0.37f + 0.03f * (Main.tileFrame[Type] + 1);
                tile.GetEmmitedLight(new Color(160, 100, 75) * mult, applyPaint: true, out r, out g, out b);
            }
        }
    }
}
