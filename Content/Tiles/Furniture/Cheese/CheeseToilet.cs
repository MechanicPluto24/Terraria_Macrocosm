using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Cheese;

public class CheeseToilet : ModTile
{

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.CanBeSatOnForNPCs[Type] = true;
        TileID.Sets.CanBeSatOnForPlayers[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

        DustType = ModContent.DustType<CheeseDust>();
        AdjTiles = [TileID.Toilets]; // Consider adding TileID.Chairs to AdjTiles to mirror "(regular) Toilet" and "Golden Toilet" behavior for crafting stations

        AddMapEntry(new Color(220, 216, 121), Language.GetText("MapObject.Toilet"));

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.CoordinateHeights = [16, 16];
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.StyleWrapLimit = 2;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.addAlternate(1);
        TileObjectData.addTile(Type);
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);
    }

    public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
    {
        Tile tile = Framing.GetTileSafely(i, j);

        info.VisualOffset.X -= (tile.TileFrameX % (18 * 2) == 0) ? 6f : -10f;
        info.TargetDirection = -1;

        if (tile.TileFrameX >= 18 * 2)
        {
            info.VisualOffset.X -= (tile.TileFrameX % (18 * 2) == 0) ? -16f : 16f;
            info.TargetDirection = 1;
        }

        info.AnchorTilePosition = new(i, j);

        if (tile.TileFrameY == 0)
            info.AnchorTilePosition.Y += 1;

        // Finally, since this is a toilet, it should generate Poo while any tier of Well Fed is active
        info.ExtraInfo.IsAToilet = true;

        // Yes we are keeping this -- Feldy
        // Here we add a custom fun effect to this tile that vanilla toilets do not have. This shows how you can type cast the restingEntity to Player and use visualOffset as well.
        if (info.RestingEntity is Player player && player.HasBuff(BuffID.Stinky))
        {
            info.VisualOffset = Main.rand.NextVector2Circular(2, 2);
        }
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
        {
            return;
        }

        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));
        player.cursorItemIconReversed = Main.tile[i, j].TileFrameX < 18 * 2;
    }

    public override void HitWire(int i, int j)
    {
        Tile tile = Main.tile[i, j];

        int spawnX = i;
        int spawnY = j - (tile.TileFrameY % (18 * 2)) / 18;

        Wiring.SkipWire(spawnX, spawnY);
        Wiring.SkipWire(spawnX, spawnY + 1);
        Wiring.SkipWire(spawnX + 1, spawnY);
        Wiring.SkipWire(spawnX + 1, spawnY + 1);

        if (Wiring.CheckMech(spawnX, spawnY, 60))
        {
            Projectile.NewProjectile(Wiring.GetProjectileSource(spawnX, spawnY), spawnX * 16 + 8, spawnY * 16 + 12, 0f, 0f, ProjectileID.ToiletEffect, 0, 0f, Main.myPlayer);
        }
    }
}
