using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Macrocosm.Content.Dusts;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ChampionsBladeSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public ref float projtype => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.Opacity=0f;
        }

        float Speed=22f;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.Opacity<1f)
                Projectile.Opacity+=0.02f;
            if(projtype==0f)
                Speed*=0.99f;
            if(projtype==1f&&Speed<28f)
                Speed*=1.01f;
            Projectile.velocity=Projectile.velocity.SafeNormalize(Vector2.UnitX)*Speed;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LuminiteBrightDust>());
                dust.velocity.X = Main.rand.Next(-30, 31) * 0.02f;
                dust.velocity.Y = Main.rand.Next(-30, 30) * 0.02f;
                dust.scale *= 1f + Main.rand.Next(-12, 13) * 0.01f;
                dust.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
		{
			
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Vector2 origin = texture.Size() / 2f;
            float Multiplier=1f;
          
            for(int i=0;i<10;i++)
            {
            Vector2 position = (Projectile.Center-(Projectile.velocity.SafeNormalize(Vector2.UnitX)*15f*i)) - Main.screenPosition;
			Main.EntitySpriteDraw(texture, position, null, new Color(255,255,255,0)*Projectile.Opacity*Multiplier, Projectile.rotation, origin, Projectile.scale*Multiplier*2.5f, SpriteEffects.None, 0f);
            Multiplier-=0.1f;
            }

			return false;
		}
    }
}