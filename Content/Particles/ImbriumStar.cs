using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class ImbriumStar : Particle
    {
        public float Alpha = 1f;

        Color color = Color.White;
        
		public override void OnSpawn()
        {
            color = Color.Lerp(Color.White, new Color(0, 217, 102, 255), 0.1f + 0.7f * Main.rand.NextFloat());
		}

        public override void AI()
        { 
			Scale -= 0.003f;

            if (Scale < 0.0001f)
                Kill();
        }


		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
            var state = spriteBatch.SaveState();

            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state);
			spriteBatch.Draw(Texture, Position - screenPosition, null, color.NewAlpha(Alpha), Rotation, Texture.Size() / 2f, ScaleV, SpriteEffects.None, 0f);
			spriteBatch.End();
			spriteBatch.Begin(state);
		}
	}
}
