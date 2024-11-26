using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines
{
    public class OilRefinery : MachineTile
    {
        public override short Width => 4;
        public override short Height => 4;
        public override MachineTE MachineTE => ModContent.GetInstance<OilRefineryTE>();

        public override bool IsPoweredOnFrame(int i, int j) => Main.tile[i, j].TileFrameY >= (Height * 18);

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleLineSkip = 5;
            TileObjectData.newTile.StyleWrapLimit = 5;

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(MachineTE.Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            DustType = -1;

            AddMapEntry(new Color(121, 107, 91), CreateMapEntryName());
        }

        public override void OnToggleStateFrame(int i, int j, bool skipWire = false)
        {
            Point16 origin = Utility.GetMultitileTopLeft(i, j);
            for (int x = origin.X; x < origin.X + Width; x++)
            {
                for (int y = origin.Y; y < origin.Y + Height; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (IsPoweredOnFrame(x, y))
                        tile.TileFrameY -= (short)(Height * 18);
                    else
                        tile.TileFrameY += (short)(Height * 18);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, origin.X, origin.Y, Width, Height);
        }

        public override bool RightClick(int i, int j)
        {
            Main.mouseRightRelease = false;
            Utility.UICloseOthers();

            if (Utility.TryGetTileEntityAs(i, j, out OilRefineryTE refinery))
            {
                UISystem.ShowMachineUI(refinery, new OilRefineryUI());
            }

            return true;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (IsPoweredOnFrame(i, j))
                frameYOffset = 18 * Height * Main.tileFrame[type];
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            int ticksPerFrame = 4;
            int frameCount = 4;
            if (++frameCounter >= ticksPerFrame)
            {
                frameCounter = 0;
                if (++frame >= frameCount)
                    frame = 0;
            }
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

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.gamePaused)
                return;

            Tile tile = Main.tile[i, j];
            int tileOffsetX = tile.TileFrameX % (Width * 18) / 18;
            int tileOffsetY = tile.TileFrameY % (Height * 18) / 18;

            if (IsPoweredOnFrame(i, j))
            {
                // Exhaust position - spawn smoke
                if (tileOffsetX == 0 && tileOffsetY == 0)
                {
                    if (Main.tileFrame[Type] % 2 == 1)
                    {
                        float atmoDensity = (0.3f + 0.7f * MacrocosmSubworld.CurrentAtmosphericDensity);
                        int count = atmoDensity < 1f ? 1 : 2;
                        for (int s = 0; s < count; s++)
                        {
                            Smoke smoke = Particle.Create<Smoke>((p) =>
                            {
                                p.Position = new Vector2(i, j) * 16f + new Vector2(15f, 2f);
                                p.Velocity = new Vector2(0, -1.1f).RotatedByRandom(MathHelper.Pi / 16) * atmoDensity;
                                p.Scale = new(0.3f);
                                p.Rotation = 0f;
                                p.Color = (new Color(80, 80, 80) * Main.rand.NextFloat(0.75f, 1f)).WithAlpha(215);
                                p.FadeIn = true;
                                p.Opacity = 0f;
                                p.ScaleVelocity = new(0.0075f);
                                p.WindFactor = 0.01f;
                            });
                        }
                    }
                }
            }
        }
    }
}
