using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Furniture.Industrial;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Industrial;

public class IndustrialDoorOpen : ModTile, IDoorTile
{
    public int Height => 3;
    public int Width => 1;
    public bool IsClosed => false;
    public int StyleCount => 1;

    public override void SetStaticDefaults()
    {
        // Properties
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileLavaDeath[Type] = true;
        Main.tileNoSunLight[Type] = true;
        TileID.Sets.HousingWalls[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.CloseDoorID[Type] = ModContent.TileType<IndustrialDoorClosed>();

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

        DustType = ModContent.DustType<IndustrialPlatingDust>();
        AdjTiles = [TileID.OpenDoor];

        RegisterItemDrop(ModContent.ItemType<IndustrialDoor>(), 0);

        AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Door"));

        TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.OpenDoor, 0));
        TileObjectData.newTile.Width = Width;
        TileObjectData.newTile.Height = Height;
        TileObjectData.addTile(Type);
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return true;
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = 1;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));
    }
}