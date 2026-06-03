
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Macrocosm.Content.NPCs.Enemies.Moon;
using Terraria.Audio;
using System.Collections;
using Macrocosm.Common.DataStructures;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Macrocosm.Content.Dusts;

namespace Macrocosm.Content.Projectiles.Hostile;
public class ZombieChemistVial : ModProjectile
{

    public ref float flaskType => ref Projectile.ai[1];

    public enum Flasks // I have to redefine it here too for readability
    {
        acid,
        oil,
        prometheum,
        distortion,
        confetti
    }

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 5;
    }

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.timeLeft = 1200;
        Projectile.tileCollide = true;
        Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
        AIType = ProjectileID.DrManFlyFlask;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        switch (flaskType)
        {
            case (float)Flasks.acid:
                Projectile.frame = 0;
                break;
            case (float)Flasks.oil:
                Projectile.frame = 1;
                break;
            case (float)Flasks.prometheum:
                Projectile.frame = 2;
                break;
            case (float)Flasks.distortion:
                Projectile.frame = 3;
                break;
            case (float)Flasks.confetti:
                Projectile.frame = 4;
                break;
        }

        return true;

    }

    private float wave = 0;
    private int waveDirection = -1;
    public override void AI()
    {
        base.AI();
        if (flaskType == (float)Flasks.distortion)
        {
            if (wave > 0.3f)
            {
                wave = 0.3f;
                waveDirection = -1;
            }
            else if (wave < -0.3f)
            {
                wave = -0.3f;
                waveDirection = 1;
            }
            else
            {
                wave += 0.03f * waveDirection;
                Projectile.velocity.Y += wave;
            }
        }
    }

    public enum Clouds
    {
        acid,
        oil,
        distortion
    }

    public override void OnKill(int timeLeft)
    {
        // clouds for all except confetti and prometheum
        // prometheum is an explosion that sets you on fire and does more damage if you're oiled
        // oil does not do that much damage and leaves longer lingering oil clouds that are friendly until set on fire and it spreads between them
        // acid is the same but doesn't set on fire and gives acid, also lingers for less
        // distortion lingers for the shortest, gives player distortion
        SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);

        switch (flaskType)
        {
            case (float)Flasks.acid:

                // spawn green dusts
                // spawn 5 acid clouds in random directions
                for (int i = 0; i < 15; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f) * 5f;
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AcidFlaskDust>(), speed.X, speed.Y);
                }
                for (int i = 0; i < 5; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f) * 5f;
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.position, speed, ModContent.ProjectileType<FlaskCloud>(), Projectile.damage, Projectile.knockBack, -1, (float)Clouds.acid);
                }
                break;
            case (float)Flasks.oil:

                // spawn black dusts
                // spawn 6 oil clouds in random directions
                for (int i = 0; i < 24; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f) * 5f;
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<OilFlaskDust>(), speed.X, speed.Y);
                }
                for (int i = 0; i < 6; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f) * 5f;
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.position, speed, ModContent.ProjectileType<FlaskCloud>(), Projectile.damage, Projectile.knockBack, -1, (float)Clouds.oil);
                }


                break;
            case (float)Flasks.prometheum:

                // explode
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<ZombieEngineerExplosion>(), Projectile.damage, Projectile.knockBack);
                break;
            case (float)Flasks.distortion:

                // spawn 4 distortion clouds
                // spawn blue dusts
                for (int i = 0; i < 12; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f) * 5f;
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<DistortionFlaskDust>(), speed.X, speed.Y);
                }
                for (int i = 0; i < 4; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f) * 5f;
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.position, speed, ModContent.ProjectileType<FlaskCloud>(), Projectile.damage, Projectile.knockBack, -1, (float)Clouds.distortion);
                }


                break;
            case (float)Flasks.confetti:
                
                // spawn confetti
                for (int i = 0; i < 22; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f) * 5f;
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Confetti, speed.X, speed.Y);
                }
                break;
        }
    }
}
