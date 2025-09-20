using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged;

public class IlmeniteProjectileDouble : ModProjectile
{
    public override string Texture => Macrocosm.EmptyTexPath;
    public float Strength
    {
        get => MathHelper.Clamp(Projectile.ai[0], 0f, 1f);
        set => Projectile.ai[0] = MathHelper.Clamp(value, 0f, 1f);
    }

    private bool exploded;

    float trailMultiplier = 0f;
    public Color colour1 = new Color(188, 89, 134);
    public Color colour2 = new Color(33, 188, 190);
    public Color colour3 = Color.Purple;
    public int helixProg;
    public Vector2 oldDevVect;

    public override void SetStaticDefaults()
    {
        ProjectileSets.HitsTiles[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = 256;
        Projectile.timeLeft = 360;
        Projectile.extraUpdates = 2;

        Projectile.CritChance = 16;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.extraUpdates += (int)Projectile.ai[1];
    }

    public override void AI()
    {
        if (trailMultiplier < 1f + (0.15f * Projectile.extraUpdates))
            trailMultiplier += 0.03f * (0.2f + Strength * 0.9f);
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        helixProg++;
    }

    SpriteBatchState state;
    public override bool PreDraw(ref Color lightColor)
    {
        var spriteBatch = Main.spriteBatch;

        state.SaveState(spriteBatch);
        spriteBatch.End();
        spriteBatch.Begin(BlendState.Additive, state);

        float deviation = 10 * MathF.Sin(MathHelper.Pi * helixProg / 20f);
        Vector2 DevVect = Utility.PolarVector(deviation, Projectile.rotation);

        Lighting.AddLight(Projectile.Center + DevVect, colour1.ToVector3());
        Lighting.AddLight(Projectile.Center - DevVect, colour2.ToVector3());

        float count = Projectile.velocity.LengthSquared() * trailMultiplier / 4;
        for (int n = 1; n < count; n++)
        {
            Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * n * 1.5f;
            deviation = 10 * MathF.Sin(MathHelper.Pi * (helixProg - n) / 20f);
            DevVect = Utility.PolarVector(deviation, Projectile.rotation);
            Color NColour1 = colour1 * (1f - (float)n / count);
            Color NColour2 = colour2 * (1f - (float)n / count);
            Utility.DrawStar(trailPosition + DevVect - Main.screenPosition, 1, NColour1, Projectile.scale * MathF.Pow(0.95f, n), Projectile.rotation, entity: true);
            Utility.DrawStar(trailPosition - DevVect - Main.screenPosition, 1, NColour2, Projectile.scale * MathF.Pow(0.95f, n), Projectile.rotation, entity: true);
        }

        spriteBatch.End();
        spriteBatch.Begin(state);

        return false;
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.FinalDamage /= (257 - Projectile.penetrate);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!exploded)
        {
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<IlmeniteExplosion>(), damageDone, hit.Knockback * 0.2f, Projectile.owner, Strength, target.Center.X, target.Center.Y);
            exploded = true;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (!exploded)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center + oldVelocity, Vector2.Zero, ModContent.ProjectileType<IlmeniteExplosion>(), Projectile.damage, Projectile.knockBack * 0.2f, Projectile.owner, Strength, Projectile.Center.X, Projectile.Center.Y);
            exploded = true;
        }
        return true;
    }
}
