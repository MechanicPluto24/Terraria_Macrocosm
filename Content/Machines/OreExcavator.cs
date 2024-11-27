using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Systems.UI;
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

namespace Macrocosm.Content.Machines
{
    public class OreExcavator : MachineTile
    {
        public override short Width => 7;
        public override short Height => 10;
        public override MachineTE MachineTE => ModContent.GetInstance<OreExcavatorTE>();

        private static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;

            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16, 16, 16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleLineSkip = 6;
            TileObjectData.newTile.StyleWrapLimit = 6;

            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.newTile.Origin = new Point16(0, Height - 1);
            TileObjectData.newTile.AnchorTop = new AnchorData();
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, Width, 0);

            TileObjectData.newTile.AnchorInvalidTiles =
            [
                TileID.MagicalIceBlock,
                TileID.Boulder,
                TileID.BouncyBoulder,
                TileID.LifeCrystalBoulder,
                TileID.RollingCactus
            ];

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(MachineTE.Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.addTile(Type);

            AdjTiles = [TileID.Containers];
            DustType = -1;

            AddMapEntry(new Color(206, 117, 44), CreateMapEntryName());
        }

        public override bool IsPoweredOnFrame(int i, int j) => Main.tile[i, j].TileFrameY >= (Height * 18) * 1;

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

            Point16 origin = Utility.GetMultitileTopLeft(i, j);
            if ((i >= origin.X + 0 && i <= origin.X + 1) && (j >= origin.Y + 7 && j <= origin.Y + 8))
            {
                Toggle(i, j, automatic: false, skipWire: false);
            }
            else
            {
                if (Utility.TryGetTileEntityAs(i, j, out OreExcavatorTE oreExcavator))
                    UISystem.ShowMachineUI(oreExcavator, new OreExcavatorUI());
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!player.mouseInterface)
            {
                player.noThrow = 2;

                Point16 origin = Utility.GetMultitileTopLeft(i, j);
                if ((i >= origin.X + 0 && i <= origin.X + 1) && (j >= origin.Y + 7 && j <= origin.Y + 8))
                {
                    if (IsPoweredOnFrame(origin.X, origin.Y))
                        CursorIcon.Current = CursorIcon.MachineTurnOff;
                    else
                        CursorIcon.Current = CursorIcon.MachineTurnOn;
                }
                else
                {
                    CursorIcon.Current = CursorIcon.Drill;
                }
            }
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

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            Utility.DrawTileExtraTexture(i, j, spriteBatch, glowmask);
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
                if (tileOffsetX == 1 && tileOffsetY == 0)
                {
                    if (Main.tileFrame[Type] % 2 == 1)
                    {
                        float atmoDensity = (0.3f + 0.7f * MacrocosmSubworld.CurrentAtmosphericDensity);
                        int count = atmoDensity < 1f ? 1 : 2;
                        for (int s = 0; s < count; s++)
                        {
                            Smoke smoke = Particle.Create<Smoke>((p) =>
                            {
                                p.Position = new Vector2(i, j) * 16f + new Vector2(1f, 16f);
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

                // Anchor tile below drill - make hit tile dust and spawn smoke "dust"
                if (tileOffsetX is 2 or 3 && tileOffsetY == Height - 1)
                {
                    if (Main.tileFrame[Type] == 3)
                    {
                        if (Main.rand.NextBool()) WorldGen.KillTile(i, j + 1, effectOnly: true, fail: true);
                        if (Main.rand.NextBool()) WorldGen.KillTile(i, j + 2, effectOnly: true, fail: true);
                        if (Main.rand.NextBool()) WorldGen.KillTile(i + 1, j + 1, effectOnly: true, fail: true);
                        if (Main.rand.NextBool()) WorldGen.KillTile(i + 1, j + 2, effectOnly: true, fail: true);
                    }

                    if (Main.tileFrame[Type] % 2 == 0)
                    {
                        for (int s = 0; s < 2; s++)
                        {
                            Point hitTile = new(i + 1, j + 2 + s);
                            Smoke smoke = Particle.Create<Smoke>((p) =>
                            {
                                p.Position = hitTile.ToWorldCoordinates();
                                p.Velocity = new Vector2(Main.rand.NextFloat(-0.7f, 0.7f), Main.rand.NextFloat(-0.1f, -0.25f));
                                p.Scale = new(0.3f);
                                p.Rotation = 0f;
                                p.Color = Smoke.GetTileHitColor(hitTile);
                                p.FadeIn = true;
                                p.Opacity = 0f;
                                p.ScaleVelocity = new(0.0075f);
                            });
                        }
                    }
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            int tileOffsetX = tile.TileFrameX % (Width * 18) / 18;
            int tileOffsetY = tile.TileFrameY % (Height * 18) / 18;

            if (tileOffsetX is 2 && tileOffsetY is 6)
            {
                if (IsPoweredOnFrame(i, j))
                    g = 0.2f;
                else
                    r = 0.2f;
            }
        }
    }
}
