using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.NPCs.Enemies.Moon.MoonLich
{
    public class MoonLichNPCSummon : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {

        }
        Color colour = new Color(100, 255, 255);

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 900;
            Projectile.penetrate = -1;

            Projectile.tileCollide = true;
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        bool Summon = false;
        int Timer = 0;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Summon = true;
            return false;
        }
        public override void AI()
        {
            Particle.CreateParticle<TintableFire>(p =>
            {
                p.Position = Projectile.position;
                p.Velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                p.DrawColor = new Color(100, 255, 255, 0);
                p.Scale = 0.03f;
            });
            Projectile.velocity.Y += 1f;
            if (Summon == true)
            {
                Projectile.velocity *= 0f;
                Timer++;
            }
            if (Timer > 30)
            {
                NPC.NewNPCDirect(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<XenoHive>(), 0, 0f);
                Projectile.Kill();
            }

        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LuminiteBrightDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
            }
        }
    }
}
