using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Luminite
{
    public class LuminiteSink : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.CountsAsWaterSource[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = [16, 16];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);

            foreach (LuminiteStyle style in Enum.GetValues(typeof(LuminiteStyle)))
                AddMapEntry(Utility.GetTileColorFromLuminiteStyle(style), Language.GetText("MapObject.Sink"));

            AdjTiles = [TileID.Sinks];
            DustType = DustID.LunarOre;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Utility.GetDustTypeFromLuminiteStyle((LuminiteStyle)(Main.tile[i, j].TileFrameY / (18 * 2)));
            return true;
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameY / (18 * 2));

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
