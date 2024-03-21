using Microsoft.Xna.Framework;
using System;

namespace Macrocosm.Common.Bases
{
	public abstract class GreatswordSwingStyle
	{
		/// <summary>
		/// Defines how a <see cref="GreatswordHeldProjectile"/> should act after having been charged.
		/// </summary>
		/// <returns><c>true</c> - keep the projectile alive <br/> <c>false</c> - kill projectile</returns>
		public abstract bool Update(ref float armRotation, ref float projectileRotation, float charge);

		public virtual bool PreDrawSword(GreatswordHeldProjectile greatswordProjectile, Color lightColor, ref Color? drawColor ) { return true; }
		public virtual void PostDrawSword(GreatswordHeldProjectile greatswordProjectile, Color lightColor) { }
	}

    public class DefaultGreatswordSwingStyle : GreatswordSwingStyle
    {
        protected float? maxSwingTimer = null;
        protected float swingTimer = 1f;
        protected float SwingTime => swingTimer / maxSwingTimer.Value;

        private const float MAX_REST_TIMER = 8f;
        private float restTimer = 1f;
        private float? initialArmRotation = null;

        public override bool Update(ref float armRotation, ref float projectileRotation, float charge)
        {
            maxSwingTimer ??= 88 - 20 * charge;
            if (swingTimer >= maxSwingTimer)
            {
                restTimer++;
                if (restTimer >= MAX_REST_TIMER)
                {
                    return false;
                }

                armRotation += 0.01f;
                return true;
            }

            initialArmRotation ??= armRotation;

            float progress = MathF.Pow(MathF.Sin(MathHelper.PiOver2 * SwingTime), 4);

            armRotation = MathHelper.Lerp(initialArmRotation.Value, initialArmRotation.Value + MathHelper.Pi * 1.15f, progress);
            projectileRotation = MathHelper.Lerp(projectileRotation, armRotation + MathHelper.Pi * 0.6f, progress);

            swingTimer++;
            return true;
        }
    }
}
