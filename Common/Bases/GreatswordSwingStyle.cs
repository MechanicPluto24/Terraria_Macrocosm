using Microsoft.Xna.Framework;

namespace Macrocosm.Common.Bases
{
    public abstract class GreatswordSwingStyle
    {
        /// <summary>
        /// Defines how a <see cref="GreatswordHeldProjectile"/> should act after having been charged.
        /// </summary>
        /// <returns><c>true</c> - keep the projectile alive <br/> <c>false</c> - kill projectile</returns>
        public abstract bool Update(ref float armRotation, ref float projectileRotation, float charge);

        public virtual void PreDrawSword(GreatswordHeldProjectile greatswordProjectile, Color lightColor) { }
        public virtual void PostDrawSword(GreatswordHeldProjectile greatswordProjectile, Color lightColor) { }
    }
}
