using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class Smoke : Particle
    {
        Rectangle rectFrame;

        public override int FrameNumber => 3;

		public override void OnSpawn()
        {
            int frameSizeY = Texture.Height / FrameNumber;
            int frame = Main.rand.Next(0, FrameNumber);
			rectFrame = new Rectangle(0, frame * frameSizeY, Texture.Width, frameSizeY);
        }

        public override void AI()
        {
			Scale -= 0.003f;

            if (Scale < 0.03f)
                Kill();
        }
	}
}
