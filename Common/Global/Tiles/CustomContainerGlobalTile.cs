using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Macrocosm.Common.Global.Tiles
{
    public class CustomContainerGlobalTile : GlobalTile
    {
        public override void Load()
        {
            IL_WorldFile.SaveChests += IL_WorldFile_SaveChests;
            On_Player.IsInInteractionRangeToMultiTileHitbox += On_Player_IsInInteractionRangeToMultiTileHitbox;
            On_ChestUI.DrawName += On_ChestUI_DrawName;
        }

        public override void Unload()
        {
            IL_WorldFile.SaveChests -= IL_WorldFile_SaveChests;
            On_Player.IsInInteractionRangeToMultiTileHitbox -= On_Player_IsInInteractionRangeToMultiTileHitbox;
            On_ChestUI.DrawName -= On_ChestUI_DrawName;
        }

        // This hook is used to prevent the player from destroying custom containers that are not empty, and tiles below them
        private bool CanTileBeAltered(int i, int j, int type)
        {
            Tile tileAbove = default;
            if (j >= 1)
                tileAbove = Main.tile[i, j - 1];

            if (TileSets.CustomContainer[tileAbove.TileType])
            {
                if (tileAbove.HasTile && type != tileAbove.TileType)
                    return false;

                Point16 topLeft = TileObjectData.TopLeft(i, j);
                if (!Chest.CanDestroyChest(topLeft.X, topLeft.Y))
                    return false;
            }

            return true;
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!CanTileBeAltered(i, j, type))
                fail = true;
        }

        public override void NumDust(int i, int j, int type, bool fail, ref int num)
        {
            if (!CanTileBeAltered(i, j, type))
                num = 0;
        }

        public override bool KillSound(int i, int j, int type, bool fail)
        {
            if (!CanTileBeAltered(i, j, type))
                return false;

            return true;
        }

        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            if (!CanTileBeAltered(i, j, type))
                resetFrame = false;

            return true;
        }

        // This hook is used to modify the SaveChest logic to allow for custom containers to be less than 2x2
        private void IL_WorldFile_SaveChests(ILContext il)
        {
            var c = new ILCursor(il);

            // Replace "chest.x + 1" with "chest.x + 0"
            if (c.TryGotoNext(MoveType.Before,
                i => i.MatchLdfld("Terraria.Chest", "x"),  // Load chest.x
                i => i.MatchLdcI4(1),                      // Load constant 1
                i => i.MatchAdd()                          // Add operation
            ))
            {
                c.Index++; // Move  
                c.Remove(); // Remove ldc.i4.1
                c.Emit(OpCodes.Ldc_I4_0); // Emit ldc.i4.0
            }

            // Replace "chest.y + 1" with "chest.y + 0"
            if (c.TryGotoNext(MoveType.Before,
                i => i.MatchLdfld("Terraria.Chest", "y"),  // Load chest.y
                i => i.MatchLdcI4(1),                      // Load constant 1
                i => i.MatchAdd()                          // Add operation
            ))
            {
                c.Index++; // Move 
                c.Remove(); // Remove ldc.i4.1
                c.Emit(OpCodes.Ldc_I4_0); // Emit ldc.i4.0
            }
        }


        // This hook is used to allow custom containers to get their DefaultContainerName
        private void On_ChestUI_DrawName(On_ChestUI.orig_DrawName orig, Microsoft.Xna.Framework.Graphics.SpriteBatch spritebatch)
        {
            Player player = Main.LocalPlayer;
            if (!Main.editChest && player.chest > -1)
            {
                if (Main.chest[player.chest] == null)
                    Main.chest[player.chest] = new Chest();
                Chest chest = Main.chest[player.chest];
                Tile tile = Main.tile[player.chestX, player.chestY];

                // Only call if this is a custom container and the chest is unnamed
                if (string.IsNullOrEmpty(chest.name) && TileSets.CustomContainer[tile.TileType])
                {
                    string text = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);
                    Color color = Color.White * (1f - (255f - Main.mouseTextColor) / 255f * 0.5f);
                    color.A = byte.MaxValue;
                    Terraria.Utils.WordwrapString(text, FontAssets.MouseText.Value, 200, 1, out var lineAmount);
                    lineAmount++;
                    for (int i = 0; i < lineAmount; i++)
                    {
                        ChatManager.DrawColorCodedStringWithShadow(spritebatch, FontAssets.MouseText.Value, text, new Vector2(504f, Main.instance.invBottom + i * 26), color, 0f, Vector2.Zero, Vector2.One, -1f, 1.5f);
                    }

                    return; // Skip original
                }
            }

            orig(spritebatch);
        }

        // This hook is used to allow the player to interact nicely with custom multi-tile containers
        private bool On_Player_IsInInteractionRangeToMultiTileHitbox(On_Player.orig_IsInInteractionRangeToMultiTileHitbox orig, Player player, int chestPointX, int chestPointY)
        {
            Tile tile = Main.tile[chestPointX, chestPointY];
            if (TileSets.CustomContainer[tile.TileType])
            {
                var data = TileObjectData.GetTileData(tile);
                int width = data != null ? data.Width : 1;
                int height = data != null ? data.Height : 1;
                int tileX = (int)((player.position.X + player.width * 0.5) / 16.0);
                int tileY = (int)((player.position.Y + player.height * 0.5) / 16.0);
                Rectangle rectangle = new(chestPointX * 16, chestPointY * 16, width * 16, height * 16);
                rectangle.Inflate(-1, -1);
                Point point = rectangle.ClosestPointInRect(player.Center).ToTileCoordinates();
                chestPointX = point.X;
                chestPointY = point.Y;
                return tileX >= chestPointX - Player.tileRangeX && tileX <= chestPointX + Player.tileRangeX + 1 && tileY >= chestPointY - Player.tileRangeY && tileY <= chestPointY + Player.tileRangeY + 1;
            }

            return orig(player, chestPointX, chestPointY);
        }
    }
}
