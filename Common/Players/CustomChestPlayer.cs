using Macrocosm.Common.Sets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players
{
    public class CustomChestPlayer : ModPlayer
    {
        public override void Load()
        {
            On_Player.IsInInteractionRangeToMultiTileHitbox += On_Player_IsInInteractionRangeToMultiTileHitbox;
        }

        public override void Unload()
        {
            On_Player.IsInInteractionRangeToMultiTileHitbox -= On_Player_IsInInteractionRangeToMultiTileHitbox;
        }

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
