using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Macrocosm.Common.CrossMod;

namespace Macrocosm.Content.Projectiles.Friendly.Magic;

public class SolarGlob : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Redemption.AddElementToProjectile(Type, Redemption.ElementID.Fire);
        Redemption.AddElementToProjectile(Type, Redemption.ElementID.Celestial);

    }

    public override void SetDefaults()
    {
        Projectile.scale = 0.5f;
        Projectile.width = 90;
        Projectile.height = 45;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = 500;
    }

    public override void AI()
    {
        Projectile.velocity.X =0f;
        Projectile.rotation = 0f;
        Projectile.velocity.Y += 0.5f * (0.3f + 0.7f * MacrocosmSubworld.GetGravityMultiplier(Projectile.Center));

        if (Main.rand.NextBool(12))
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare);

        if (Projectile.timeLeft < 50)
            Projectile.Opacity -= 0.02f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity *=0f;
        return false;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White*Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White*0.5f*Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, (float)(Projectile.scale*((Math.Sin(Main.time*3f)*0.2f)+1.2f)), Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);

        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        int impactDustCount = Main.rand.Next(40, 70);
        for (int i = 0; i < impactDustCount; i++)
        {
            int dist = 160;
            Vector2 dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
            float distFactor = (Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist));
            Vector2 velocity = (Projectile.Center - dustPosition).SafeNormalize(default) * -6f;
            Particle.Create<DustParticle>((p =>
            {
                p.DustType = DustID.SolarFlare;
                p.Position = dustPosition;
                p.Velocity = velocity;
                p.Scale = new Vector2(Main.rand.NextFloat(1.2f, 2f));
                p.NoGravity = true;
                p.NormalUpdate = true;
            }));
        }
    }
}
