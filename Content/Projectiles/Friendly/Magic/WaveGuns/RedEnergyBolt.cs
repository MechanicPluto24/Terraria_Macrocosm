using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic.WaveGuns
{
    public class RedEnergyBolt : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private RedEnergyTrail trail;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            trail = new();
        }

        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians((int)(Math.Cos(Timer / 10) / 10)));
            Timer++;
            Lighting.AddLight(Projectile.Center, new Color(255, 0, 0).ToVector3());
        }

        public override void OnKill(int timeLeft)
        {
            int count = 40;
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Dust dust = Dust.NewDustPerfect(Projectile.oldPosition + Projectile.Size / 2f + Projectile.oldVelocity, DustID.RedTorch, velocity, Scale: Main.rand.NextFloat(1f, 1.6f), Alpha: 0, newColor: new Color(255, 0, 0, 0));
                dust.noGravity = true;
            }

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    float progress = (float)i / (float)Projectile.oldPos.Length;
                    Vector2 pos = Projectile.oldPos[i];
                    Dust dust = Dust.NewDustDirect(pos, 20, 20, DustID.RedTorch, 0f, Projectile.oldVelocity.Y * 0.5f, Scale: Main.rand.NextFloat(1f, 1.6f) * (1f - progress), Alpha: 0, newColor: new Color(255, 0, 0, 0));
                    dust.noGravity = true;
                }
            }
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Trace1").Value;
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            trail?.Draw(Projectile, Projectile.Size / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 75, 75, 0), Projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() / 2f, Projectile.scale * 0.35f, SpriteEffects.None, 0f);
            return false;
        }
    }
}

