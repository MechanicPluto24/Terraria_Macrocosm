
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Macrocosm.Content.NPCs.Enemies.Moon;
using Terraria.Audio;
using System.Collections;

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





                break;
            case (float)Flasks.oil:





                break;
            case (float)Flasks.prometheum:
                



                break;
            case (float)Flasks.distortion:
                





                break;
            case (float)Flasks.confetti:
                





                break;
        }
    }
}
