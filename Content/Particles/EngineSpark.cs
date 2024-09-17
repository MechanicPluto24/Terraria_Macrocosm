using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles
{
    public class EngineSpark : Particle
    {
        public override string TexturePath => Macrocosm.EmptyTexPath;
        public override ParticleDrawLayer DrawLayer => ParticleDrawLayer.AfterProjectiles;

        public Color ColorOnSpawn { get; set; }
        public Color ColorOnDespawn { get; set; }

        private float Opacity;
        private float defScale = 0f;

        public override void SetDefaults()
        {
            TimeToLive = 10;

            Opacity = 0f;
            ColorOnSpawn = default;
            ColorOnDespawn = default;
        }

        public override void OnSpawn()
        {
            defScale = Scale.X;
        }

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Circle6").Value;
            Color color = Color.Lerp(ColorOnSpawn, ColorOnDespawn, (float)TimeLeft / TimeToLive);
            spriteBatch.Draw(glow, Center - screenPosition, null, color.WithOpacity(Opacity), Rotation, glow.Size() / 2, 0.0375f * Scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            float speed = Velocity.LengthSquared() * 0.4f;
            Rotation = Velocity.ToRotation();
            Scale = new Vector2(Math.Clamp(speed, 0, 5), Math.Clamp(speed, 0, 1)) * 0.11f * defScale;

            Opacity = 1f - Utility.InverseLerp(1f, 0f, (float)TimeLeft / TimeToLive, clamped: true);

            Velocity *= 0.71f;

            Lighting.AddLight(Center, new Vector3(1f, 1f, 1f) * Scale.X * 0.02f);

            if (Scale.Y < 0.1f)
                Kill();
        }
    }
}
