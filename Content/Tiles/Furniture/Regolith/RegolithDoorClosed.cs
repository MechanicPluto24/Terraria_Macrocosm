using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Regolith
{
    public class RegolithDoorClosed : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.NotReallySolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.OpenDoorID[Type] = ModContent.TileType<RegolithDoorOpen>();

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

            DustType = ModContent.DustType<RegolithDust>();
            AdjTiles = [TileID.ClosedDoor];
            AddMapEntry(new(201, 201, 204), Language.GetText("MapObject.Door"));

            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.newTile.StyleLineSkip = 3;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 2);
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

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
}