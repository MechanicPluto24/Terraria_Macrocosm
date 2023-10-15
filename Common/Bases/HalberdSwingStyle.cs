using Microsoft.Xna.Framework;

namespace Macrocosm.Common.Bases
{
    public abstract class HalberdSwingStyle
    {
        public abstract bool Update(ref float armRotation, ref float projectileRotation, float charge);
        public virtual void PreDrawHalberd(HalberdHeldProjectile halberdProjectile, Color lightColor) { }
        public virtual void PostDrawHalberd(HalberdHeldProjectile halberdProjectile, Color lightColor) { }
    }
}
