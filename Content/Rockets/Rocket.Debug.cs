
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;

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
			foreach(RocketModule module in Modules)
			{
				Rectangle rect = new((int)(module.Hitbox.X - Main.screenPosition.X), (int)(module.Hitbox.Y - Main.screenPosition.Y), module.Hitbox.Width, module.Hitbox.Height);
				Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.Green * 0.5f);
			}
 		}
	}
}
