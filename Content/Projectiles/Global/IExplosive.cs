using Macrocosm.Common.Utils;
using Terraria;

namespace Macrocosm.Content.Projectiles.Global
{
    /// <summary> Interface for all ModProjectile types that explode in contact with the ground or an enemy </summary>
    public interface IExplosive
    {
        public int OriginalWidth { get; }
        public int OriginalHeight { get; }

        /// <summary> The explosion blast radius </summary>
        public float BlastRadius { get; }

        /// <summary> Called when the projectile hits anything. By default, explodes the projectile with the corresponding blast radius. Implement this only for otherwise special behaviour. </summary>
        public void OnHit(Projectile projectile)
        {
            projectile.Explode(BlastRadius);
        }
    }
}
