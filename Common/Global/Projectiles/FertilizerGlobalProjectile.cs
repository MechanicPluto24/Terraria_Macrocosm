using Macrocosm.Common.Sets;
using System.Numerics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Projectiles;

public class FertilizerGlobalProjectile : GlobalProjectile
{
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.Fertilizer;

    // While CustomTree supports AttemptToGrowTreeFromSapling, Fertilizer AI only calls it for vanilla tiles (including ModTree), but not for modded tiles :(
    // Here it runs the logic again for our saplings 
    public override void PostAI(Projectile projectile)
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            int hitMinX = (int)(projectile.position.X / 16f) - 1;
            int hitMaxX = (int)((projectile.position.X + projectile.width) / 16f) + 2;
            int hitMinY = (int)(projectile.position.Y / 16f) - 1;
            int hitMaxY = (int)((projectile.position.Y + projectile.height) / 16f) + 2;

            for (int i = hitMinX; i < hitMaxX; i++)
            {
                for (int j = hitMinY; j < hitMaxY; j++)
                {
                    Vector2 position = new Vector2(i, j) * 16f;
                    if (!(projectile.position.X + projectile.width > position.X) || !(projectile.position.X < position.X + 16f) || !(projectile.position.Y + (float)projectile.height > position.Y) || !(projectile.position.Y < position.Y + 16f) || !Main.tile[i, j].HasTile)
                        continue;

                    Tile tile = Main.tile[i, j];
                    if (TileSets.SaplingTreeGrowthType[tile.TileType] > 0)
                    {
                        if (Main.remixWorld && j >= (int)Main.worldSurface - 1 && j < Main.maxTilesY - 20)
                            WorldGen.AttemptToGrowTreeFromSapling(i, j, underground: false);

                        WorldGen.AttemptToGrowTreeFromSapling(i, j, j > (int)Main.worldSurface - 1);

                        // TODO (1.4.5): Add super fertilizer support
                    }
                }
            }
        }
    }
}
