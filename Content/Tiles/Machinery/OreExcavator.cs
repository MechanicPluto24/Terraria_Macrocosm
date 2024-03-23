using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Machinery
{
	public class OreExcavator : ModTile
	{
        private const int Width = 7;
        private const int Height = 10;

        public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileWaterDeath[Type] = true;
			Main.tileLavaDeath[Type] = true;

            AnimationFrameHeight = 16 * 10;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;

            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16, 16, 16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 0; // No padding, in order to make animating the sprite easier

            TileObjectData.newTile.Origin = new Point16(0, 9);
			TileObjectData.newTile.AnchorTop = new AnchorData();
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, Width, 0);

			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;

			TileObjectData.addTile(Type);

            DustType = -1;

			AddMapEntry(new Color(206, 117, 44), CreateMapEntryName());
		}

        public override void HitWire(int i, int j)
        {
            int leftX = i - Main.tile[i, j].TileFrameX / 18 % Width;
            int topY = j - Main.tile[i, j].TileFrameY / 18 % Height;

            for (int x = leftX; x < leftX + Width; x++)
            {
                for (int y = topY; y < topY + Height; y++)
                {
                    Tile tile = Main.tile[x, y];
                    // turn on/off
                    if (Wiring.running)
                         Wiring.SkipWire(x, y);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, leftX, topY, Width, Height);
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameYOffset = AnimationFrameHeight * Main.tileFrame[type];
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

            // Exhaust position 
            if (tileOffsetX == 1 && tileOffsetY == 0 && !Main.gamePaused)
            {
                if(Main.tileFrame[Type] % 2 == 0)
                {
                    for(int p = 0; p < 1; p++)
                    {
                        Smoke smoke = Particle.CreateParticle<Smoke>(new Vector2(i, j) * 16f + new Vector2(0, 4f), new Vector2(0, -1.4f).RotatedByRandom(MathHelper.Pi / 8), 1f, 0f);
                        smoke.Color = new Color(80, 80, 80, 215);
                        smoke.FadeIn = true;
                        smoke.Opacity = 0f;
                    }
                }
            }

            if (tileOffsetX is 2 or 3 && tileOffsetY == 9)
            {
                if (Main.tileFrame[Type] is 6 or 7)
                {
                    WorldGen.KillTile(i, j + 1, effectOnly: true, fail: true);
                }
            }


            base.DrawEffects(i, j, spriteBatch, ref drawData);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
		}
	}
}
