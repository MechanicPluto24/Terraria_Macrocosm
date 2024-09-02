using Terraria;
using Terraria.Audio;
using Macrocosm.Content.Dusts;
using Terraria.ID;
using Macrocosm.Common.Subworlds;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Content.Projectiles.Friendly.Thrown
{
    public class LunarCrystalProjectile : ModProjectile
    {
        public override string Texture => "Macrocosm/Content/Items/Consumables/Throwable/LunarCrystal";
        public int BounceCounter
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        private int numBounces = 10;
        public override void SetDefaults()
        {
            Projectile.timeLeft=3600;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
        }
        public override bool PreAI()
        {
        Lighting.AddLight(Projectile.Center, new Color(0, 255, 180).ToVector3() * 2f);
        return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.oldVelocity = oldVelocity;
            Bounce(oldVelocity);
            if(BounceCounter> numBounces)
                Projectile.velocity.X*=0.8f;
            return false;
        }
        public override void AI(){
        Projectile.velocity.Y += 1f * MacrocosmSubworld.CurrentGravityMultiplier;
        Projectile.oldVelocity = Projectile.velocity * -1f;
        Projectile.rotation =   Projectile.velocity.ToRotation();
        }
        private void Bounce(Vector2 oldVelocity)
        {
             if (BounceCounter < numBounces){
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LuminiteBrightDust>());
                dust.velocity.X = Main.rand.Next(-70, 71) * 0.04f;
                dust.velocity.Y = Main.rand.Next(-70, 70) * 0.04f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.02f;
                dust.noGravity = true;
            }
             }
            BounceCounter++;
            float bounceFactor = 0.5f + 0.5f * BounceCounter / (float)numBounces;
            if (BounceCounter < numBounces)
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = oldVelocity.X * -bounceFactor;

                if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f)
                    Projectile.velocity.Y = oldVelocity.Y * -bounceFactor;

            
            }
        }

        
    }
}
