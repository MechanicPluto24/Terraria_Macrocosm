using Macrocosm.Common.Bases;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class InvarBullet : RicochetBullet
    {
        public override int RicochetCount => 3;

        public override float RicochetSpeed => 15f;

        public override void SetStaticDefaults()
        {
        }

        public override void SetProjectileDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.timeLeft = 600;
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
    }
}
