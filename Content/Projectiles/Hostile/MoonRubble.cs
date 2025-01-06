using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class MoonRubble : ModProjectile
    {


        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;

            Projectile.tileCollide = false;

            Projectile.timeLeft = 300;
            Projectile.hostile = true;
            Projectile.netImportant = true;
            Projectile.scale = Main.rand.NextFloat(0.5f, 1.5f);
        }


        //Literally just regolith debris lol.
        //TODO Sprite is made by me and is therfore bad and needs to be resprited.
        public bool ScheduleAmbientTileSpawnEffect
        {
            get => Projectile.ai[0] != 0f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        public override void AI()
        {
            float gravity = 0.8f * (0.5f + 0.5f * MacrocosmSubworld.GetGravityMultiplier()); ;
            Projectile.velocity.Y += gravity;
            Projectile.rotation += Projectile.velocity.X * 0.05f;

            // If colliding, stop and roll on the ground
            Projectile.velocity = Collision.TileCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            // Roll up slopes
            Vector4 slopeCollision = Collision.SlopeCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, gravity, fall: true);
            Projectile.position = slopeCollision.XY();
            Projectile.velocity = slopeCollision.ZW();

            if (Projectile.velocity.Y == 0f)
                Projectile.hostile = false;
            else
                Projectile.hostile = true;


            // Decelerate while on the ground
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
            Projectile.Opacity = (float)Projectile.timeLeft / 300;

            // Dust and sound effects for all clients
            if (ScheduleAmbientTileSpawnEffect)
            {
                ScheduleAmbientTileSpawnEffect = false;

                SoundEngine.PlaySound(SoundID.Dig with { Volume = 0.2f }, Projectile.Center);

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

        public override Color? GetAlpha(Color lightColor)
        {
            return base.GetAlpha(lightColor);
        }


    }
}
