using Macrocosm.Content.Liquids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    /// <summary> Panel wrapper for <see cref="UILiquid"/> with automatic hiding of overflow and (TODO) gradations </summary>
    public class UILiquidTankPiston : UILiquidTank
    {
        private static Asset<Texture2D> piston;

        private int animationTimer;
        private int animationSpeed;
        private int pistonFrame = 0;
        private const int pistonFrameMax = 9;

        public UILiquidTankPiston(LiquidType liquidType) : base(liquidType) { }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width = new(62, 0);
            Height = new(60, 0);
            Bubbles = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (pistonFrame > 0)
            {
                if (animationTimer++ >= animationSpeed)
                {
                    if (pistonFrame++ >= pistonFrameMax - 1)
                        pistonFrame = 0;

                    animationTimer = 0;
                }

                LiquidLevel = pistonFrame switch
                {
                    0 => 0.85f,
                    1 or 6 => 0.2f,
                    2 or 3 or 4 or 5 => 0.1f,
                    7 => 0.45f,
                    8 => 0.55f,
                    _ => 0f
                };
            }
            else
            {
                animationTimer = 0;
            }
        }

        public void StartAnimation(int speed)
        {
            if (pistonFrame <= 0)
            {
                pistonFrame = 1;
                animationSpeed = speed;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            piston ??= ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Piston");

            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position();

            spriteBatch.Draw(piston.Value, position + new Vector2(2, 2), piston.Frame(pistonFrameMax, frameX: pistonFrame), Color.White);
        }
    }
}
