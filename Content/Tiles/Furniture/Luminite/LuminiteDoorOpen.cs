using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Luminite;

public class LuminiteDoorOpen : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileLavaDeath[Type] = true;
        Main.tileNoSunLight[Type] = true;
        TileID.Sets.HousingWalls[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.CloseDoorID[Type] = ModContent.TileType<LuminiteDoorClosed>();

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

        DustType = DustID.LunarOre;
        AdjTiles = [TileID.OpenDoor];

        foreach (LuminiteStyle style in Enum.GetValues(typeof(LuminiteStyle)))
            AddMapEntry(Utility.GetTileColorFromLuminiteStyle(style), Language.GetText("MapObject.Door"));

        TileObjectData.newTile.Width = 2;
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.Origin = new Point16(0, 0);
        TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
        TileObjectData.newTile.UsesCustomCanPlace = true;
        TileObjectData.newTile.LavaDeath = true;
        TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.StyleWrapLimit = 2;
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Origin = new Point16(0, 1);
        TileObjectData.addAlternate(0);
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Origin = new Point16(0, 2);
        TileObjectData.addAlternate(0);
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Origin = new Point16(1, 0);
        TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
        TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.addAlternate(1);
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Origin = new Point16(1, 1);
        TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
        TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.addAlternate(1);
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Origin = new Point16(1, 2);
        TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
        TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.addAlternate(1);
        TileObjectData.addTile(Type);

        // Tiles usually drop their corresponding item automatically, but RegisterItemDrop is needed here.
        RegisterItemDrop(ModContent.ItemType<Items.Furniture.Luminite.LuminiteDoor>(), (int)LuminiteStyle.Luminite);
        RegisterItemDrop(ModContent.ItemType<Items.Furniture.Heavenforge.HeavenforgeDoor>(), (int)LuminiteStyle.Heavenforge);
        RegisterItemDrop(ModContent.ItemType<Items.Furniture.LunarRust.LunarRustDoor>(), (int)LuminiteStyle.LunarRust);
        RegisterItemDrop(ModContent.ItemType<Items.Furniture.Astra.AstraDoor>(), (int)LuminiteStyle.Astra);
        RegisterItemDrop(ModContent.ItemType<Items.Furniture.DarkCelestial.DarkCelestialDoor>(), (int)LuminiteStyle.DarkCelestial);
        RegisterItemDrop(ModContent.ItemType<Items.Furniture.Mercury.MercuryDoor>(), (int)LuminiteStyle.Mercury);
        RegisterItemDrop(ModContent.ItemType<Items.Furniture.StarRoyale.StarRoyaleDoor>(), (int)LuminiteStyle.StarRoyale);
        RegisterItemDrop(ModContent.ItemType<Items.Furniture.Cryocore.CryocoreDoor>(), (int)LuminiteStyle.Cryocore);
        RegisterItemDrop(ModContent.ItemType<Items.Furniture.CosmicEmber.CosmicEmberDoor>(), (int)LuminiteStyle.CosmicEmber);
    }

    public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameY / (18 * 3));

    public override bool CreateDust(int i, int j, ref int type)
    {
        type = Utility.GetDustTypeFromLuminiteStyle((LuminiteStyle)(Main.tile[i, j].TileFrameY / (18 * 3)));
        return true;
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