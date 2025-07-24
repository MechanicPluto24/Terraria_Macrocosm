using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Paintings;

public class SakuraRuckenfigur : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = false;
        Main.tileSolidTop[Type] = false;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileTable[Type] = false;
        Main.tileLavaDeath[Type] = true;

        TileObjectData.newTile.DefaultToPainting(3, 5);
        TileObjectData.addTile(Type);

        TileID.Sets.DisableSmartCursor[Type] = true;
        AddMapEntry(new Color(89, 74, 60), Language.GetText("Painting"));

        DustType = -1;
    }
}