using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.LoadingScreens.WorldGen
{
    public class UIWorldGenProgressBar : UIGenProgressBar
    {
        private readonly Asset<Texture2D> texUpper;
        private readonly Asset<Texture2D> texLower;
        private readonly Asset<Texture2D> fillLarge;
        private readonly Asset<Texture2D> fillSmall;

        private readonly Color fillLargeStart;
        private readonly Color fillLargeEnd;
        private readonly Color fillSmallStart;
        private readonly Color fillSmallEnd;

        private float overallProgress;
        private float currentProgress;

        private Vector2 position;

        public void SetPosition(float x, float y) => position = new Vector2(x, y);

        public UIWorldGenProgressBar(Asset<Texture2D> texUpper, Asset<Texture2D> texLower)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                this.texUpper = texUpper;
                this.texLower = texLower;
                fillLarge = ModContent.Request<Texture2D>("Macrocosm/Content/LoadingScreens/WorldGen/DefaultLargeFill", AssetRequestMode.ImmediateLoad);
                fillSmall = ModContent.Request<Texture2D>("Macrocosm/Content/LoadingScreens/WorldGen/DefaultSmallFill", AssetRequestMode.ImmediateLoad);
            }
            base.Recalculate();
        }

        public UIWorldGenProgressBar(Asset<Texture2D> texUpper, Asset<Texture2D> texLower, Color fillLargeStart, Color fillLargeEnd, Color fillSmallStart, Color fillSmallEnd) : this(texUpper, texLower)
        {
            this.fillLargeStart = fillLargeStart;
            this.fillLargeEnd = fillLargeEnd;
            this.fillSmallStart = fillSmallStart;
            this.fillSmallEnd = fillSmallEnd;
            base.Recalculate();
        }

        public UIWorldGenProgressBar(Asset<Texture2D> texUpper, Asset<Texture2D> texLower, Asset<Texture2D> fillLarge, Asset<Texture2D> fillSmall) : this(texUpper, texLower)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                this.fillLarge = fillLarge;
                this.fillSmall = fillSmall;
            }
            base.Recalculate();
        }

        public UIWorldGenProgressBar(Asset<Texture2D> texUpper, Asset<Texture2D> texLower, Asset<Texture2D> fillLarge, Asset<Texture2D> fillSmall, Color fillLargeStart, Color fillLargeEnd, Color fillSmallStart, Color fillSmallEnd) : this(texUpper, texLower, fillLarge, fillSmall)
        {
            this.fillLargeStart = fillLargeStart;
            this.fillLargeEnd = fillLargeEnd;
            this.fillSmallStart = fillSmallStart;
            this.fillSmallEnd = fillSmallEnd;
            base.Recalculate();
        }

        public new void SetProgress(float overallProgress, float currentProgress)
        {
            this.overallProgress = overallProgress;
            this.currentProgress = currentProgress;
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            if ((texUpper ?? texLower ?? fillLarge ?? fillSmall) != null)
            {
                DrawFilling(spriteBatch, new Rectangle((int)position.X + 20, (int)position.Y + 38, 568, 22), overallProgress, fillLarge, fillLargeStart, fillLargeEnd, new Color(48, 48, 48));

                DrawFilling(spriteBatch, new Rectangle((int)position.X + 48, (int)position.Y + 60, 506, 12), currentProgress, fillSmall, fillSmallStart, fillSmallEnd, new Color(48, 48, 48));

                Rectangle r = GetDimensions().ToRectangle();
                r.X -= 8;
                spriteBatch.Draw(texUpper.Value, position + r.TopLeft(), Color.White);
                spriteBatch.Draw(texLower.Value, position + r.TopLeft() + new Vector2(44f, 60f), Color.White);
            }
        }

        private static void DrawFilling(SpriteBatch spriteBatch, Rectangle rect, float progress, Asset<Texture2D> texture, Color barStart, Color barEnd, Color background)
        {

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, background);

            int steps = (int)((rect.Right - rect.Left) * progress);
            for (int i = 0; i < steps; i++)
            {
                spriteBatch.Draw(texture.Value, new Rectangle(rect.Left + i, rect.Y, 1, rect.Height), Color.Lerp(barStart, barEnd, (float)i / steps));
            }
            spriteBatch.Draw(texture.Value, new Rectangle(rect.X + (int)(progress * rect.Width), rect.Y, 2, rect.Height), null, Color.Lerp(barStart, barEnd, 0.5f));
        }
    }
}
