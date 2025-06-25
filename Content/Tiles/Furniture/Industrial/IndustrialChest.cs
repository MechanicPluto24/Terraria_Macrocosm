using Macrocosm.Common.Drawing;
using Macrocosm.Common.Players;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Keys;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
using Macrocosm.Common.Sets;

namespace Macrocosm.Content.Tiles.Furniture.Industrial
{
    public class IndustrialChest : ModTile
    {
        private static Asset<Texture2D> glowmask;

        public enum State
        {
            /// <summary> Default style (neither lockable nor unlockable) </summary>
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
            if (Main.tile[i, j].TileType != ModContent.TileType<IndustrialChest>())
                return State.Unknown;

            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);

            if (!Enum.IsDefined(typeof(State), style))
                return State.Unknown;

            return (State)style;
        }

        public override void SetStaticDefaults()
        {

            Main.tileContainer[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.AvoidedByNPCs[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.IsAContainer[Type] = true;
            TileID.Sets.BasicChest[Type] = true;

            TileSets.CustomContainer[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles =
            [
                TileID.MagicalIceBlock,
                TileID.Boulder,
                TileID.BouncyBoulder,
                TileID.LifeCrystalBoulder,
                TileID.RollingCactus
            ];
            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;

            DustType = ModContent.DustType<IndustrialPlatingDust>();
            AddMapEntry(new Color(200, 200, 200), this.GetLocalization("MapEntryNormal"), MapChestName);
            AddMapEntry(new Color(200, 200, 200), this.GetLocalization("MapEntryUnlocked"), MapChestName);
            AddMapEntry(new Color(200, 200, 200), this.GetLocalization("MapEntryLocked"), MapChestName);            
            AdjTiles = [TileID.Containers];

            TileSets.RandomStyles[Type] = 3;

            // All styles
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.Industrial.IndustrialChest>(), 0);
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.Industrial.IndustrialChestElectronic>(), 1, 2);
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / 36);

        public override LocalizedText DefaultContainerName(int frameX, int frameY) => this.GetLocalization("MapEntry" + ((State)(frameX / 36)).ToString());

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool IsLockedChest(int i, int j) => GetState(i, j) is State.Locked;

        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
        {
            if (GetState(i, j) is not State.Locked)
                return false;

            dustType = DustType;
            return true;
        }

        public override bool LockChest(int i, int j, ref short frameXAdjustment, ref bool manual) => GetState(i, j) is State.Unlocked;

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
                    if (player.ConsumeItem(key, includeVoidBag: true))
                    {
                        if(Chest.Unlock(left, top)){
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(MessageID.LockAndUnlock, -1, -1, null, player.whoAmI, 1f, left, top);
                        }
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
                    if (GetState(left, top) is State.Locked)
                    {
                        if (player.GetModPlayer<MacrocosmPlayer>().KnowsToUseZombieFinger)
                            player.cursorItemIconID = ModContent.ItemType<ZombieFinger>();
                        else
                            CursorIcon.Current = CursorIcon.QuestionMark;
                    }
                    else
                    {
                        player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));
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
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            int height = TileObjectData.GetTileData(tile).CoordinateHeights[tile.TileFrameY / 18 % 2];

            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");

            spriteBatch.Draw(
                glowmask.Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}