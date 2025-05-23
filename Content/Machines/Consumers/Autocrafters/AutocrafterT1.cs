using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Common.UI.Machines;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines.Consumers.Autocrafters
{
    public class AutocrafterT1 : MachineTile
    {
        public override short Width => 3;
        public override short Height => 2;
        public override MachineTE MachineTE => ModContent.GetInstance<AutocrafterT1TE>();

        private static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.DefaultToMachine(this);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, Width, 0);
            TileObjectData.addTile(Type);

            AdjTiles = [TileID.Containers];
            DustType = -1;

            AddMapEntry(new Color(49, 33, 19), CreateMapEntryName());
        }

        public override bool IsPoweredOnFrame(int i, int j) => Main.tile[i, j].TileFrameY >= Height * 18 * 1;
        public override void OnToggleStateFrame(int i, int j, bool skipWire = false)
        {
            Point16 origin = TileObjectData.TopLeft(i, j);
            for (int x = origin.X; x < origin.X + Width; x++)
            {
                for (int y = origin.Y; y < origin.Y + Height; y++)
                {
                    Tile tile = Main.tile[x, y];

                    if (IsPoweredOnFrame(x, y))
                        tile.TileFrameY -= (short)(Height * 18);
                    else
                        tile.TileFrameY += (short)(Height * 18);

                    if (skipWire && Wiring.running)
                        Wiring.SkipWire(x, y);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, origin.X, origin.Y, Width, Height);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Main.mouseRightRelease = false;
            Utility.UICloseOthers();
            if (TileEntity.TryGet(i, j, out MachineTE te))
                UISystem.ShowMachineUI(te, new AutocrafterUI());

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (!player.mouseInterface)
            {
                player.noThrow = 2;
                Main.LocalPlayer.cursorItemIconEnabled = true;
                player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (IsPoweredOnFrame(i, j))
                frameYOffset = 18 * Height * Main.tileFrame[type];
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            int ticksPerFrame = 5;
            int frameCount = 6;
            if (++frameCounter >= ticksPerFrame)
            {
                frameCounter = 0;
                if (++frame >= frameCount)
                    frame = 0;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            TileRendering.DrawTileExtraTexture(i, j, spriteBatch, glowmask, applyPaint: true, Color.White);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            int tileOffsetX = tile.TileFrameX % (Width * 18) / 18;
            int tileOffsetY = tile.TileFrameY % (Height * 18) / 18;

            if (tileOffsetX is 3 && tileOffsetY is 0)
            {
                if (IsPoweredOnFrame(i, j))
                    tile.GetEmmitedLight(new Color(0, 50, 0), applyPaint: true, out r, out g, out b);
                else
                    tile.GetEmmitedLight(new Color(50, 0, 0), applyPaint: true, out r, out g, out b);
            }
        }
    }
}
