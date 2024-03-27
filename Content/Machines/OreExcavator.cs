using Macrocosm.Common.Bases.Machines;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            TileObjectData.newTile.CoordinatePadding = 0; // No padding, in order to make animating the sprite easier

            TileObjectData.newTile.Origin = new Point16(0, Height - 1);
            TileObjectData.newTile.AnchorTop = new AnchorData();
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, Width, 0);

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

            AdjTiles = [TileID.Containers];
            DustType = -1;

            AddMapEntry(new Color(206, 117, 44), CreateMapEntryName());
        }

        public override void HitWire(int i, int j)
        {
            Point16 origin = Utility.GetMultitileTopLeft(i, j);
            for (int x = origin.X; x < origin.X + Width; x++)
            {
                for (int y = origin.Y; y < origin.Y + Height; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.TileFrameY >= Height * 16)
                        tile.TileFrameY -= (short)(Height * 16);
                    else
                        tile.TileFrameY += (short)(Height * 16);

                    if (Wiring.running)
                        Wiring.SkipWire(x, y);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, origin.X, origin.Y, Width, Height);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Chest.DestroyChest(i, j);
            ModContent.GetInstance<OreExcavatorTE>().Kill(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            Main.mouseRightRelease = false;
            Utility.UICloseOthers();

            Point16 origin = Utility.GetMultitileTopLeft(i, j);
            if (i == origin.X + 4 && j == origin.Y + 5)
            {
                for (int x = origin.X; x < origin.X + Width; x++)
                {
                    for (int y = origin.Y; y < origin.Y + Height; y++)
                    {
                        Tile tile = Main.tile[x, y];
                        if (tile.TileFrameY >= Height * 16)
                            tile.TileFrameY -= (short)(Height * 16);
                        else
                            tile.TileFrameY += (short)(Height * 16);
                    }
                }

            }
            else
            {
                if (Utility.TryGetTileEntityAs(i, j, out OreExcavatorTE oreExcavator))
                {
                    UISystem.ShowMachineUI(oreExcavator, new OreExcavatorUI());
                }
            }

            return true;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (Main.tile[i, j].TileFrameY >= 16 * Height)
                frameYOffset = 16 * Height * Main.tileFrame[type];
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 4)
            {
                frameCounter = 0;
                if (++frame >= 8)
                    frame = 0;
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Tile tile = Main.tile[i, j];

            int tileOffsetX = tile.TileFrameX % (Width * 16) / 16;
            int tileOffsetY = tile.TileFrameY % (Height * 16) / 16;

            if (Main.tile[i, j].TileFrameY >= 16 * Height)
            {
                // Exhaust position - spawn smoke
                if (tileOffsetX == 1 && tileOffsetY == 0 && !Main.gamePaused)
                {
                    if (Main.tileFrame[Type] % 2 == 0)
                    {
                        for (int p = 0; p < 1; p++)
                        {
                            Smoke smoke = Particle.CreateParticle<Smoke>(new Vector2(i, j) * 16f + new Vector2(0, 4f), new Vector2(0, -1.4f).RotatedByRandom(MathHelper.Pi / 8), 1f, 0f);
                            smoke.Color = new Color(80, 80, 80, 215);
                            smoke.FadeIn = true;
                            smoke.Opacity = 0f;
                        }
                    }
                }

                // Anchor tile below drill - make hit tile dust
                if (tileOffsetX is 2 or 3 && tileOffsetY == 9)
                {
                    if (Main.tileFrame[Type] is 6 or 7)
                    {
                        if (Main.rand.NextBool(2))
                            WorldGen.KillTile(i, j + 1, effectOnly: true, fail: true);

                        if (Main.rand.NextBool(3))
                            WorldGen.KillTile(i, j + 2, effectOnly: true, fail: true);
                    }
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
        }
    }
}
