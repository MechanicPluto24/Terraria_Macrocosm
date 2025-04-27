using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
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
    public class AutocrafterT3 : MachineTile
    {
        public override short Width => 4;
        public override short Height => 3;
        public override MachineTE MachineTE => ModContent.GetInstance<AutocrafterT3TE>();

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

            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.newTile.Origin = new Point16(0, Height - 1);
            TileObjectData.newTile.AnchorTop = new AnchorData();
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

            AdjTiles = [TileID.Containers];
            DustType = -1;

            AddMapEntry(new Color(149, 149, 149), CreateMapEntryName());
        }

        public override bool IsPoweredOnFrame(int i, int j) => Main.tile[i, j].TileFrameY >= Height * 18 * 1;
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
            if (Utility.TryGetTileEntityAs(i, j, out AutocrafterT3TE autocrafter))
                UISystem.ShowMachineUI(autocrafter, new AutocrafterUI());

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
            int frameCount = 73;
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

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.gamePaused)
                return;

            if (!TileObjectData.IsTopLeft(i, j))
                return;

            Tile tile = Main.tile[i, j];
            int tileOffsetX = tile.TileFrameX % (Width * 18) / 18;
            int tileOffsetY = tile.TileFrameY % (Height * 18) / 18;
            int frame = Main.tileFrame[Type] + 1;
            bool powered = IsPoweredOnFrame(i, j);

            if (powered && WeldingSparkFrame(frame))
            {
                float intensity = frame % 2 == (frame >= 49 ? 0 : 1) ? 0f : 1f;
                Vector2 position = new Vector2(i, j) * 16f + new Vector2(31f, 32f);
                if (frame is >= 34 and <= 38) position.X += 4f;
                if (frame is >= 49 and <= 54) position.X += 2f;

                for (int k = 0; k < (int)(12f * intensity); k++)
                {
                    Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<ElectricSparkDust>(), Main.rand.NextVector2Circular(1f, 1f), Scale: Main.rand.NextFloat(0.1f, 0.3f));
                    dust.noGravity = false;
                    dust.color = new Color(112, 179, 218) * 0.6f;
                    dust.alpha = Main.rand.Next(10);
                }

                for (int k = 0; k < (int)(4f * intensity); k++)
                {
                    Particle.Create<LightningParticle>((p) =>
                    {
                        p.Position = position;
                        p.Velocity = Main.rand.NextVector2Circular(2f, 2f);
                        p.Scale = new(Main.rand.NextFloat(0.1f, 0.15f));
                        p.FadeOutNormalizedTime = 0.5f;
                        p.Color = new Color(112, 179, 218).WithAlpha((byte)Main.rand.Next(0, 64));
                        p.OutlineColor = new Color(112, 179, 218) * 0.2f;
                    });
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            int tileOffsetX = tile.TileFrameX % (Width * 18) / 18;
            int tileOffsetY = tile.TileFrameY % (Height * 18) / 18;
            int frame = Main.tileFrame[Type] + 1;
            bool powered = IsPoweredOnFrame(i, j);

            // Power LED light
            if (tileOffsetX == 4 && tileOffsetY == 1)
            {
                // Power LED
                Color ledColor = powered ? new Color(0, 50, 0) : new Color(50, 0, 0);
                tile.GetEmmitedLight(ledColor, applyPaint: true, out r, out g, out b);
            }

            // Screen green light
            if (tileOffsetX is 1 or 2 && tileOffsetY is 0 or 1)
            {
                if (powered && frame is not (0 or 42))
                    tile.GetEmmitedLight(new Color(0, 75, 0), applyPaint: true, out r, out g, out b);
            }

            // Welding sparks light
            if (tileOffsetX is 1 or 2 && tileOffsetY is 1)
            {
                if (powered && WeldingSparkFrame(frame))
                {
                    // Alternating intensity
                    float intensity = frame % 2 == (frame >= 49 ? 0 : 1) ? 0.4f : 0.5f;
                    tile.GetEmmitedLight(new Color(112, 179, 218) * intensity, applyPaint: false, out r, out g, out b);
                }
            }
        }

        private bool WeldingSparkFrame(int frame) => frame is >= 18 and <= 22 || frame is >= 34 and <= 38 || frame is >= 49 and <= 54;
    }
}
