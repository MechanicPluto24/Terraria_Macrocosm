using Macrocosm.Common.Bases.Tiles;
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
    public class KeroseneGenerator : MachineTile
    {
        public override short Width => 6;
        public override short Height => 4;
        public override MachineTE MachineTE => ModContent.GetInstance<KeroseneGeneratorTE>();

        private static Asset<Texture2D> extra;
        private float vibrationY;

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

            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.StyleHorizontal = true;
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

            DustType = -1;

            AddMapEntry(new Color(145, 80, 20), CreateMapEntryName());
        }


        public override bool IsPoweredOnFrame(int i, int j) => Main.tile[i, j].TileFrameY >= (Height * 18);

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

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (frameCounter++ >= 2)
            {
                frameCounter = 0;
                if (frame++ >= 1)
                {
                    frame = 0;
                }
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Tile tile = Main.tile[i, j];
            if (IsPoweredOnFrame(i,j) && tile.TileFrameX % (18 * Width) == 0 && tile.TileFrameY % (18 * Height) == 0)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);

            if (Main.gamePaused)
                return;

            int tileOffsetX = tile.TileFrameX % (Width * 18) / 18;
            int tileOffsetY = tile.TileFrameY % (Height * 18) / 18;
            if (Utility.TryGetTileEntityAs(i, j, out KeroseneGeneratorTE keroseneGenerator))
            {
                if (keroseneGenerator.PoweredOn)
                {
                    // Exhaust position - spawn smoke
                    if (tileOffsetX == 0 && tileOffsetY == 0)
                    {
                        float atmoDensity = (0.3f + 0.7f * MacrocosmSubworld.CurrentAtmosphericDensity);
                        for (int k = 0; k < 4; k++)
                        {
                            if (Main.rand.NextBool())
                                continue;

                            Smoke smoke = Particle.Create<Smoke>((p) =>
                            {
                                p.Position = new Vector2(i + 2, j) * 16f + new Vector2(8, 14f);
                                p.Velocity = new Vector2(-1.4f, -0.7f).RotatedByRandom(MathHelper.Pi / 16) * atmoDensity;
                                p.Acceleration = new Vector2(0f, -0.02f * atmoDensity);
                                p.Scale = new(0.2f);
                                p.Rotation = 0f;
                                p.Color = (new Color(80, 80, 80) * Main.rand.NextFloat(0.75f, 1f)).WithAlpha(215);
                                p.FadeIn = true;
                                p.Opacity = 0.4f;
                                p.ScaleVelocity = new(0.0075f);
                                p.WindFactor = 0.01f;
                            });
                        }
                    }
                }
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            extra ??= ModContent.Request<Texture2D>(Texture + "_Extra");
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 position = new Vector2(i, j) * 16f - Main.screenPosition + zero + new Vector2(6, 6 + Main.tileFrame[Type]);
            spriteBatch.Draw(extra.Value, position, null, Lighting.GetColor(i, j));
        }

        public override bool RightClick(int i, int j)
        {
            Main.mouseRightRelease = false;
            Utility.UICloseOthers();

            if (Utility.TryGetTileEntityAs(i, j, out KeroseneGeneratorTE keroseneGenerator))
                UISystem.ShowMachineUI(keroseneGenerator, new KeroseneGeneratorUI());

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!player.mouseInterface)
            {
                player.noThrow = 2;
                Point16 origin = Utility.GetMultitileTopLeft(i, j);
                Main.LocalPlayer.cursorItemIconEnabled = true;
                player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));

            }
        }
    }
}
