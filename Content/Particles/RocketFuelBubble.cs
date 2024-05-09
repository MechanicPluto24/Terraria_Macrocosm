using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Rockets.UI.Cargo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles
{
    public class RocketFuelBubble : Particle
    {
        public override void AI()
        {
            Scale -= 0.001f;

            if (Scale < 0.2f)
                 Active = false;
        }
    }
}
