using Microsoft.Xna.Framework;
using SteelSeries.GameSense;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Macrocosm.Common.Utils;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Trails;
using Macrocosm.Common.DataStructures;
using Terraria.GameContent;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class TrailStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 18;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        private float currentAmplitude; //Current amplitude of the sine wave
        private float frequency = 0.125f;  // The frequency of the sine wave movement
        private float elapsedTime; // Pretty self-explanatory
        private float amplitudeDecay = 0.9f; // Percentage of amplitude retained after each cycle
        private int cycleCount = 0; // Keep track of the cycles so we can dimish the amplitude on a per-cycle basis
        private TrailScepterTrail trail;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.timeLeft = 600;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            AIType = ProjectileID.Bullet;
            Projectile.light = 0.7f;
            Projectile.penetrate = 5;
            trail = new();
        }

        public override void OnSpawn(IEntitySource source)
        {
            currentAmplitude = Main.rand.NextFloat(-20f, 20f);
        }


        public override void AI()
        {
            if((int)elapsedTime%4==0)
            {
                Dust d =Dust.NewDustDirect(Projectile.Center, 16, 16, DustID.GemDiamond, Scale: Main.rand.NextFloat(0.2f,1.5f));
                d.noGravity=true;
                d.velocity=Main.rand.NextVector2Circular(1, 4);
            }
            elapsedTime += 1f;

           
            float phase = frequency * elapsedTime;

            // Check for completed cycles (2π radians is one complete cycle)
            if (phase >= (cycleCount + 1) * MathHelper.TwoPi)
            {
                cycleCount++;
                
                currentAmplitude *= amplitudeDecay;
            }

            float sineOffset = currentAmplitude * (float)Math.Sin(phase);
            Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * sineOffset;
            Projectile.position += offset;

        }
        private SpriteBatchState state;

        public override bool PreDraw(ref Color lightColor)
        {
            state.SaveState(Main.spriteBatch);
            Texture2D tex = TextureAssets.Projectile[Type].Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            trail?.Draw(Projectile, Projectile.Size / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = tex.Size() / 2f;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (Color.White) * Projectile.Opacity, Projectile.rotation- MathHelper.Pi/2, origin, Projectile.scale*0.6f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (new Color(0, 217, 102, 0)) * 0.4f * Projectile.Opacity, Projectile.rotation - MathHelper.Pi/2, origin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Dust d =Dust.NewDustDirect(Projectile.Center, 16, 16, DustID.GemDiamond, Scale: Main.rand.NextFloat(0.2f,1.5f));
            d.noGravity=true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
        }
    }
}
