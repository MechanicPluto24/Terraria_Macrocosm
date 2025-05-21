using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class CelestialMeteorStaffProjectileStardust : ModProjectile
    {
        public override string Texture => "Macrocosm/Content/Projectiles/Environment/Meteors/StardustMeteor";

        public override void SetStaticDefaults()
        {
            Redemption.AddElementToItem(Type, Redemption.ElementID.Explosive);
            Redemption.AddElementToItem(Type, Redemption.ElementID.Arcane);
            Redemption.AddElementToItem(Type, Redemption.ElementID.Celestial);
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.scale = 0.5f;
            Projectile.width = 64;
            Projectile.height = 64;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
                ImpactEffects();
        }

        public override void AI()
        {
            float dustScaleMin = 1f;
            float dustScaleMax = 1.6f;

            if (Main.rand.NextBool(1))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    Main.rand.NextBool() ? DustID.YellowStarDust : DustID.DungeonWater,
                    0f,
                    0f,
                    Scale: Main.rand.NextFloat(dustScaleMin, dustScaleMax)
                );

                dust.noGravity = true;
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Check to see if there even IS a worm.
            if (Projectile.owner == Main.myPlayer)
            {
                Player player = Main.player[Projectile.owner];//Genuinley don't ask, for some reason Projectile.owner doesn't like to be zero >:( (In the stardust worm)
                if (player.GetModPlayer<StardustWormPlayer>().StardustWorms.Capacity == 0)
                {
                    Projectile p1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StardustWormProjectile>(), (int)(Projectile.damage / 8), 0, Main.myPlayer, 0f);
                    p1.timeLeft = 400;
                    Projectile p2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StardustWormProjectile>(), (int)(Projectile.damage / 8), 0, Main.myPlayer, 1f);
                    p2.timeLeft = 500;
                    player.GetModPlayer<StardustWormPlayer>().StardustWorms.Add(p1);
                    player.GetModPlayer<StardustWormPlayer>().StardustWorms.Add(p2);
                }
                else
                {
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StardustWormProjectile>(), (int)(Projectile.damage / 8), 0, Main.myPlayer, player.GetModPlayer<StardustWormPlayer>().StardustWorms.Capacity);
                    p.timeLeft = 500;
                    player.GetModPlayer<StardustWormPlayer>().StardustWorms.Add(p);
                }
            }
        }

        public void ImpactEffects()
        {
            int impactDustCount = Main.rand.Next(200, 300);
            for (int i = 0; i < impactDustCount; i++)
            {
                int dist = 160;
                Vector2 dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
                float distFactor = (Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist));
                Vector2 velocity = (Projectile.Center - dustPosition).SafeNormalize(default) * -14f;
                Particle.Create<DustParticle>((p =>
                {
                    p.DustType = Main.rand.NextBool() ? DustID.YellowStarDust : DustID.DungeonWater;
                    p.Position = dustPosition;
                    p.Velocity = velocity;
                    p.Scale = new Vector2(Main.rand.NextFloat(1.2f, 2f));
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                }));
            }
        }
    }
}
