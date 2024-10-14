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
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override string Texture => Macrocosm.EmptyTexPath;

        private bool summon = false;
        private int timer = 0;

        public override void SetStaticDefaults()
        {
        }

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

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            summon = true;
            return false;
        }

        public override void AI()
        {
            Particle.Create<TintableFire>(p =>
            {
                p.Position = Projectile.position + new Vector2(0, Projectile.height / 2);
                p.Velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                p.Color = new Color(100, 255, 255, 0);
                p.Scale = new(0.12f);
            });

            Projectile.velocity.Y += 1f;

            if (summon == true)
            {
                Projectile.velocity *= 0f;
                timer++;
            }

            if (timer > 30)
            {
                int npcToSummon = Main.rand.Next(1, 4);
                switch (npcToSummon)
                {
                    case 1:
                        NPC.NewNPCDirect(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<XenoHive>(), 0, 0f);
                        break;
                    case 2:
                        NPC.NewNPCDirect(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<LunarChimera>(), 0, 0f);
                        break;
                    case 3:
                        NPC.NewNPCDirect(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<Scatterbrained>(), 0, 0f);
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < 20; i++)
                {
                    Particle.Create<TintableFire>(p =>
                    {
                        p.Position = Projectile.position + new Vector2(0, Projectile.height / 2);
                        p.Velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 5f);
                        p.Color = new Color(34, 221, 151, 0);
                        p.Scale = new(0.16f);
                    });
                }

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
