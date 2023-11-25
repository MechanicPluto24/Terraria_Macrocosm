using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace Macrocosm.Common.DataStructures
{
    /// <summary> By GroxTheGreat @ BaseMod, modified by Feldy </summary>
    public class DrawAnimationPrecise : DrawAnimation
    {
        readonly int Width = 0, Height = 0, offsetX = 0, offsetY = 2;
        readonly bool vertical = true;

        public DrawAnimationPrecise(int ticksperframe, int frameCount, int width, int height, bool vertical = true, int offsetX = 0, int offsetY = 2)
        {
            Width = width;
            Height = height;

            Frame = 0;
            FrameCounter = 0;
            FrameCount = frameCount;
            TicksPerFrame = ticksperframe;

            this.vertical = vertical;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public override void Update()
        {
            int num = FrameCounter + 1;
            FrameCounter = num;
            if (num >= TicksPerFrame)
            {
                FrameCounter = 0;
                num = Frame + 1;
                Frame = num;
                if (num >= FrameCount)
                    Frame = 0;
            }
        }

        public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1)
        {
            return new Rectangle(vertical ? 0 : (Width + offsetX) * this.Frame, vertical ? (Height + offsetY) * Frame : 0, Width, Height);
        }
    }
}
