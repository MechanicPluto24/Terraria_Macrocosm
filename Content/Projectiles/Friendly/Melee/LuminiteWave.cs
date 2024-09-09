using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 120;
            Projectile.scale=1.5f;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = -1;
            Projectile.Opacity=0f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            trail = new();
        }
        float Speed=2f;
        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            
            SpriteEffects effects = Projectile.velocity.X> 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float Multiplier=1f;
             state.SaveState(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            trail?.Draw(Projectile, Projectile.Size / 2f);
             Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

          
            for(int i=0;i<4;i++)
            {
            Vector2 position = (Projectile.Center-(Projectile.velocity.SafeNormalize(Vector2.UnitX)*25f*i)) - Main.screenPosition;
			Main.EntitySpriteDraw(texture, position, null, new Color(213, 155, 148,0)*Projectile.Opacity*Multiplier, Projectile.rotation, texture.Size()/2, Projectile.scale*Multiplier,effects, 0f);
            Multiplier-=0.1f;
            position = (Projectile.Center-(Projectile.velocity.SafeNormalize(Vector2.UnitX)*25f*(float)(i+0.5))) - Main.screenPosition;
            Main.EntitySpriteDraw(texture, position, null, new Color(94, 229, 163,0)*Projectile.Opacity*Multiplier, Projectile.rotation, texture.Size()/2, Projectile.scale*Multiplier, effects, 0f);
            Multiplier-=0.1f;
            }

       

            

            

            return false;
        }
        
        public override void AI()
        {
            if(Projectile.timeLeft%2==0){
                Dust dust = Dust.NewDustDirect(Projectile.Center + new Vector2(0, Projectile.height/2), Projectile.width, Projectile.height,  ModContent.DustType<LuminiteBrightDust>(), Scale: 3);
                    dust.velocity =Projectile.velocity*0.5f;
                    dust.noLight = false;
                    dust.noGravity = true;
                    Dust dust2 = Dust.NewDustDirect(Projectile.Center - new Vector2(0, Projectile.height/2), Projectile.width, Projectile.height,  ModContent.DustType<LuminiteBrightDust>(), Scale: 3);
                    dust2.velocity =Projectile.velocity*0.5f;
                    dust2.noLight = false;
                    dust2.noGravity = true;

            }
           
            if (Projectile.Opacity<1f)
                Projectile.Opacity+=0.04f;
            if(Speed<28f)
                Speed*=1.5f;
            Projectile.velocity=Projectile.velocity.SafeNormalize(Vector2.UnitX)*Speed;
           
        }
    }
}