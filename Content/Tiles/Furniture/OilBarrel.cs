using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture;

public class OilBarrel : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.CoordinateHeights = [16, 16];
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);

        DustType = -1;
        AddMapEntry(new Color(40, 166, 209), CreateMapEntryName());
    }
}
