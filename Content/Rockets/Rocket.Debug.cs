using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket
    {
        public void DrawDebugBounds()
        {
            Rectangle rect = new((int)(Bounds.X - Main.screenPosition.X), (int)(Bounds.Y - Main.screenPosition.Y), Bounds.Width, Bounds.Height);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.Red * 0.5f);
        }

        public void DrawDebugModuleHitbox()
        {
            foreach (RocketModule module in ModuleTemplates)
            {
                Rectangle rect = new((int)(module.Bounds.X - Main.screenPosition.X), (int)(module.Bounds.Y - Main.screenPosition.Y), module.Bounds.Width, module.Bounds.Height);
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.Green * 0.5f);

                rect = new((int)(module.Bounds.X - Main.screenPosition.X), (int)(module.Bounds.Y - Main.screenPosition.Y), module.Bounds.Width, module.Bounds.Height);
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.Blue * 0.5f);
            }
        }

        public void DisplayWhoAmI()
        {
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, WhoAmI.ToString(), Bounds.Center.ToVector2() - new Vector2(0, 400) - Main.screenPosition, Color.White, 0f, Vector2.Zero, Vector2.One);
        }

        public override string ToString()
        {
            return "Active: " + Active + ", WhoAmI: " + WhoAmI + ", CurrentSubworld: " + CurrentWorld;
        }
    }
}
