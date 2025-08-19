using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Luminite;

public class LuminiteTable : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileTable[Type] = true;
        Main.tileSolidTop[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.IgnoredByNpcStepUp[Type] = true;

        DustType = DustID.LunarOre;
        AdjTiles = [TileID.Tables];

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.CoordinateHeights = [16, 16];
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

        foreach (LuminiteStyle style in Enum.GetValues(typeof(LuminiteStyle)))
            AddMapEntry(Utility.GetTileColorFromLuminiteStyle(style), Language.GetText("MapObject.Table"));
    }

    public override bool CreateDust(int i, int j, ref int type)
    {
        type = Utility.GetDustTypeFromLuminiteStyle((LuminiteStyle)(Main.tile[i, j].TileFrameX / (18 * 3)));
        return true;
    }

    public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / (18 * 3));

    public override void NumDust(int x, int y, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }
}