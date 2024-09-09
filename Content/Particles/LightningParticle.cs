using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class LightningParticle : Particle
    {
        public override string TexturePath => Macrocosm.TexturesPath + "Lightning";

        public Color DrawColor;
        public float ScaleVelocity; 
        public int TimeToLive;

        public override int FrameSpeed => 2;
        public override int FrameCount => 6;
        public override int SpawnTimeLeft => 18;

        public override void AI()
        {
            Velocity *= 0.9f;
            Rotation = Velocity.ToRotation();
            Scale += ScaleVelocity;
        }

        public override void UpdateFrame()
        {
            frameCnt++;
            if (frameCnt >= FrameSpeed)
            {
                frameCnt = 0;
                currentFrame = Main.rand.Next(FrameCount);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture.Value, Position - screenPosition, GetFrame(), DrawColor * FadeFactor, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
        }
    }
}
