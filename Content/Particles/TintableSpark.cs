using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Macrocosm.Content.Particles
{
    public class TintableSpark : Particle
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        private float origScale;

        public override void SetDefaults()
        {
            TimeToLive = 250;
            Color = Color.White;
        }

        public override void OnSpawn()
        {
            origScale = Scale.X;
        }

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            Texture2D texture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            spriteBatch.Draw(texture, Center - screenPosition, null, Color.WithOpacity(0.8f), Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            float speed = Velocity.LengthSquared() * 0.9f;
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
            Scale = new Vector2(Math.Clamp(speed, 0, 2), Math.Clamp(speed, 0, 5)) * 0.04f * origScale;

            Velocity *= 0.91f;

            Lighting.AddLight(Center, new Vector3(1f, 1f, 1f) * Scale.X * 0.02f);

            if (Scale.Y < 0.1f)
                Kill();
        }
    }
}
