using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles
{
    public class PortalSpark : Particle
    {
        public override int SpawnTimeLeft => 250;
        public override string TexturePath => Macrocosm.EmptyTexPath;
        public override ParticleDrawLayer DrawLayer => ParticleDrawLayer.AfterProjectiles;

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "CurvedGlow").Value;
            spriteBatch.Draw(glow, Center - screenPosition, null, new Color(89, 151, 193), Rotation, glow.Size() / 2, 0.0375f * ScaleV, SpriteEffects.None, 0f);
            return false;
        }

        bool spawned = false;
        float origScale = 0f;
        public override void AI()
        {
            if (!spawned)
            {
                origScale = Scale;
                spawned = true;
            }
            float speed = Velocity.LengthSquared() * 0.4f;
            Rotation = Velocity.ToRotation();
            ScaleV = new Vector2(Math.Clamp(speed, 0, 15), Math.Clamp(speed, 0, 1)) * 0.09f * origScale;

            Velocity *= 0.91f;

            Lighting.AddLight(Center, new Vector3(1f, 1f, 1f) * Scale * 0.02f);

            if (ScaleV.Y < 0.1f)
                Kill();
        }
    }
}
