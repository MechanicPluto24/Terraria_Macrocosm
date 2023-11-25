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
            foreach (RocketModule module in Modules.Values)
            {
                Rectangle rect = new((int)(module.Hitbox.X - Main.screenPosition.X), (int)(module.Hitbox.Y - Main.screenPosition.Y), module.Hitbox.Width, module.Hitbox.Height);
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.Green * 0.5f);
            }
        }

        public void DisplayWhoAmI()
        {
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, WhoAmI.ToString(), CommandPod.Center - new Vector2(0, 100) - Main.screenPosition, Color.White, 0f, Vector2.Zero, Vector2.One);
        }

        public override string ToString()
        {
            return "Active: " + Active + ", WhoAmI: " + WhoAmI + ", CurrentSubworld: " + CurrentWorld;
        }
    }
}
