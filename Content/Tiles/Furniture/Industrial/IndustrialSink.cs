using Macrocosm.Common.Sets;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Industrial
{
    public class IndustrialSink : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.CountsAsWaterSource[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.DrawYOffset = 0;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Sink"));

            AdjTiles = [TileID.Sinks];
            DustType = ModContent.DustType<IndustrialPlatingDust>();

            TileSets.RandomStyles[Type] = 2;

            // All styles
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.Industrial.IndustrialSink>());
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
