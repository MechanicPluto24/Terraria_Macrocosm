using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Tiles.Ambient;
using Macrocosm.Content.Tiles.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Debris
{
    public class RegolithDebris : ModProjectile
    {
        private const int TimeToLive = 60;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;

            Projectile.timeLeft = TimeToLive;

            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Pick a random frame 
            Projectile.frame = Main.rand.Next(6);
        }

        public bool ScheduleAmbientTileSpawnEffect
        {
            get => Projectile.ai[0] != 0f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        public override void AI()
        {
			Projectile.velocity.Y += 0.15f * MacrocosmSubworld.CurrentGravityMultiplier;
			Projectile.rotation += Projectile.velocity.X * 0.05f;

            // If colliding, stop and roll on the ground
			Projectile.velocity = Collision.TileCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            if (Projectile.velocity.Y == 0f)
            {
                Projectile.velocity.X *= 0.97f;

                if (Projectile.velocity.X > -0.01 && Projectile.velocity.X < 0.01)
                    Projectile.velocity.X = 0f;
            }

            // Keep time left as it is until the debris stops
			if (Projectile.velocity != Vector2.Zero)
                 Projectile.timeLeft++;

            // Fade out as timeLeft decreases
            Projectile.Opacity = (float)Projectile.timeLeft / TimeToLive;
 
            // Once stopped...
			if (Projectile.velocity == Vector2.Zero && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int tileX = (int)(Projectile.Center.X / 16);
                int tileY = (int)(Projectile.Center.Y / 16);
                Tile below = Main.tile[tileX, tileY + 1];

                //...randomly, on the moment the debris stopped, check if tile is regolith
                if (Main.rand.NextBool(4) && Projectile.timeLeft == TimeToLive && below.TileType == ModContent.TileType<Regolith>())
                {
                    int style = Main.rand.Next(9);
                    bool placeTile = false;

                    // Attempt to place a 1x1 regolith pile
                    if (TileObject.CanPlace(tileX, tileY, ModContent.TileType<RegolithRockSmall>(), style, Projectile.direction, out TileObject objectData))
                        placeTile = TileObject.Place(objectData);

                    if (placeTile)
                    {
                        // Send tile placement to clients if on MP
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendObjectPlacement(-1, tileX, tileY, objectData.type, objectData.style, objectData.alternate, objectData.random, Projectile.direction);
                            Projectile.netUpdate = true;
                        }

                        // Schedule effects for all clients
                        ScheduleAmbientTileSpawnEffect = true;
                    }
                }
            }

            // Dust and sound effects 
            if (ScheduleAmbientTileSpawnEffect)
            {
                ScheduleAmbientTileSpawnEffect = false;

                SoundEngine.PlaySound(SoundID.Dig with { Volume = 0.2f }, Projectile.Center) ;

                for (int i = 0; i < Main.rand.Next(5, 10); i++)
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        ModContent.DustType<RegolithDust>(),
                        new Vector2(Main.rand.NextFloat(-1.2f, 1.2f), Main.rand.NextFloat(0f, -1.8f)),
                        Scale: Main.rand.NextFloat(0.2f, 1.1f)
                    );
                
                    dust.noGravity = false;
                }
            }
        }
    }
}
