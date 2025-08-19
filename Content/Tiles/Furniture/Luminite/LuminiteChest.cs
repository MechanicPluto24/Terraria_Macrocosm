using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Systems.Flags;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Keys;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Luminite;

public class LuminiteChest : ModTile
{
    public override void SetStaticDefaults()
    {
        // Properties
        Main.tileSpelunker[Type] = true;
        Main.tileContainer[Type] = true;
        Main.tileShine2[Type] = true;
        Main.tileShine[Type] = 1200;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileOreFinderPriority[Type] = 500;

        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.BasicChest[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.AvoidedByNPCs[Type] = true;
        TileID.Sets.InteractibleByNPCs[Type] = true;
        TileID.Sets.IsAContainer[Type] = true;
        TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
        TileID.Sets.GeneralPlacementTiles[Type] = false;

        DustType = DustID.LunarOre;
        AdjTiles = [TileID.Containers];

        IList styles = Enum.GetValues(typeof(LuminiteStyle));
        for (int i = 0; i < styles.Count; i++)
        {
            LuminiteStyle style = (LuminiteStyle)styles[i];
            AddMapEntry(Utility.GetTileColorFromLuminiteStyle(style), this.GetLocalization($"MapEntry{i * 2}"), MapChestName);
            AddMapEntry(Utility.GetTileColorFromLuminiteStyle(style), this.GetLocalization($"MapEntry{(i * 2) + 1}"), MapChestName);
        }

        RegisterItemDrop(ModContent.ItemType<Items.Furniture.Luminite.LuminiteChest>(), 0, 1);

        // Sometimes mods remove content, such as tile styles, or tiles accidentally get corrupted. We can, if desired, register a fallback item for any tile style that doesn't have an automatically determined item drop. This is done by omitting the tileStyles parameter.
        RegisterItemDrop(ItemID.Chest);

        // Placement
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
        TileObjectData.newTile.AnchorInvalidTiles = [
            TileID.MagicalIceBlock,
            TileID.Boulder,
            TileID.BouncyBoulder,
            TileID.LifeCrystalBoulder,
            TileID.RollingCactus
        ];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
        TileObjectData.addTile(Type);
    }

    public override bool CreateDust(int i, int j, ref int type)
    {
        type = Utility.GetDustTypeFromLuminiteStyle((LuminiteStyle)(Main.tile[i, j].TileFrameX / (18 * 2 * 2)));
        return true;
    }

    public override ushort GetMapOption(int i, int j) => (ushort)((Main.tile[i, j].TileFrameX / (18 * 2)));

    public override LocalizedText DefaultContainerName(int frameX, int frameY) => this.GetLocalization("MapEntry" + (frameX / (18 * 2)));

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

    public override bool IsLockedChest(int i, int j) => (Main.tile[i, j].TileFrameX / 36) % 2 == 1;

    public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
    {
        LuminiteStyle style = (LuminiteStyle)(Main.tile[i, j].TileFrameX / (18 * 2 * 2));
        dustType = Utility.GetDustTypeFromLuminiteStyle(style);

        if (style == LuminiteStyle.Luminite)
            return true;

        switch (style)
        {
            case LuminiteStyle.Heavenforge:
                WorldData.HeavenforgeShrineUnlocked = true;
                OnFirstUnlock(WorldData.HeavenforgeShrineUnlocked, i, j, style);
                break;
            case LuminiteStyle.LunarRust:
                WorldData.LunarRustShrineUnlocked = true;
                OnFirstUnlock(WorldData.LunarRustShrineUnlocked, i, j, style);
                break;
            case LuminiteStyle.Astra:
                WorldData.AstraShrineUnlocked = true;
                OnFirstUnlock(WorldData.AstraShrineUnlocked, i, j, style);
                break;
            case LuminiteStyle.DarkCelestial:
                WorldData.DarkCelestialShrineUnlocked = true;
                OnFirstUnlock(WorldData.DarkCelestialShrineUnlocked, i, j, style);
                break;
            case LuminiteStyle.Mercury:
                WorldData.MercuryShrineUnlocked = true;
                OnFirstUnlock(WorldData.MercuryShrineUnlocked, i, j, style);
                break;
            case LuminiteStyle.StarRoyale:
                WorldData.StarRoyaleShrineUnlocked = true;
                OnFirstUnlock(WorldData.StarRoyaleShrineUnlocked, i, j, style);
                break;
            case LuminiteStyle.Cryocore:
                WorldData.CryocoreShrineUnlocked = true;
                OnFirstUnlock(WorldData.CryocoreShrineUnlocked, i, j, style);
                break;
            case LuminiteStyle.CosmicEmber:
                WorldData.CosmicEmberShrineUnlocked = true;
                OnFirstUnlock(WorldData.CosmicEmberShrineUnlocked, i, j, style);
                break;
            default:
                break;
        }

        return true;
    }

    private void OnFirstUnlock(bool flag, int i, int j, LuminiteStyle style)
    {
        if (flag)
            return;

        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;

        for (float f = 0f; f < 10f; f += 1f)
        {
            int time = Main.rand.Next(20, 40);
            Particle.Create<PrettySparkle>((p) =>
            {
                p.Position = new Vector2(i, j) * 16f + new Vector2(16f) + Main.rand.NextVector2Circular(16f, 16f);
                p.Color = Utility.GetLightColorFromLuminiteStyle(style);
                p.Scale = new Vector2(1f + Main.rand.NextFloat() * 2f, 0.7f + Main.rand.NextFloat() * 0.7f);
                p.Velocity = new Vector2(0f, -2f) * Main.rand.NextFloat();
                p.FadeInNormalizedTime = 5E-06f;
                p.FadeOutNormalizedTime = 0.95f;
                p.TimeToLive = time;
                p.FadeOutEnd = time;
                p.FadeInEnd = time / 2;
                p.FadeOutStart = time / 2;
                p.AdditiveAmount = 0.35f;
                p.DrawHorizontalAxis = true;
                p.DrawVerticalAxis = true;
            }, shouldSync: true);
        }

        NetMessage.SendData(MessageID.WorldData);
    }

    public override bool LockChest(int i, int j, ref short frameXAdjustment, ref bool manual)
    {
        int style = Main.tile[i, j].TileFrameX / 36;
        return style % 2 == 0;
    }

    public static string MapChestName(string name, int i, int j)
    {
        int left = i;
        int top = j;
        Tile tile = Main.tile[i, j];

        if (tile.TileFrameX % 36 != 0)
            left--;

        if (tile.TileFrameY != 0)
            top--;

        int chest = Chest.FindChest(left, top);
        if (chest < 0)
            return Language.GetTextValue("LegacyChestType.0");

        if (Main.chest[chest].name == "")
            return name;

        return name + ": " + Main.chest[chest].name;
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = 1;
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        Chest.DestroyChest(i, j);
    }

    // Disabled for now, since it's buggy
    public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
    {
        return false;
    }

    public override bool RightClick(int i, int j)
    {
        Player player = Main.LocalPlayer;
        Tile tile = Main.tile[i, j];
        Main.mouseRightRelease = false;
        int left = i;
        int top = j;

        if (tile.TileFrameX % 36 != 0)
            left--;

        if (tile.TileFrameY != 0)
            top--;

        player.CloseSign();
        player.SetTalkNPC(-1);
        Main.npcChatCornerItem = 0;
        Main.npcChatText = "";
        if (Main.editChest)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            Main.editChest = false;
            Main.npcChatText = string.Empty;
        }

        if (player.editedChestName)
        {
            NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
            player.editedChestName = false;
        }

        bool isLocked = Chest.IsLocked(left, top);
        if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked)
        {
            if (left == player.chestX && top == player.chestY && player.chest != -1)
            {
                player.chest = -1;
                Recipe.FindRecipes();
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
            else
            {
                NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top);
                Main.stackSplit = 600;
            }
        }
        else
        {
            if (isLocked)
            {
                // Make sure to change the code in UnlockChest if you don't want the chest to only unlock at night.
                int key = ModContent.ItemType<XaocKey>();
                if (player.HasItemInInventoryOrOpenVoidBag(key) && Chest.Unlock(left, top))
                {
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.LockAndUnlock, -1, -1, null, player.whoAmI, 1f, left, top);
                }
            }
            else
            {
                int chest = Chest.FindChest(left, top);
                if (chest != -1)
                {
                    Main.stackSplit = 600;
                    if (chest == player.chest)
                    {
                        player.chest = -1;
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    }
                    else
                    {
                        SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                        player.OpenChest(left, top, chest);
                    }

                    Recipe.FindRecipes();
                }
            }
        }

        return true;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        Tile tile = Main.tile[i, j];
        int left = i;
        int top = j;
        if (tile.TileFrameX % 36 != 0)
        {
            left--;
        }

        if (tile.TileFrameY != 0)
        {
            top--;
        }

        int chest = Chest.FindChest(left, top);
        player.cursorItemIconID = -1;
        if (chest < 0)
        {
            player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
        }
        else
        {
            string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY); // This gets the ContainerName text for the currently selected language
            player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
            if (player.cursorItemIconText == defaultName)
            {
                player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));

                if (Main.tile[left, top].TileFrameX / 36 % 2 == 1)
                    player.cursorItemIconID = ModContent.ItemType<XaocKey>();

                player.cursorItemIconText = "";
            }
        }

        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
    }

    public override void MouseOverFar(int i, int j)
    {
        MouseOver(i, j);
        Player player = Main.LocalPlayer;
        if (player.cursorItemIconText == "")
        {
            player.cursorItemIconEnabled = false;
            player.cursorItemIconID = 0;
        }
    }
}
