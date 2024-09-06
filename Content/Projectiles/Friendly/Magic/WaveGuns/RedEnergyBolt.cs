using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic.WaveGuns
{
    public class RedEnergyBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
          public override string Texture => Macrocosm.EmptyTexPath;
        

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height =30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft =300;
            Projectile.SetTrail<RedEnergyTrail>();
        }
        int Timer=0;
        
        public override void AI()
        { 
            Projectile.velocity=Projectile.velocity.RotatedBy(MathHelper.ToRadians((int)(Math.Cos(Timer/10)/10)));
            Timer++;          
            Lighting.AddLight(Projectile.Center, new Color(255, 0, 0).ToVector3());
        }
        public override void OnKill(int timeLeft)
        {
            int count = 10;
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.RedTorch, velocity, Scale: Main.rand.NextFloat(1f, 1.6f));
                dust.noGravity = true;
            }
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Light3").Value;
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            Projectile.GetTrail().Draw(Projectile.Size / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(255,150,150,0), Projectile.velocity.ToRotation()+(Timer/8), texture.Size() / 2f, Projectile.scale*0.05f, SpriteEffects.None, 0f);
       

            return false;
        }
    }
    
}

