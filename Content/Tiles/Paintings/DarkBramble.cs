using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Paintings;

public class DarkBramble : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = false;
        Main.tileSolidTop[Type] = false;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileTable[Type] = false;
        Main.tileLavaDeath[Type] = true;

        TileObjectData.newTile.DefaultToPainting(4, 4);
        TileObjectData.addTile(Type);

        TileID.Sets.DisableSmartCursor[Type] = true;
        AddMapEntry(new Color(45, 49, 37), Language.GetText("Painting"));

        DustType = -1;
    }
}