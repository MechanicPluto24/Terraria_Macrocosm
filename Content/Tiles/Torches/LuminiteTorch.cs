using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Torches
{
    public class LuminiteTorch : ModTile
    {
        private static Asset<Texture2D> flameTexture;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileWaterDeath[Type] = true;

            TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.DisableSmartInteract[Type] = true;
            TileID.Sets.Torch[Type] = true;

            DustType = ModContent.DustType<LuminiteBrightDust>();
            AdjTiles = [TileID.Torches];

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Torches, 0));
            /*  This is what is copied from the Torches tile
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new[] { 124, 561, 574, 575, 576, 577, 578 };
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new[] { 124, 561, 574, 575, 576, 577, 578 };
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.addAlternate(0);
			*/

            TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
            TileObjectData.newSubTile.LinkedAlternates = true;
            TileObjectData.newSubTile.WaterDeath = false;
            TileObjectData.newSubTile.LavaDeath = false;
            TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.addSubTile(0);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(100, 255, 166), Language.GetText("ItemName.Torch"));

            flameTexture ??= ModContent.Request<Texture2D>(Texture + "_Flame");
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
        }

        public override float GetTorchLuck(Player player) => player.InModBiome<MoonBiome>() ? 1f : -0.5f;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(5, 10);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];

            if (tile.TileFrameX < 66)
            {
                int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
                if (style == 0)
                {
                    Vector3 color = new Color(105, 255, 145).ToVector3();
                    r = color.X;
                    g = color.Y;
                    b = color.Z;
                }
            }
        }

        // This code slightly lowers the draw position if there is a solid tile above, so the flame doesn't overlap that tile. Terraria torches do this same logic.
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 0;
            if (WorldGen.SolidTile(i, j - 1))
                offsetY = 4;
        }

        // The following code draws multiple flames on top our placed torch.
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var tile = Main.tile[i, j];

            if (!TileDrawing.IsVisible(tile))
                return;

            int offsetY = 0;
            if (WorldGen.SolidTile(i, j - 1))
                offsetY = 4;

            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
                zero = Vector2.Zero;

            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i); // Don't remove any casts.
            int width = 20;
            int height = 20;
            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;

            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            Color color = new Color(105, 255, 145, 0);

            for (int k = 0; k < 7; k++)
            {
                float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
                spriteBatch.Draw(flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx, j * 16 - (int)Main.screenPosition.Y + offsetY + yy) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}