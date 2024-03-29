using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class CelestialStar : Particle
    {
        public override string TexturePath => Macrocosm.EmptyTexPath;

        public float Alpha = 0.3f;
        public Color Color = Color.White;
        public Color? SecondaryColor = Color.White;

        public BlendState BlendStateOverride = null;

        bool fadeIn = true;
        float defScale;
        float actualScale;

        public override void OnSpawn()
        {
            defScale = Scale;
            actualScale = 0.5f;
            Alpha = 1f;
        }

        public override void AI()
        {
            if (fadeIn)
            {
                actualScale *= 1.45f;
                if (actualScale > defScale)
                    fadeIn = false;
            }
            else
            {
                actualScale *= 0.92f;
            }

            Velocity *= 0.98f;

            Lighting.AddLight(Center, Color.ToVector3() * (actualScale / defScale));

            if (actualScale < 0.2f)
                Kill();
        }

        SpriteBatchState state;
        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            if (BlendStateOverride is not null)
            {
                state.SaveState(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(BlendStateOverride, state);
            }

            spriteBatch.DrawStar(Position - screenPosition, 2, Color * (actualScale / defScale), actualScale, Rotation);

            if (BlendStateOverride is not null)
            {
                spriteBatch.End();
                spriteBatch.Begin(state);
            }
            return false;
        }
    }
}
