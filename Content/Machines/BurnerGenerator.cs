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
    public class BurnerGenerator : MachineTile
    {
        public override short Width => 4;
        public override short Height => 3;
        public override MachineTE MachineTE => ModContent.GetInstance<BurnerGeneratorTE>();

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

            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;  

            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.newTile.Origin = new Point16(0, Height - 1);
            TileObjectData.newTile.AnchorTop = new AnchorData();
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, Width, 0);

            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;

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

            DustType = -1;

            AddMapEntry(new Color(139, 75, 14), CreateMapEntryName());
        }

        public override bool IsPoweredOnFrame(int i, int j) => Main.tile[i, j].TileFrameY >= (Height * 18);

        public override void TogglePowerStateFrame(int i, int j)
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

        public override void HitWire(int i, int j)
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

                    if (Wiring.running)
                        Wiring.SkipWire(x, y);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, origin.X, origin.Y, Width, Height);
        }

        public override bool RightClick(int i, int j)
        {
            /*
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

            */

            if (Utility.TryGetTileEntityAs(i, j, out BurnerGeneratorTE burnerGenerator))
                UISystem.ShowMachineUI(burnerGenerator, new BurnerGeneratorUI());

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!UISystem.Active && !player.mouseInterface)
            {
                player.noThrow = 2;

                Main.LocalPlayer.cursorItemIconEnabled = true;
                Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<Items.Machines.BurnerGenerator>();
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
            int frameCount = 6;
            if (++frameCounter >= ticksPerFrame)
            {
                frameCounter = 0;
                if (++frame >= frameCount)
                    frame = 0;
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
                    {
                        float atmoDensity = (0.3f + 0.7f * MacrocosmSubworld.CurrentAtmosphericDensity);
                        if(true)
                        {
                            Smoke smoke = Particle.CreateParticle<Smoke>((p) =>
                            {
                                p.Position = new Vector2(i, j) * 16f + new Vector2(Main.rand.NextBool() ? 4 : 12, 8f);
                                p.Velocity = new Vector2(0, -1.1f).RotatedByRandom(MathHelper.Pi / 16) * atmoDensity;
                                p.Scale = 0.3f;
                                p.Rotation = 0f;
                                p.DrawColor = (new Color(80, 80, 80) * Main.rand.NextFloat(0.75f, 1f)).WithAlpha(215);
                                p.FadeIn = true;
                                p.Opacity = 0f;
                                p.ExpansionRate = 0.0075f;
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

            if (tileOffsetX is 0 or 1 && tileOffsetY is 2)
            {
                if (IsPoweredOnFrame(i, j))
                {
                    r = 0.75f;
                    g = 0.5f;
                }
            }
        }
    }
}
