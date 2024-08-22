using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    // I GIVE UP WITH THIS I'M JUST MAKING TWO DIFFERENT SUBCLASSES AT THIS POINT.
    public class StarDestroyerStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public Color colour = new Color(0, 0, 0);
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            ProjectileID.Sets.TrailCacheLength[Type] = 2;
        }
    }

    public class StarDestroyerStarBlue : StarDestroyerStar
    {
        public NPC FindClosestNPC(float maxDetectDistance)
        {//example mod
            NPC closestNPC = null;


            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;


            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];

                if (target.CanBeChasedBy())
                {

                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
        int Timer = 0;
        public override string Texture => "Macrocosm/Content/Projectiles/Friendly/Ranged/StarDestroyerStar";
        public override void AI()
        {
            Projectile.rotation += 0.5f;
            colour = new Color(100, 100, 255);
            Projectile.SetTrail<StarTrail>();
            Projectile.rotation = Projectile.velocity.ToRotation();
            NPC closestNPC = FindClosestNPC(9000f);
            if (closestNPC == null)
            {

                return;
            }
            else
            {
                Vector2 vel = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Projectile.velocity = Projectile.velocity + (vel * 0.9f);
                Projectile.velocity = (Projectile.velocity).SafeNormalize(Vector2.UnitX);
                Projectile.velocity *= 30f;
            }
            Timer++;
            if (Timer % 6 == 0)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, -Projectile.velocity.RotatedByRandom(Math.PI * 2) * 0.3f, 16);//16 and 17 are the star gores.
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, -Projectile.velocity.RotatedByRandom(Math.PI * 2) * 0.3f, 17);
            }
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = Projectile.Size / 2f;
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            Projectile.GetTrail().Draw(Projectile.Size / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (colour) * 0.4f, Projectile.rotation, origin, Projectile.scale * 1.8f, SpriteEffects.None, 0);
            return false;
        }
        public override void OnKill(int timeLeft)
        {

            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(5, 5);
                Dust dust = Dust.NewDustPerfect(Projectile.position, 15, velocity, Scale: Main.rand.NextFloat(1f, 1.8f));
                dust.noGravity = true;
            }
        }
    }
    public class StarDestroyerStarYellow : StarDestroyerStar
    {
        int Timer = 0;
        public override string Texture => "Macrocosm/Content/Projectiles/Friendly/Ranged/StarDestroyerStar";
        public override void AI()
        {
            Projectile.rotation += 0.5f;
            colour = new Color(255, 180, 25);
            Projectile.SetTrail<StarTrailAlt>();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.CritChance += 2;
            Timer++;
            if (Timer % 6 == 0)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.velocity.RotatedByRandom(Math.PI * 2) * 0.3f, 16);//16 and 17 are the star gores.
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity.RotatedByRandom(Math.PI * 2) * 0.3f, 17);
            }
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = Projectile.Size / 2f;
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            Projectile.GetTrail().Draw(Projectile.Size / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (colour) * 0.4f, Projectile.rotation, origin, Projectile.scale * 1.8f, SpriteEffects.None, 0);
            return false;
        }
        public override void OnKill(int timeLeft)
        {

            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(5, 5);
                Dust dust = Dust.NewDustPerfect(Projectile.position, 259, velocity, Scale: Main.rand.NextFloat(1f, 1.8f));
                dust.noGravity = true;
            }
            var explosion = Particle.CreateParticle<TintableExplosion>(Explosion =>
            {
                Explosion.Position = Projectile.Center;
                Explosion.DrawColor = colour.WithOpacity(0.8f);
                Explosion.Scale = 1f;
                Explosion.NumberOfInnerReplicas = 5;
                Explosion.ReplicaScalingFactor = 0.25f;
            });
        }
    }

}

