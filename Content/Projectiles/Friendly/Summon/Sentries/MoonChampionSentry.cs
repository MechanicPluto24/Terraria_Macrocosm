using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon.Sentries;

public class MoonChampionSentry : ModProjectile
{
    private static Asset<Texture2D> turretTexture;
    private static Asset<Texture2D> backTexture;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 40;
        Projectile.height = 46;
        Projectile.tileCollide = true;
        Projectile.sentry = true;
        Projectile.timeLeft = Projectile.SentryLifeTime;

        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.penetrate = -1;
    }

    public override bool? CanDamage() => false;

    private bool spawned = false;
    private float turretRotation = 0f;
    private int timer = 0;
    private float offset = 0f;

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (!spawned)
        {
            spawned = true;
            Projectile.spriteDirection = owner.Center.X < Projectile.Center.X ? -1 : 1;
        }

        Projectile.velocity.Y += 1;
        SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
        if (foundTarget && Projectile.spriteDirection == -1 ? targetCenter.X < Projectile.Center.X : targetCenter.X > Projectile.Center.X)
        {
            Vector2 turningVector = (targetCenter - Projectile.Center).SafeNormalize(Vector2.UnitX);
            turretRotation = (new Vector2(5, 0).RotatedBy(turretRotation) + (turningVector * 0.6f)).ToRotation();
            timer++;
            if (timer == 60)
            {
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(-14, 0), new Vector2(16f, 0).RotatedBy(turretRotation), ProjectileID.RocketI, Projectile.damage / 2, 1f, Main.myPlayer);
                p.DamageType = DamageClass.Summon;
                timer = 0;
                offset = 5f;
            }
        }
        else
        {

            turretRotation = MathHelper.Lerp(turretRotation, Projectile.spriteDirection == 1 ? 0 : MathHelper.Pi, 0.08f);
            timer = 0;
        }

        offset *= 0.9f;
    }

    private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
    {
        distanceFromTarget = 700f;
        targetCenter = Projectile.position;
        foundTarget = false;

        // This code is required if your minion weapon has the targeting feature
        if (owner.HasMinionAttackTargetNPC)
        {
            NPC npc = Main.npc[owner.MinionAttackTargetNPC];
            float between = Vector2.Distance(npc.Center, Projectile.Center);

            // Reasonable distance away so it doesn't target across multiple screens
            if (between < 2000f)
            {
                distanceFromTarget = between;
                targetCenter = npc.Center;
                foundTarget = true;
            }
        }

        if (!foundTarget && Projectile.alpha < 1)
        {
            // This code is required either way, used for finding a target
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.CanBeChasedBy() && Vector2.Distance(npc.Center, Projectile.Center) < 4000f)
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center);
                    bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                    bool inRange = between < distanceFromTarget;
                    bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                    // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                    // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                    bool closeThroughWall = between < 100f;

                    if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        backTexture ??= ModContent.Request<Texture2D>(Texture + "_Back");
        turretTexture ??= ModContent.Request<Texture2D>(Texture + "_Turret");
        SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        SpriteEffects effect2 = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

        Main.EntitySpriteDraw(backTexture.Value, Projectile.Center - Main.screenPosition, null, lightColor, 0f, backTexture.Size() / 2, Projectile.scale, effect, 0f);
        Main.EntitySpriteDraw(turretTexture.Value, Projectile.Center + new Vector2(0, -14) - Main.screenPosition + new Vector2(-offset, 0).RotatedBy(turretRotation), null, lightColor, turretRotation, turretTexture.Size() / 2, Projectile.scale, effect2, 0f);

        return true;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        fallThrough = false;
        return true;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (oldVelocity.Y > 0)
            Projectile.velocity.Y = 0;
        return false;
    }

}
