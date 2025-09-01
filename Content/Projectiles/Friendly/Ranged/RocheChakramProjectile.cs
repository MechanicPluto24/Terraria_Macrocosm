using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Ranged;

public class RocheChakramProjectile : ModProjectile
{
    public ref float Speed => ref Projectile.ai[0];

    public int ExplosionTimer
    {
        get => (int)Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 46;
        Projectile.height = 46;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.localNPCHitCooldown = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.timeLeft = 600;

        Speed = 25;
        ExplosionTimer = 0;
    }

    public bool ShouldExplode()
    {
        Player player = Main.player[Projectile.owner];
        bool foundOlder = true;
        if (player.ownedProjectileCounts[ModContent.ProjectileType<RocheChakramProjectile>()] > 3)
        {
            for (int i = 0; i <= 1000; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (i != Projectile.whoAmI)
                    if (projectile.type == ModContent.ProjectileType<RocheChakramProjectile>() && projectile.owner == Projectile.owner && projectile.active)
                        if (projectile.timeLeft < Projectile.timeLeft)
                            foundOlder = false;
            }

            return foundOlder;
        }
        return false;
    }

    private bool spawned;
    public override void AI()
    {
        if (!spawned)
        {
            Speed = 25;
            spawned = true;
        }

        for (int i = 0; i < 2; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteBrightDust>());
            dust.velocity.X = Main.rand.Next(-30, 31) * 0.02f;
            dust.velocity.Y = Main.rand.Next(-30, 30) * 0.02f;
            dust.scale *= 1f + Main.rand.Next(-12, 13) * 0.01f;
            dust.noGravity = true;
        }

        if (Main.rand.NextBool(4))
        {
            Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteDust>());
            dust2.velocity.X = Main.rand.Next(-30, 31) * 0.02f;
            dust2.velocity.Y = Main.rand.Next(-30, 30) * 0.02f;
            dust2.scale *= 1f + Main.rand.Next(-12, 13) * 0.005f;
            dust2.noGravity = true;
        }
;

        if (ShouldExplode())
        {
            ExplosionTimer++;
        }

        Projectile.rotation += 0.65f;
        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
        Projectile.velocity *= Speed;

        if (Speed > -35f)
            Speed -= 0.2f;

        if (Speed < 0f)
        {
            Projectile.velocity = (Main.player[Projectile.owner].Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
            Projectile.velocity *= -Speed;
            if (Projectile.Distance(Main.player[Projectile.owner].Center) < 50f)
            {
                Projectile.Kill();
                if (Main.myPlayer == Projectile.owner)
                    Main.player[Projectile.owner].QuickSpawnItem(Projectile.GetSource_FromAI(), ModContent.ItemType<RocheChakram>());
            }
        }

        if (ExplosionTimer > 10)
            Explode();
    }

    public void Explode()
    {
        for (int i = 0; i < 2; i++)
        {
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 18f, ModContent.ProjectileType<RocheSpike>(), (int)(Projectile.damage / 2.5), 1f, Main.myPlayer, ai0: 0f);
        }

        for (int i = 0; i < (int)15; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteBrightDust>());
            dust.velocity.X = Main.rand.Next(-30, 31) * 0.02f;
            dust.velocity.Y = Main.rand.Next(-30, 30) * 0.02f;
            dust.scale *= 1f + Main.rand.Next(-12, 13) * 0.01f;
            dust.noGravity = true;

            Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteDust>());
            dust2.velocity.X = Main.rand.Next(-30, 31) * 0.02f;
            dust2.velocity.Y = Main.rand.Next(-30, 30) * 0.02f;
            dust2.scale *= 1f + Main.rand.Next(-12, 13) * 0.01f;
            dust2.noGravity = true;
        }

        Projectile.Kill();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
            float dashFactor = MathHelper.Clamp(Projectile.velocity.Length(), 0, 20) / 20f;
            float trailFactor = (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
            Color color = Projectile.GetAlpha(lightColor) * dashFactor * trailFactor;
            SpriteEffects effect = Projectile.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, drawPos, texture.Frame(1, Main.projFrames[Type], frameY: Projectile.frame), color * 0.6f, Projectile.oldRot[i], Projectile.Size / 2, Projectile.scale, effect, 0f);
        }

        return true;
    }
}