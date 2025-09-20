using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.TileFrame;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Furniture.Industrial;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Industrial;

public class IndustrialBulkheadOpen : ModTile, IDoorTile
{
    public int Height => 5;
    public int Width => 1;
    public bool IsClosed => false;
    public int StyleCount => 1;
    public AnimationData? AnimationData => new(3, 5, [3, 2, 1, 0], forcedUpdate: true);

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileLavaDeath[Type] = true;
        Main.tileNoSunLight[Type] = true;
        TileID.Sets.HousingWalls[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.CloseDoorID[Type] = ModContent.TileType<IndustrialBulkheadClosed>();

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

        DustType = ModContent.DustType<IndustrialPlatingDust>();
        AdjTiles = [TileID.OpenDoor];

        RegisterItemDrop(ModContent.ItemType<IndustrialBulkhead>(), 0);

        AddMapEntry(new Color(200, 200, 200), CreateMapEntryName());

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

        TileObjectData.addTile(Type);
    }

    public Rectangle ModifyAutoDoorPlayerCollisionRectangle(Point tileCoords, Rectangle original)
    {
        Rectangle result = original;
        result.Y -= 16;
        result.Height = Height * 5;
        return result;
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
    {
        if (TileAnimation.GetTemporaryFrame(TileObjectData.TopLeft(i, j), out int frame))
            frameYOffset = (short)(18 * Height * frame);
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