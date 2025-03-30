using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;

namespace Macrocosm.Content.Particles
{
    /// <summary> Create pixel box dusts randomly picked from a texture. Blatantly inefficient, avoid </summary>
    public class TextureDustParticle : Particle
    {
        public override string Texture => Macrocosm.EmptyTexPath;
        public override int MaxPoolCount => 1000;

        public Asset<Texture2D> SourceTexture = Macrocosm.EmptyTex;
        private RawTexture textureData;

        public override void SetDefaults()
        {
            TimeToLive = 60;
            Scale = new(1);
        }

        public override void OnSpawn()
        {
            textureData = RawTexture.FromAsset(SourceTexture);
            List<Color> opaque = textureData.Data.Where((c) => c.A == 255).ToList();
            if (opaque.Count > 0)
            {
                Color = opaque.GetRandom(Main.rand);
                if (Color == Color.White)
                {
                    Main.NewText(SourceTexture.Name);
                }
            }

            Velocity.Y += 1f;
        }

        public override void AI()
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            Rectangle sourceRect = new(0, 0, 1, 1);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, Position - screenPosition, sourceRect, Utility.Colorize(Color, lightColor) * FadeFactor, Rotation, Vector2.Zero, Scale * 2, SpriteEffects.None, 0f);
        }
    }
}
