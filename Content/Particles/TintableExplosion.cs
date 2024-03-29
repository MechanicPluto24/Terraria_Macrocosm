using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class TintableExplosion : Particle
    {
        public override int TrailCacheLenght => 15;

        public Color DrawColor;
        public int NumberOfInnerReplicas;
        public float ReplicaScalingFactor;

        public override int FrameNumber => 7;
        public override int FrameSpeed => 4;
        public override bool DespawnOnAnimationComplete => true;


        private bool rotateClockwise = false;

        public override void OnSpawn()
        {
            rotateClockwise = Main.rand.NextBool();
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), DrawColor, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
        }

        public override void PostDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            for (int i = 1; i < NumberOfInnerReplicas; i++)
            {
                float explosionProgress = (float)currentFrame / FrameNumber;
                float replicaDecrease = 1f - (float)i / NumberOfInnerReplicas;
                float scale = Scale * MathHelper.Lerp(ReplicaScalingFactor + (1f - ReplicaScalingFactor) * replicaDecrease, 1.06f, explosionProgress);

                Color color = DrawColor.WithOpacity(((float)TimeLeft / SpawnTimeLeft) * 0.7f);
                spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), color, Rotation, Size * 0.5f, scale, SpriteEffects.None, 0f);
            }
        }

        public override void AI()
        {
            Lighting.AddLight(Center, DrawColor.ToVector3());

            if (rotateClockwise)
                Rotation += 0.005f;
            else
                Rotation -= 0.005f;
        }

        public override void OnKill()
        {
        }
    }
}
