using Terraria;
using SubworldLibrary;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.UI.Chat;
using Macrocosm.Common.Utility;
using Macrocosm.Common.Utility.IO;

namespace Macrocosm.Content.Subworlds.Moon
{
    /*
    public class MoonSubworldLoadUI : UIDefaultSubworldLoad
    {
        bool toEarth;
        double animationTimer = 0;
        Texture2D lunaBackground;
        Texture2D earthBackground;
        private string _chosenMessage;
        public override void OnInitialize()
        {
            toEarth = SubworldSystem.IsActive<Moon>();
            lunaBackground = ModContent.GetTexture($"{nameof(Macrocosm)}/Content/Subworlds/LoadingBackgrounds/Luna");
            earthBackground = ModContent.GetTexture($"{nameof(Macrocosm)}/Content/Subworlds/LoadingBackgrounds/Earth");
            var textFileLoader = new TextFileLoader();
            if (toEarth)
                _chosenMessage = ListRandom.Pick(textFileLoader.Parse("Content/Subworlds/Earth/EarthMessages"));
            else
                _chosenMessage = ListRandom.Pick(textFileLoader.Parse("Content/Subworlds/Moon/MoonMessages"));
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // TODO: Delegate raw SpriteBatch calls to a composite library like every proper XNA engine does
            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.UIScaleMatrix);
            if (toEarth)
            {
                spriteBatch.Draw
                (
                    earthBackground,
                    new Rectangle(Main.screenWidth - earthBackground.Width, Main.screenHeight - earthBackground.Height + 50 - (int)(animationTimer * 10), earthBackground.Width, earthBackground.Height),
                    null,
                    Color.White * (float)(animationTimer / 5) * 0.8f
                );
                string msgToPlayer = "Earth"; // Title
                Vector2 messageSize = Main.fontDeathText.MeasureString(msgToPlayer) * 1f;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, msgToPlayer, new Vector2(Main.screenWidth / 2f - messageSize.X / 2f, messageSize.Y), new Color(94, 150, 255), 0f, Vector2.Zero, Vector2.One);
                Vector2 messageSize2 = Main.fontDeathText.MeasureString(_chosenMessage) * 0.7f;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, _chosenMessage, new Vector2(Main.screenWidth / 2f - messageSize2.X / 2f, Main.screenHeight - messageSize2.Y - 20), Color.White, 0f, Vector2.Zero, new Vector2(0.7f));
            }
            else
            {
                spriteBatch.Draw
                (
                    lunaBackground,
                    new Rectangle(Main.screenWidth - lunaBackground.Width, Main.screenHeight - lunaBackground.Height + 50 - (int)(animationTimer * 10), lunaBackground.Width, lunaBackground.Height),
                    null,
                    Color.White * (float)(animationTimer / 5) * 0.8f
                );
                string msgToPlayer = "Earth's Moon"; // Title
                Vector2 messageSize = Main.fontDeathText.MeasureString(msgToPlayer) * 1f;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, msgToPlayer, new Vector2(Main.screenWidth / 2f - messageSize.X / 2f, messageSize.Y), Color.White, 0f, Vector2.Zero, Vector2.One);
                Vector2 messageSize2 = Main.fontDeathText.MeasureString(_chosenMessage) * 0.7f;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, _chosenMessage, new Vector2(Main.screenWidth / 2f - messageSize2.X / 2f, Main.screenHeight - messageSize2.Y - 20), Color.White, 0f, Vector2.Zero, new Vector2(0.7f));
            }
            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);

            base.DrawSelf(spriteBatch);
        }
        public override void Update(GameTime gameTime)
        {
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > 5)
                animationTimer = 5;
        }
        
    }
    */
}
