using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class TintableExplosion : Particle
    {
        public override int FrameCount => 7;
        public override bool DespawnOnAnimationComplete => true;

        public int NumberOfInnerReplicas { get; set; }
        public float ReplicaScalingFactor { get; set; }

        public override void SetDefaults()
        {
            FrameSpeed = 4;
            RotationVelocity = Main.rand.NextBool() ? 0.005f : -0.005f;

            NumberOfInnerReplicas = 0;
            ReplicaScalingFactor = 0f;
        }

        public override void OnSpawn()
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(TextureAsset.Value, Position - screenPosition, GetFrame(), Color, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
        }

        public override void PostDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            for (int i = 1; i < NumberOfInnerReplicas; i++)
            {
                float explosionProgress = (float)currentFrame / FrameCount;
                float replicaDecrease = 1f - (float)i / NumberOfInnerReplicas;
                float scale = Scale.X * MathHelper.Lerp(ReplicaScalingFactor + (1f - ReplicaScalingFactor) * replicaDecrease, 1.06f, explosionProgress);

                Color color = Color.WithOpacity(((float)TimeLeft / TimeToLive) * 0.7f);
                spriteBatch.Draw(TextureAsset.Value, Position - screenPosition, GetFrame(), color, Rotation, Size * 0.5f, scale, SpriteEffects.None, 0f);
            }
        }

        public override void AI()
        {
            Lighting.AddLight(Center, Color.ToVector3());
        }

        public override void OnKill()
        {
        }
    }
}
