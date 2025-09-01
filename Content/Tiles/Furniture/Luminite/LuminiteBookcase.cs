using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Luminite;

public class LuminiteBookcase : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolidTop[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);

        HitSound = SoundID.Dig;
        DustType = DustID.LunarOre;

        foreach (LuminiteStyle style in Enum.GetValues(typeof(LuminiteStyle)))
            AddMapEntry(Utility.GetTileColorFromLuminiteStyle(style), Language.GetText("ItemName.Bookcase"));
    }

    public override bool CreateDust(int i, int j, ref int type)
    {
        type = Utility.GetDustTypeFromLuminiteStyle((LuminiteStyle)(Main.tile[i, j].TileFrameX / (18 * 3)));
        return true;
    }

    public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / (18 * 3));
}
