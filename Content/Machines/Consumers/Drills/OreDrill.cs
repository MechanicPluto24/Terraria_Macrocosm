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

namespace Macrocosm.Content.Machines.Consumers.Drills
{
    public class OreDrill : MachineTile
    {
        public override short Width => 4;
        public override short Height => 6;
        public override MachineTE MachineTE => ModContent.GetInstance<OreDrillTE>();

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            SceneData.Hooks[Type] = NearbyEffects;

            TileObjectData.newTile.DefaultToMachine(this);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, Width, 0);
            TileObjectData.addTile(Type);

            AdjTiles = [TileID.Containers];
            DustType = -1;

            AddMapEntry(new Color(206, 117, 44), CreateMapEntryName());
        }

        // Frame 0 => idle, Frame 1 => active
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
                UISystem.ShowMachineUI(te, new DrillUI());
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!player.mouseInterface)
            {
                player.noThrow = 2;
                CursorIcon.Current = CursorIcon.Drill;
            }
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

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (IsPoweredOnFrame(i, j))
                frameYOffset = 18 * Height * Main.tileFrame[type] * -1; // Negative because "idle" frame is also an animation frame
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            int ticksPerFrame = 8;
            int frameCount = 2;
            if (++frameCounter >= ticksPerFrame)
            {
                frameCounter = 0;
                if (++frame >= frameCount)
                    frame = 0;
            }
        }

        public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
        {
            if (!visible)
                return;

            int tileOffsetX = tileFrameX % (Width * 18) / 18;
            int tileOffsetY = tileFrameY % (Height * 18) / 18;

            if (IsPoweredOnFrame(i, j))
            {
                // Exhaust position - spawn smoke
                if (tileOffsetY == 0 && tileOffsetX % 2 == 0)
                {
                    if (Main.tileFrame[Type] == 1)
                    {
                        float atmoDensity = 0.3f + 0.7f * MacrocosmSubworld.GetAtmosphericDensity(new Vector2(i, j) * 16f);
                        int count = atmoDensity < 1f ? 1 : 2;
                        for (int s = 0; s < count; s++)
                        {
                            Smoke smoke = Particle.Create<Smoke>((p) =>
                            {
                                p.Position = new Vector2(i, j) * 16f + new Vector2(18f, 16f);
                                p.Velocity = new Vector2(0, -1.1f).RotatedByRandom(MathHelper.Pi / 16) * atmoDensity;
                                p.Scale = new(0.3f);
                                p.Rotation = 0f;
                                p.Color = (new Color(80, 80, 80) * Main.rand.NextFloat(0.75f, 1f)).WithAlpha(215);
                                p.VanillaUpdate = true;
                                p.Opacity = 0f;
                                p.ScaleVelocity = new(0.0075f);
                                p.WindFactor = 0.01f;
                            });
                        }
                    }
                }

                // Anchor tile below drill - make hit tile dust and spawn smoke "dust"
                if (tileOffsetX == 2 && tileOffsetY == Height - 1)
                {
                    if (Main.tileFrame[Type] == 1)
                    {
                        if (Main.rand.NextBool(4)) 
                            WorldGen.KillTile(i, j + 1, effectOnly: true, fail: true);
                    }
                    else if (Main.tileFrame[Type] == 0)
                    {
                        for (int s = 0; s < 1; s++)
                        {
                            Point hitTile = new(i + 1, j + 2 + s);
                            Smoke smoke = Particle.Create<Smoke>((p) =>
                            {
                                p.Position = hitTile.ToWorldCoordinates();
                                p.Velocity = new Vector2(Main.rand.NextFloat(-0.7f, 0.7f), Main.rand.NextFloat(-0.1f, -0.25f));
                                p.Scale = new(0.3f);
                                p.Rotation = 0f;
                                p.Color = Smoke.GetTileHitColor(hitTile);
                                p.VanillaUpdate = true;
                                p.Opacity = 0f;
                                p.ScaleVelocity = new(0.0075f);
                            });
                        }
                    }
                }
            }
        }
    }
}
