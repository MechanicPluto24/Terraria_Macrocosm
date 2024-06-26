﻿using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class ArtemiteStar : Particle
    {
        public override string TexturePath => Macrocosm.EmptyTexPath;

        public int StarPointCount { get; set; } = 2;

        public float FadeInFactor { get; set; } = 1.25f;
        public float FadeOutFactor { get; set; } = 0.775f;

        public float Alpha = 0.3f;
        public Color Color = new(130, 220, 199);

        bool fadeIn = true;
        float defScale;
        float actualScale;


        public override void OnSpawn()
        {
            defScale = Scale;
            actualScale = 0.1f;
        }

        public override void AI()
        {
            if (fadeIn)
            {
                actualScale *= FadeInFactor;

                if (actualScale > defScale)
                    fadeIn = false;
            }
            else
            {
                actualScale *= FadeOutFactor;
            }

            Lighting.AddLight(Center, Color.ToVector3() * actualScale * 0.5f);

            if (actualScale < 0.2f && !fadeIn)
                Kill();
        }

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.DrawStar(Position - screenPosition, StarPointCount, Color, new Vector2(0.5f * actualScale, actualScale), Rotation);
            return false;
        }
    }
}
