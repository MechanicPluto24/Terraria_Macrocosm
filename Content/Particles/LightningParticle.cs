using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles
{
    public class LightningParticle : Particle
    {
        private static Asset<Texture2D> outline;

        public override string TexturePath => Macrocosm.TexturesPath + "Lightning";
        public override int FrameCount => 6;

        public Color OutlineColor;

        public override void SetDefaults()
        {
            TimeToLive = 18;
            FrameSpeed = 2;
            OutlineColor = Color.Transparent;
        }

        public override void OnSpawn()
        {
        }

        public override void AI()
        {
            Velocity *= 0.9f;
            Rotation = Velocity.ToRotation();
        }

        public override void UpdateFrame()
        {
            frameCounter++;
            if (frameCounter >= FrameSpeed)
            {
                frameCounter = 0;
                currentFrame = Main.rand.Next(FrameCount);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            outline ??= ModContent.Request<Texture2D>(TexturePath + "_Outline");
            spriteBatch.Draw(outline.Value, Position - screenPosition, GetFrame(), OutlineColor * FadeFactor, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Texture.Value, Position - screenPosition, GetFrame(), Color * FadeFactor, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
        }
    }
}
