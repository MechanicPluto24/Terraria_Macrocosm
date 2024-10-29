using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles
{
    public class SeleniteSpark : LuminiteSpark
    {
        public override void OnSpawn()
        {
            base.OnSpawn();
            Color = new List<Color>() {
                    new(177, 230, 207),
                    new(83, 129, 167),
                    new(157, 136, 169),
                    new(130, 179, 185)
            }.GetRandom();
        }
    }
}
