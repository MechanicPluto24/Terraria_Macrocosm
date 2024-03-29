using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Banners
{
    public class MercurianBanner : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            //TileID.Sets.SwaysInWindBasic[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 6;

            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 3, 0);

            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;

            TileObjectData.addTile(Type);

            DustType = ModContent.DustType<DianiteDust>(); // placeholder

            RegisterItemDrop(ModContent.ItemType<Items.Placeable.Banners.MercurianBanner>(), 0, 1);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(251, 105, 15), name);
        }
    }
}
