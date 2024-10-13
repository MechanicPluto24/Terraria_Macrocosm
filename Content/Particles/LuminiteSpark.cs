using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles
{
    public class LuminiteSpark : Particle
    {
        public override string TexturePath => Macrocosm.EmptyTexPath;
        public override ParticleDrawLayer DrawLayer => ParticleDrawLayer.AfterProjectiles;

        private float origScale;

        public override void SetDefaults()
        {
            TimeToLive = 250;
        }

        public override void OnSpawn()
        {
            origScale = Scale.X;
            Color = new List<Color>() {
                    new(44, 209, 147),
                    new(0, 141, 92)
            }.GetRandom();
        }

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            Texture2D glow = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            spriteBatch.Draw(glow, Center - screenPosition, null, Color.WithOpacity(0.8f), Rotation, glow.Size() / 2, Scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            float speed = Velocity.LengthSquared() * 0.9f;
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
            Scale = new Vector2(Math.Clamp(speed, 0, 2), Math.Clamp(speed, 0, 5)) * 0.04f * origScale;

            Velocity *= 0.91f;

            Lighting.AddLight(Center, new Vector3(1f, 1f, 1f) * Scale.X * 0.02f);

            //if (ScaleV.Y < 0.1f)
            //	Kill();
        }
    }
}
