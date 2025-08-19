using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Pickups;

public class Medkit : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = false;
        Main.tileSolidTop[Type] = false;
        Main.tileTable[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;

        DustType = -1;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
        TileObjectData.newTile.CoordinateHeights = [18];
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidWithTop, 2, 0);
        TileObjectData.addTile(Type);

        TileID.Sets.DisableSmartCursor[Type] = true;
        AddMapEntry(new Color(202, 0, 0), CreateMapEntryName());

        FlexibleTileWand.RubblePlacementMedium.AddVariations(ModContent.ItemType<Items.Consumables.Potions.Medkit>(), Type, 0);
        RegisterItemDrop(ModContent.ItemType<Items.Consumables.Potions.Medkit>(), 0);
    }
}