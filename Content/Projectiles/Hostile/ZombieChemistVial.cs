
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Macrocosm.Content.NPCs.Enemies.Moon;

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

    }
}
