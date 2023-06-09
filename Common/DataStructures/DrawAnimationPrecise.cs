using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace Macrocosm.Common.DataStructures
{
	public class DrawAnimationPrecise : DrawAnimation
	{
		int Width = 0, Height = 0, offsetX = 0, offsetY = 2;
		bool vertical = true;
		public DrawAnimationPrecise(int ticksperframe, int frameCount, int w, int h, bool v = true, int offX = 0, int offY = 2)
		{
			Width = w;
			Height = h;
			vertical = v;
			offsetX = offX;
			offsetY = offY;
			this.Frame = 0;
			this.FrameCounter = 0;
			this.FrameCount = frameCount;
			this.TicksPerFrame = ticksperframe;
		}

		public override void Update()
		{
			int num = this.FrameCounter + 1;
			this.FrameCounter = num;
			if (num >= this.TicksPerFrame)
			{
				this.FrameCounter = 0;
				num = this.Frame + 1;
				this.Frame = num;
				if (num >= this.FrameCount)
				{
					this.Frame = 0;
				}
			}
		}

		public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1)
		{
			return new Rectangle(vertical ? 0 : (this.Width + offsetX) * this.Frame, vertical ? (this.Height + offsetY) * this.Frame : 0, this.Width, this.Height);
		}
	}
}
