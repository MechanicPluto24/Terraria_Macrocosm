
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket
	{
		public void DrawDebugHitbox()
		{
			Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, Hitbox, Color.Red.NewAlpha(0.5f));
		}
	}
}
