using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ArtemiteSpearProjectile : ModProjectile
    {
        protected virtual float HoldoutRangeMin => 50f;
        protected virtual float HoldoutRangeMax => 160f;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.scale = 0.95f;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            if (Projectile.timeLeft > duration)
                Projectile.timeLeft = duration;

            Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float halfDuration = duration * 0.5f;
            float progress;

            float holdoutOffset = 0f;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
                holdoutOffset = 45f; // Some offset so the handle doesn't return all the way back to the player
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * (HoldoutRangeMin + holdoutOffset), Projectile.velocity * HoldoutRangeMax, progress);

            // Apply proper rotation to the sprite.
            Projectile.rotation += MathHelper.ToRadians(Projectile.spriteDirection == -1 ? 40f : 135f);

            if (Projectile.timeLeft == halfDuration)
            {
                Vector2 shootPosition = Projectile.Center;
                Vector2 shootVelocity = Projectile.velocity * 15f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPosition, shootVelocity, ModContent.ProjectileType<ArtemiteSpearProjectileShoot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            // Spawn dust on use
            if (!Main.dedServ)
            {
                if (Main.rand.NextBool(3))
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteDust>(), Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, Scale: 0.6f);
                }
            }

            return false; // Don't execute vanilla AI.
        }
    }
}
