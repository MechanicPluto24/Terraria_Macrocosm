using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
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

namespace Macrocosm.Content.Machines.Generators.Fuel
{
    public class KeroseneGenerator : MachineTile
    {
        public override short Width => 6;
        public override short Height => 4;
        public override MachineTE MachineTE => ModContent.GetInstance<KeroseneGeneratorTE>();

        private static Asset<Texture2D> extra;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            SceneData.Hooks[Type] = NearbyEffects;


            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.Origin = new Point16(0, Height - 1);

            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = true;

            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, Width, 0);
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


        public override bool IsPoweredOnFrame(int i, int j) => Main.tile[i, j].TileFrameY >= Height * 18;

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

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
                return;

            if (TileObjectData.IsTopLeft(i, j) && IsPoweredOnFrame(i, j))
                TileCounts.Instance.PollutionLevel += 5f;
        }

        public void NearbyEffects(int i, int j, SceneData sceneData)
        {
            if (TileObjectData.IsTopLeft(i, j) && IsPoweredOnFrame(i, j))
                sceneData.Macrocosm.PollutionLevel += 5f;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (frameCounter++ >= 6)
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
            if (Main.gamePaused)
                return;

            int tileOffsetX = tile.TileFrameX % (Width * 18) / 18;
            int tileOffsetY = tile.TileFrameY % (Height * 18) / 18;
            if (Utility.TryGetTileEntityAs(i, j, out MachineTE te))
            {
                if (te.PoweredOn)
                {
                    // Exhaust position - spawn smoke
                    if (tileOffsetX == 0 && tileOffsetY == 0)
                    {
                        float atmoDensity = 0.3f + 0.7f * MacrocosmSubworld.GetAtmosphericDensity(new Vector2(i, j) * 16f);
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
                                p.VanillaUpdate = true;
                                p.Opacity = 0.4f;
                                p.ScaleVelocity = new(0.0075f);
                                p.WindFactor = Main.windSpeedCurrent > 0 ? 0.035f : 0.01f;
                            });
                        }
                    }
                }
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (IsPoweredOnFrame(i, j))
            {
                extra ??= ModContent.Request<Texture2D>(Texture + "_Extra");
                Vector2 offset = new(0, Main.tileFrame[Type] * 2);
                TileRendering.DrawTileExtraTexture(i, j, spriteBatch, extra, applyPaint: true, drawOffset: offset);
            }
        }

        public override bool RightClick(int i, int j)
        {
            Main.mouseRightRelease = false;
            Utility.UICloseOthers();

            if (Utility.TryGetTileEntityAs(i, j, out MachineTE te))
                UISystem.ShowMachineUI(te, new KeroseneGeneratorUI());

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
    }
}
