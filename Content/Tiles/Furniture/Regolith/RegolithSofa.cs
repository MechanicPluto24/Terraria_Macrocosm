using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Regolith;

public class RegolithSofa : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.CanBeSatOnForNPCs[Type] = true;
        TileID.Sets.CanBeSatOnForPlayers[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.Origin = new Point16(1, 1);
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
        AdjTiles = [TileID.Benches];
        DustType = ModContent.DustType<RegolithDust>();
        AddMapEntry(new(201, 201, 204), Language.GetText("ItemName.Sofa"));
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);
    }

    public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
    {
        Tile tile = Main.tile[i, j];
        info.TargetDirection = info.RestingEntity.direction;

        // Use this for frame-conditional offsets
        /*
			if ((tile.TileFrameX % 54 == 0 && info.TargetDirection == -1) || (tile.TileFrameX % 54 == 36 && info.TargetDirection == 1))
				info.VisualOffset = new Vector2(-4f, 2f);
			else if ((tile.TileFrameX % 54 == 0 && info.TargetDirection == 1) || (tile.TileFrameX % 54 == 36 && info.TargetDirection == -1))
				info.VisualOffset = new Vector2(4f, 2f);
			else
				info.VisualOffset = new Vector2(0f, 2f);
			*/

        info.VisualOffset = new Vector2(info.TargetDirection == 1 ? 0f : -2f, 2f);
        info.AnchorTilePosition = new(i, j);

        if (tile.TileFrameY / 18 % 2 == 0)
            info.AnchorTilePosition.Y += 1;
    }

    public override bool RightClick(int i, int j)
    {
        Player player = Main.LocalPlayer;

        if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
        {
            player.GamepadEnableGrappleCooldown();
            player.sitting.SitDown(player, i, j);
        }

        return true;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            return;

        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));
    }
}
