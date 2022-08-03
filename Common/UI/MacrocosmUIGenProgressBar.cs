using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace Macrocosm.Common.UI {
    public class MacrocosmUIGenProgressBar : UIGenProgressBar {
        private readonly Texture2D texUpper;
        private readonly Texture2D texLower;

        private float visualOverallProgress;
        private float targetOverallProgress;
        private float visualCurrentProgress;
        private float targetCurrentProgress;

        const int longBarWidth = 562;
        const int longBarHeight = 18;
        const int shortBarWidth = 508;
        const int shortBarHeight = 10;

        private Vector2 position;

        public void SetPosition(float x, float y) => position = new Vector2(x, y);

        public MacrocosmUIGenProgressBar(Texture2D texUpper, Texture2D texLower) {
            if (Main.netMode != NetmodeID.Server) {
                this.texUpper = texUpper;
                this.texLower = texLower;
            }
            base.Recalculate();
        }

        public MacrocosmUIGenProgressBar(Texture2D texUpper, Texture2D texLower, float totalWidth, float totalHeight) : this(texUpper, texLower) {
            Width.Pixels = totalWidth;
            Height.Pixels = totalHeight;
            base.Recalculate();
        }

        public new void SetProgress(float overallProgress, float currentProgress) {
            targetCurrentProgress = currentProgress;
            targetOverallProgress = overallProgress;
        }

        public new void DrawSelf(SpriteBatch spriteBatch) {
            if (texUpper != null && texLower != null) {
                visualOverallProgress = targetOverallProgress;
                visualCurrentProgress = targetCurrentProgress;

                CalculatedStyle dimensions = GetDimensions();
                int completedWidth = (int)(visualOverallProgress * (float)longBarWidth);
                int completedWidth2 = (int)(visualCurrentProgress * (float)shortBarWidth);

                Vector2 value = new(dimensions.X, dimensions.Y);
                Color color = default;

                color.PackedValue = 4286836223u;
                DrawFilling(spriteBatch, position + value + new Vector2(20f, 40f), longBarHeight, completedWidth, longBarWidth, color, Color.Lerp(color, Color.Black, 0.5f), new Color(48, 48, 48));
                color.PackedValue = 4290947159u;
                DrawFilling(spriteBatch, position + value + new Vector2(48f, 60f), shortBarHeight, completedWidth2, shortBarWidth, color, Color.Lerp(color, Color.Black, 0.5f), new Color(33, 33, 33));

                Rectangle r = GetDimensions().ToRectangle();
                r.X -= 8;
                spriteBatch.Draw(texUpper, position + r.TopLeft(), Color.White);
                spriteBatch.Draw(texLower, position + r.TopLeft() + new Vector2(44f, 60f), Color.White);
            }
        }

        private static void DrawFilling(SpriteBatch spritebatch, Vector2 position, int height, int completedWidth, int totalWidth, Color filled, Color separator, Color empty) {
            if (completedWidth % 2 != 0)
                completedWidth--;

            spritebatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X, (int)position.Y, completedWidth, height), new Rectangle(0, 0, 1, 1), filled);
            spritebatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X + completedWidth, (int)position.Y, totalWidth - completedWidth, height), new Rectangle(0, 0, 1, 1), empty);
            spritebatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X + completedWidth - 2, (int)position.Y, 2, height), new Rectangle(0, 0, 1, 1), separator);
        }
    }
}
