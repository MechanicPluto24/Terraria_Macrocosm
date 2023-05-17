using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class ChandriumCrescentMoon : Particle
    {
        bool fadeIn;
		byte alpha;
        
		public override void OnSpawn()
        {
            fadeIn = true;
        }

        public override void AI()
        {
            Rotation += 0.16f * (WhoAmI % 2 == 0 ? 1f : -1f);
 
            if (alpha <= 24)
                fadeIn = false;

            if (fadeIn)
            {
                alpha -= 15;
                Scale += 0.01f;
            }
            else
            {
                alpha++;
				Scale -= 0.01f;
            }

			Scale -= 0.01f;
            alpha++;

            if (Scale < 0.3f)
                Kill();

            Lighting.AddLight(Position, new Vector3(0.607f, 0.258f, 0.847f) * Scale);
        }


		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
			spriteBatch.Draw(Texture, Position - screenPosition, null, lightColor.NewAlpha(0.5f), Rotation, Texture.Size() / 2f, Scale, SpriteEffects.None, 0f);
		}
	}

    
}
