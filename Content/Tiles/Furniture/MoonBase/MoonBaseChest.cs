using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.CursorIcons;
using Macrocosm.Content.Items.Tools;
using Macrocosm.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.MoonBase
{
    public class MoonBaseChest : ModTile
    {
        public enum State
        {
            /// <summary> Default style (not lockable or unlockable) </summary>
            Normal,
            /// <summary> Unlocked chest, lockable </summary>
            Unlocked,
            /// <summary> Locked chest, unlockable </summary>
            Locked,
            /// <summary> Unknown, when evaluating any other tile or other unexpected things happen </summary>
            Unknown = -1
        }

        /// <summary> Returns the chest state based on its tile style </summary>
        public static State GetState(int i, int j)
        {
            if (Main.tile[i, j].TileType != ModContent.TileType<MoonBaseChest>())
                return State.Unknown;

            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);

            if (!Enum.IsDefined(typeof(State), style))
                return State.Unknown;

            return (State)style;
        }

        public override void SetStaticDefaults()
        {
            Main.tileSpelunker[Type] = true;
            Main.tileContainer[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1000;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.BasicChest[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.AvoidedByNPCs[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.IsAContainer[Type] = true;
            TileID.Sets.FriendlyFairyCanLureTo[Type] = true;

            Main.tileOreFinderPriority[Type] = 1050;

            DustType = ModContent.DustType<MoonBasePlatingDust>();
            AdjTiles = new int[] { TileID.Containers };

            AddMapEntry(new Color(200, 200, 200), this.GetLocalization("MapEntryNormal"), MapChestName);
            AddMapEntry(new Color(200, 200, 200), this.GetLocalization("MapEntryUnlocked"), MapChestName);
            AddMapEntry(new Color(200, 200, 200), this.GetLocalization("MapEntryLocked"), MapChestName);

            // Style 1 is the chest when locked. We want that tile style to drop the chest item as well. Use the Chest Lock item to lock this chest.
            // No item places chest in the locked style, so the automatically determined item drop is unknown, this is why RegisterItemDrop is necessary in this situation. 
            RegisterItemDrop(ModContent.ItemType<Items.Placeable.Furniture.MoonBase.MoonBaseChest>(), 1);

            // Sometimes mods remove content, such as tile styles, or tiles accidentally get corrupted.
            // We can, if desired, register a fallback item for any tile style that doesn't have an automatically determined item drop.
            // This is done by omitting the tileStyles parameter.
            RegisterItemDrop(ItemID.Chest);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);

            TileObjectData.newTile.AnchorInvalidTiles = new int[]
            {
                TileID.MagicalIceBlock,
                TileID.Boulder,
                TileID.BouncyBoulder,
                TileID.LifeCrystalBoulder,
                TileID.RollingCactus
            };

            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
        }

        public override ushort GetMapOption(int i, int j)
        {
            return (ushort)(Main.tile[i, j].TileFrameX / 36);
        }

        public override LocalizedText DefaultContainerName(int frameX, int frameY)
        {
            return this.GetLocalization("MapEntry" + ((State)(frameX / 36)).ToString());
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public override bool IsLockedChest(int i, int j)
        {
            return GetState(i, j) is State.Locked;
        }

        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
        {
            if (GetState(i, j) is not State.Locked)
                return false;

            dustType = DustType;
            return true;
        }

        public override bool LockChest(int i, int j, ref short frameXAdjustment, ref bool manual)
        {
            return GetState(i, j) is State.Unlocked;
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

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Chest.DestroyChest(i, j);
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

            bool isLocked = GetState(left, top) is State.Locked;
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
                    int key = ModContent.ItemType<ZombieFinger>();
                    if (Chest.Unlock(left, top) && player.ConsumeItem(key, includeVoidBag: true))
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
                left--;

            if (tile.TileFrameY != 0)
                top--;

            int chest = Chest.FindChest(left, top);
            player.cursorItemIconID = -1;
            if (chest < 0)
            {
                player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
            }
            else
            {
                string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);
                player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;

                if (player.cursorItemIconText == defaultName)
                {
                    player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Furniture.MoonBase.MoonBaseChest>();

                    if (GetState(left, top) is State.Locked)
                    {
                        if (player.GetModPlayer<MacrocosmPlayer>().KnowsToUseZombieFinger)
                            player.cursorItemIconID = ModContent.ItemType<ZombieFinger>();
                        else
                            player.cursorItemIconID = CursorIcon.GetType<QuestionMark>();
                    }

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

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (GetState(i, j) is not State.Unlocked or State.Locked)
                return;

            Tile tile = Main.tile[i, j];
            Texture2D glow = ModContent.Request<Texture2D>("Macrocosm/Content/Tiles/Furniture/MoonBase/MoonBaseChest_Glow").Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            int height = TileObjectData.GetTileData(tile).CoordinateHeights[tile.TileFrameY / 18 % 2];

            spriteBatch.Draw(
                glow,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}