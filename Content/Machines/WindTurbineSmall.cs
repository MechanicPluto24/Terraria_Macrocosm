using Macrocosm.Common.Drawing;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines
{
    public class WindTurbineSmall : MachineTile
    {
        public override short Width => 3;
        public override short Height => 7;

        public override MachineTE MachineTE => ModContent.GetInstance<WindTurbineSmallTE>();

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.Origin = new Point16(1, Height - 1);

            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = true;

            TileObjectData.newTile.Origin = new Point16(1, Height - 1);
            TileObjectData.newTile.AnchorTop = new AnchorData();
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, Width, 0);

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(MachineTE.Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.addTile(Type);

            DustType = -1;
            AddMapEntry(new Color(134, 137, 139), CreateMapEntryName());
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            var topLeft = TileObjectData.TopLeft(i, j);
            if (WorldGen.InAPlaceWithWind(topLeft.X, topLeft.Y, Width, Height))
                frameYOffset = 18 * Height * Main.tileFrame[type];
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            int ticksPerFrame = Math.Clamp((int)(10 * (1f - Math.Abs(Utility.WindSpeedScaled))), 2, 6);

            int frameCount = 9;

            if (Math.Abs(Utility.WindSpeedScaled) > 0.1f)
            {
                if (++frameCounter >= ticksPerFrame)
                {
                    frameCounter = 0;

                    if (Utility.WindSpeedScaled > 0f)
                        frame++;
                    else
                        frame--;

                    if (frame >= frameCount)
                        frame = 0;

                    if (frame < 0)
                        frame = frameCount - 1;
                }
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (TileObjectData.IsTopLeft(i, j))
                Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);

            return false;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileRendering.DrawMultiTileGrass(i, j, totalWindMultiplier: 0.04f, rowsToIgnore: 1, perTileLighting: false);
        }
    }
}
