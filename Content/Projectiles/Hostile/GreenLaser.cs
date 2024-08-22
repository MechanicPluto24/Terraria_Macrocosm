using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class GreenLaser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;

            ProjectileSets.HitsTiles[Type] = true;
        }
        
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BulletDeadeye);
            AIType = ProjectileID.Bullet;
            Projectile.width = 4;
            Projectile.height = 4;
        }
        public  Color colour =new Color(100, 255, 255,0);
       
        public override bool PreAI()
        {


            Lighting.AddLight(Projectile.position, colour.ToVector3() * 0.5f);

            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawMagicPixelTrail(Vector2.Zero, 2.8f, 0.5f, colour * lightColor.GetBrightness(), colour * lightColor.GetBrightness());
            return true;
        }
    }
}
