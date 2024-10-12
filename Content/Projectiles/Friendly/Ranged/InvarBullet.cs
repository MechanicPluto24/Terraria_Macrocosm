using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class InvarBullet : RicochetBullet
    {
        public override int RicochetCount => 3;

        public override float RicochetSpeed => 15f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetProjectileDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 3;
            Projectile.frame = 0;
        }

        private bool spawned = false;
        public override void AI()
        {
            if (!spawned)
            {
                //Projectile.frame = Main.rand.Next(2); // second frame is ugly
                spawned = true;
            }
        }

        public override bool CanRicochet()
        {
            // The more invar bullets shot by this player there are...
            int otherCount = Main.projectile.Where((other) => other.type == Type && other.owner == Projectile.owner).Count();
            float cap = 25f;
            float chance = MathHelper.Clamp(otherCount / cap, 0f, 1f);

            // ...the lower the chance to ricochet
            if (Main.rand.NextFloat() < chance)
                return false;

            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.damage -= 5;
        }

        public override void OnHitNPC(bool didRicochet, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage -= 5;
        }

        public override void OnHitNPCEffect(bool didRicochet, NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < Main.rand.Next(10, 20); i++)
                Dust.NewDustPerfect(Projectile.position, ModContent.DustType<InvarBits>(), Projectile.oldVelocity.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f), Scale: 2.4f);

            if (didRicochet)
                SoundEngine.PlaySound(SFX.Ricochet with { Volume = 0.3f }, Projectile.position);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < Main.rand.Next(10, 20); i++)
                Dust.NewDustPerfect(Projectile.position + Projectile.oldVelocity * 1.2f, ModContent.DustType<InvarBits>(), Projectile.oldVelocity.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f), Scale: 2.4f);

            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawMagicPixelTrail(new Vector2(0, 40), 2.1f, 0.4f, new Color(121, 92, 18) * Projectile.Opacity, new Color(121, 92, 18, 0) * Projectile.Opacity);
            return true;
        }
    }
}
