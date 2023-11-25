namespace Macrocosm.Content.Rockets.Modules
{
    public abstract class AnimatedRocketModule : RocketModule
    {
        /// <summary> The current animation frame </summary>
        public int CurrentFrame { get; set; } = 9;

        /// <summary> The total number of frames </summary>
        public virtual int NumberOfFrames { get; set; } = 10;

        public int FrameSpeed { get; set; } = 4;

        public bool ShouldAnimate { get; set; } = true;

        private int frameCounter;
        private bool isAnimating = false;
        private bool isAnimatingForward = true;

        protected AnimatedRocketModule(Rocket rocket) : base(rocket)
        {
        }

        public void UpdateAnimation()
        {
            if (!isAnimating)
                return;

            if (frameCounter >= FrameSpeed)
            {
                frameCounter = 0;

                if (isAnimatingForward)
                {
                    if (CurrentFrame == NumberOfFrames - 1)
                    {
                        isAnimating = false;
                    }
                    else
                    {
                        CurrentFrame++;
                    }
                }
                else
                {
                    if (CurrentFrame == 0)
                    {
                        isAnimating = false;
                    }
                    else
                    {
                        CurrentFrame--;
                    }
                }
            }
            else
            {
                frameCounter++;
            }
        }

        public void StartAnimation()
        {
            if (!ShouldAnimate)
                return;

            isAnimatingForward = true;
            isAnimating = true;
        }

        public void StartReverseAnimation()
        {
            if (!ShouldAnimate)
                return;

            isAnimatingForward = false;
            isAnimating = true;
        }

        public void StopAnimation()
        {
            frameCounter = 0;
            isAnimating = false;
        }
    }
}
