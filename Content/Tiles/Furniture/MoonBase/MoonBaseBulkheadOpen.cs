using Macrocosm.Common.Hooks;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Placeable.Furniture.MoonBase;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.MoonBase
{
	public class MoonBaseBulkheadOpen : ModTile, IDoorTile
	{
		public int Height => 5;
		public int Width => 1;
        public bool IsClosed => false;
        public int StyleCount => 1;

        public override void SetStaticDefaults() {
			// Properties
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileLavaDeath[Type] = true;
			Main.tileNoSunLight[Type] = true;
			TileID.Sets.HousingWalls[Type] = true;  
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.CloseDoorID[Type] = ModContent.TileType<MoonBaseBulkheadClosed>();

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

			DustType = ModContent.DustType<MoonBasePlatingDust>();
			AdjTiles = [TileID.OpenDoor];

			RegisterItemDrop(ModContent.ItemType<MoonBaseBulkhead>(), 0);

			AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Door"));

            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.DrawYOffset = 0;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

            /*
		    TileObjectData.newTile.StyleMultiplier = 2;
		    TileObjectData.newTile.StyleWrapLimit = 2;
		 
		    for (int k = 1; k < 5; k++)
		    {
		 	   TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		 	   TileObjectData.newAlternate.Origin = new Point16(0, k);
		 	   TileObjectData.addAlternate(0);
		    }
		 
		    TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		    TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		    TileObjectData.addAlternate(1);
		    for (int l = 1; l < 5; l++)
		    {
		 	   TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		 	   TileObjectData.newAlternate.Origin = new Point16(0, l);
		 	   TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		 	   TileObjectData.addAlternate(1);
		    }
		    */

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
			player.cursorItemIconID = ModContent.ItemType<MoonBaseBulkhead>();
		}
	}
}