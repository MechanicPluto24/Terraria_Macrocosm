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
            Bounds.DrawDebugBounds(Main.spriteBatch, Color.Red);
        }

        public void DrawDebugModuleHitbox()
        {
            foreach (RocketModule module in Modules)
                module.Bounds.DrawDebugBounds(Main.spriteBatch, Color.Blue);
        }

        public void DisplayWhoAmI()
        {
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, WhoAmI.ToString(), new Vector2(Center.X, Position.Y - 50) - Main.screenPosition, Color.White, 0f, Vector2.Zero, Vector2.One);
        }

        public override string ToString()
        {
            return "Active: " + Active + ", WhoAmI: " + WhoAmI + ", CurrentSubworld: " + CurrentWorld;
        }
    }
}
