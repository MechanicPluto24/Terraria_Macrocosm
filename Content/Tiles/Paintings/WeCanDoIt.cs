using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Paintings
{
    public class WeCanDoIt : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = false;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.addTile(Type);

            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(21, 136, 255), Language.GetText("Mods.Macrocosm.Common.MapNames.Poster"));
        }
    }
}