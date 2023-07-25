using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Macrocosm.Common.Bases
{
    internal class DefaultGreatswordSwingStyle : GreatswordSwingStyle
    {
        private const float MAX_SWING_TIMER = 35;
        private float swingTimer = 1;
        private float SwingTime => swingTimer / MAX_SWING_TIMER;
        private const float MAX_REST_TIMER = 6;
        private float restTimer = 1;
        private float RestTime => swingTimer / MAX_SWING_TIMER;
        private float? initialArmRotation = null;
        public override bool Update(ref float armRotation, ref float projectileRotation)
        {
            if (swingTimer >= MAX_SWING_TIMER)
            {
                restTimer++;
                if (restTimer >= MAX_REST_TIMER)
                {
                    return false;
                }

                return true;
            }

            if (initialArmRotation is null)
            {
                initialArmRotation = armRotation;
            }

            armRotation = MathHelper.Lerp(initialArmRotation.Value, initialArmRotation.Value + MathHelper.Pi, MathF.Pow(MathF.Sin(MathHelper.PiOver2 * SwingTime), 2f));
            projectileRotation = MathHelper.Lerp(projectileRotation, armRotation + MathHelper.Pi * 0.7f, SwingTime);

            swingTimer++;

            return true;
        }
    }
}
