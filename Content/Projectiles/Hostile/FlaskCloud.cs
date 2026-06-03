using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Buffs;

namespace Macrocosm.Content.Projectiles.Hostile;
public class FlaskCloud : ModProjectile
{

    public ref float cloudType => ref Projectile.ai[0];

    public enum Clouds
    {
        acid,
        oil,
        distortion
    }

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 9;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.timeLeft = 500;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }

    private bool CheckForPrometheum()
    {
        // thanks hallam
        foreach (Projectile otherProj in Main.ActiveProjectiles)
        {
            if (otherProj.type == ModContent.ProjectileType<ZombieEngineerExplosion>() && Projectile.Hitbox.Intersects(otherProj.Hitbox))
            {
                return true;
            }
        }
        return false;
    }

    private int timer = 0;
    private bool exploding = false;
    public override void AI()
    {
        timer++;
        switch (cloudType)
        {
            case (float)Clouds.acid:
                if (timer <= 10)
                {
                    Projectile.frame = 0;
                }
                else if (timer <= 20)
                {
                    Projectile.frame = 1;
                }
                else if (timer <= 30)
                {
                    Projectile.frame = 2;
                }
                else
                {
                    timer = 0;
                }
                break;
            case (float)Clouds.oil:
                if (timer <= 10)
                {
                    Projectile.frame = 3;
                }
                else if (timer <= 20)
                {
                    Projectile.frame = 4;
                }
                else if (timer <= 30)
                {
                    Projectile.frame = 5;
                }
                else
                {
                    timer = 0;
                }
                break;
            case (float)Clouds.distortion:
                if (timer <= 10)
                {
                    Projectile.frame = 6;
                }
                else if (timer <= 20)
                {
                    Projectile.frame = 7;
                }
                else if (timer <= 30)
                {
                    Projectile.frame = 8;
                }
                else
                {
                    timer = 0;
                }
                break;
        }
            
        Projectile.velocity -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.08f;
        
        if (cloudType == (float)Clouds.oil)
        {
            ApplyOilToPlayers();
            Projectile.hostile = false;
            if (!exploding)
            {
                if (CheckForPrometheum())
                {
                    exploding = true;
                }
            }
            else
            {
                timer++;
                if (timer >= 60) 
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<ZombieEngineerExplosion>(), Projectile.damage, Projectile.knockBack);
                    Projectile.Kill();
                }
            }
        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        switch (cloudType)
        {
            case (float)Clouds.acid:
                target.AddBuff(BuffID.Poisoned, 270, false);
                break;
            case (float)Clouds.distortion:
                target.AddBuff(BuffID.VortexDebuff, 270, false);
                break;
        }
    }

    public void ApplyOilToPlayers()
    {
        foreach (Player player in Main.ActivePlayers)
        {
            if (Projectile.Hitbox.Intersects(player.Hitbox))
            {
                player.AddBuff(BuffID.Oiled, 270, false);
            }
        }
    }
}
