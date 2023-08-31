using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
	public class ChandriumCrescentMoon : Particle
    {
        bool rotateClockwise = false;
		byte alpha;
        
		public override void OnSpawn()
        {
            rotateClockwise = Main.rand.NextBool();
        }

        public override void AI()
        {
            Rotation += 0.12f * (rotateClockwise ? 1f : -1f);

            Scale -= 0.016f;
            alpha++;

            if (Scale < 0.05f)
                Kill();

            Lighting.AddLight(Position, new Vector3(0.607f, 0.258f, 0.847f) * Scale);
        }

        private SpriteBatchState state;
		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
            state.SaveState(spriteBatch);
			spriteBatch.Draw(Texture, Position - screenPosition, null, new Color(180, 112, 226).NewAlpha(0.45f), Rotation, Texture.Size() / 2f, ScaleV, SpriteEffects.None, 0f);
		}
	}
}
