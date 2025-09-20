using Macrocosm.Common.CrossMod;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon.Turrets;

public class LaserTurretProjectile : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 1;

        Redemption.AddElement(Projectile, Redemption.ElementID.Fire);
        Redemption.AddElement(Projectile, Redemption.ElementID.Thunder);
    }

    public int AITimer = 0;

    private const float MaxBeamLength = 3000f;
    private const float BeamHitboxCollisionWidth = 22f;
    private float transparency = 0f;

    public override void SetDefaults()
    {
        Projectile.width = 28;
        Projectile.height = 22;
        Projectile.hide = true;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.alpha = 255;
        Projectile.timeLeft = 35;
    }

    public int Owner
    {
        get => (int)Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

    public override void OnKill(int timeLeft)
    {
        NPC owner = Main.npc[Owner];
        if (owner.active && owner.type == ModContent.NPCType<LaserTurret>())
            owner.ai[0] = 0f;
    }

    public override bool ShouldUpdatePosition() => false;
    public override void AI()
    {
        NPC owner = Main.npc[Owner];
        if (!owner.active || owner.type != ModContent.NPCType<LaserTurret>())
            Projectile.Kill();

        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
        Projectile.rotation = Projectile.velocity.ToRotation();
        AITimer++;

        if (transparency < 1f && AITimer < 25)
            transparency += 0.1f;
        else
            transparency -= 0.2f;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindNPCs.Add(index);
    }

    public override Color? GetAlpha(Color lightColor)  => Color.White * (1f - Projectile.alpha / 255f);

    private SpriteBatchState state;
    public override bool PreDraw(ref Color lightColor)
    {
        // If the beam doesn't have a defined direction, don't draw anything.
        if (Projectile.velocity == Vector2.Zero)
            return false;

        LaserTurret turret = Main.npc[Owner].ModNPC as LaserTurret;
        if (turret is null)
            return false;

        state.SaveState(Main.spriteBatch);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(BlendState.Additive, state);

        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 aim = Projectile.velocity.SafeNormalize(Vector2.UnitX);
        Vector2 start = Main.npc[Owner].Center + turret.TurretHeight + new Vector2(0, -1).RotatedBy(aim.ToRotation());
        Vector2 end = start + aim * Utility.CastLength(start, aim, 2000f, false);
        Color color = new Color(255, 255, 255).WithAlpha(225) * transparency * (0.9f + (0.1f * MathF.Sin(AITimer)));

        for (int i = 0; i < 4; i++)
        {
            float scaleFactor = 1f + i * 0.3f;
            float alphaFactor = 1f - i * 0.25f;
            Utility.DrawBeam(texture, start - Main.screenPosition, end - Main.screenPosition, new Vector2(Projectile.scale * scaleFactor), color * alphaFactor, new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
        }


        Main.spriteBatch.End();
        Main.spriteBatch.Begin(state);
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host Prism), that's good enough.
        if (projHitbox.Intersects(targetHitbox))
            return true;

        // Otherwise, perform an AABB line collision check to check the whole beam.
        float _ = float.NaN;
        Vector2 beamEndPos = Projectile.Center + Projectile.velocity * Utility.CastLength(Projectile.Center, new Vector2(1, 0).RotatedBy(Projectile.rotation), 2000f, false);
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, BeamHitboxCollisionWidth * Projectile.scale, ref _);
    }
}