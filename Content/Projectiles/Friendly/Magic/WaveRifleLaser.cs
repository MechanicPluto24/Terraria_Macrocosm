using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class WaveRifleLaser : RicochetBullet
    {
        public override void SetStaticDefaults()
        {
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override bool CanRicochet() => false;
        bool HitSomething=false;
        int Timer=0;
        public override void SetProjectileDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.position, new Color(255, 0, 255).ToVector3() * 0.6f);
            
            Timer++;
            if (Timer>90)
            {
                for (int i = 0; i < 5; i++)
                    { 
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.ToRadians(i*72)) *20f, ModContent.ProjectileType<WaveStar>(), Projectile.damage/3, 0f, -1);
                    }
                    Projectile.Kill();
            }
        
        }

    }
    public class WaveStar : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

       

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawMagicPixelTrail(Vector2.Zero, 5f, 1f,new Color(255,0,255), new Color(255,0,255).WithOpacity(0f));

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Main.spriteBatch.DrawStar(Projectile.Center - Main.screenPosition, 1, new Color(255,0,255), 0.6f, Projectile.rotation + MathHelper.PiOver2, entity: true);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }


        

        public override void AI()
        {
        Projectile.velocity*=0.995f;
        Projectile.rotation = Projectile.velocity.ToRotation();
        }

       
    }

}