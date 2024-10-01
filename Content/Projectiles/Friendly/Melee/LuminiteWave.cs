using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using System;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class LuminiteWave : ModProjectile
    {
        private LuminiteFireTrail trail;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public ref float Speed => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 120;
            Projectile.scale = 1.5f;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = -1;
            Projectile.Opacity = 1f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            trail = new();
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            SpriteEffects effects = Projectile.spriteDirection== 1 ? SpriteEffects.None : SpriteEffects.None;
            float multiplier = 1f;

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            trail?.Draw(Projectile, Projectile.Size / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            for (int i = 0; i < 4; i++)
            {
                Vector2 position = (Projectile.Center - (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 25f * i)) - Main.screenPosition;
                Main.EntitySpriteDraw(texture, position, null, new Color(213, 155, 148, 0) * Projectile.Opacity * multiplier, Projectile.rotation, texture.Size() / 2, Projectile.scale * multiplier, effects, 0f);
                multiplier -= 0.1f;
                position = (Projectile.Center - (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 25f * (float)(i + 0.5))) - Main.screenPosition;
                Main.EntitySpriteDraw(texture, position, null, new Color(94, 229, 163, 0) * Projectile.Opacity * multiplier, Projectile.rotation, texture.Size() / 2, Projectile.scale * multiplier, effects, 0f);
                multiplier -= 0.1f;
            }

            return false;
        }
        public override void OnKill(int timeLeft){
            for (int i=0; i<55;i++){
            Dust dust = Dust.NewDustDirect(Projectile.Center , Projectile.width, Projectile.height, ModContent.DustType<LuminiteBrightDust>(), Scale: 3);
            dust.velocity = Projectile.velocity.RotatedByRandom(MathHelper.Pi*2)*Main.rand.NextFloat(0.1f,0.5f);
            dust.noLight = false;
            dust.noGravity = true;
            }
        }
        public override void AI()
        {
            Projectile.rotation=Projectile.velocity.ToRotation();
            Projectile.direction = Math.Sign(Projectile.velocity.X);
            Projectile.spriteDirection=Projectile.direction;

            if (Projectile.timeLeft % 2 == 0)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center + new Vector2(0, Projectile.height / 2), Projectile.width, Projectile.height/2, ModContent.DustType<LuminiteBrightDust>(), Scale: 3);
                dust.velocity = Projectile.velocity * 0.5f;
                dust.noLight = false;
                dust.noGravity = true;
                Dust dust2 = Dust.NewDustDirect(Projectile.Center - new Vector2(0, Projectile.height / 2), Projectile.width, Projectile.height/2, ModContent.DustType<LuminiteBrightDust>(), Scale: 3);
                dust2.velocity = Projectile.velocity * 0.5f;
                dust2.noLight = false;
                dust2.noGravity = true;
            }

            if (Projectile.Opacity > 0f)
                Projectile.Opacity -= 0.004f;

            if (Speed >= 1f)
                Speed *= 0.995f;

            if(Projectile.Opacity<=0.5f)
                Projectile.Kill();

            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * Speed;
        }
    }
}