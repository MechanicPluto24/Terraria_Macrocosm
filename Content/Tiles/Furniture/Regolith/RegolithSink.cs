using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Regolith;

public class RegolithSink : ModTile
{
    public override void SetStaticDefaults()
    {
        TileID.Sets.CountsAsWaterSource[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.addTile(Type);

        AdjTiles = [TileID.Sinks];
        DustType = ModContent.DustType<RegolithDust>();
        AddMapEntry(new(201, 201, 204), Language.GetText("MapObject.Sink"));
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}
