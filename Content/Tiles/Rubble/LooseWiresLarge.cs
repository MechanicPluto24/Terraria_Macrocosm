using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Rubble
{
    public abstract class LooseWiresLarge : ModTile
    {
        // We want both tiles to use the same texture
        public override string Texture => this.GetNamespacePath().Replace(Name, nameof(LooseWiresLarge));

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileID.Sets.MultiTileSway[Type] = true;

            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = [16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.addTile(Type);

            DustType = 50;
            AddMapEntry(new Color(136, 136, 136));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = j % 2 == 0 ? 48 : 50;
            return true;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (TileObjectData.IsTopLeft(tile))
                TileRendering.AddCustomSpecialPoint(i, j, CustomSpecialDraw);

            return false; // We must return false here to prevent the normal tile drawing code from drawing the default static tile. Without this a duplicate tile will be drawn.
        }

        public void CustomSpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileRendering.DrawMultiTileInWindTopAnchor(i, j);
        }
    }

    // This is the natural tile, this version is placed during world generation
    public class LooseWiresLargeNatural : LooseWiresLarge
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }

    // This is the fake tile that will be placed by the Rubblemaker.
    public class LooseWiresLargeFake : LooseWiresLarge
    {
        public override void SetStaticDefaults()
        {
            // Call to base SetStaticDefaults. Must inherit static defaults from base type 
            base.SetStaticDefaults();

            // Add rubble variant, all existing styles, to Rubblemaker, allowing to place this tile by consuming Wire
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ItemID.Wire, Type, 0, 1, 2);

            // Tiles placed by Rubblemaker drop the item used to place them.
            RegisterItemDrop(ItemID.Wire);
        }
    }

}