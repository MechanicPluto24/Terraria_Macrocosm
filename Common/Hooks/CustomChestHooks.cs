using Macrocosm.Common.Sets;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;

namespace Macrocosm.Common.Hooks
{
    public class CustomChestHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            IL_WorldFile.SaveChests += IL_WorldFile_SaveChests;
            On_Player.IsInInteractionRangeToMultiTileHitbox += On_Player_IsInInteractionRangeToMultiTileHitbox;
            On_ChestUI.DrawName += On_ChestUI_DrawName;
        }

        public void Unload()
        {
            IL_WorldFile.SaveChests -= IL_WorldFile_SaveChests;
            On_Player.IsInInteractionRangeToMultiTileHitbox -= On_Player_IsInInteractionRangeToMultiTileHitbox;
            On_ChestUI.DrawName -= On_ChestUI_DrawName;
        }

        // This hook is used to replace the +1 check with +0, allowing containers to save even if less than 2x2
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


        // This hook is used to allow our custom containers to get their DefaultContainerName
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
                if (string.IsNullOrEmpty(chest.name) && TileSets.CustomContainerSize[tile.TileType].X > 0 && TileSets.CustomContainerSize[tile.TileType].Y > 0)
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
            Point size = TileSets.CustomContainerSize[tile.TileType];
            if (size.X > 0 && size.Y > 0)
            {
                int tileX = (int)((player.position.X + player.width * 0.5) / 16.0);
                int tileY = (int)((player.position.Y + player.height * 0.5) / 16.0);
                Rectangle rectangle = new(chestPointX * 16, chestPointY * 16, size.X * 16, size.Y * 16);
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
