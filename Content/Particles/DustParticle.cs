using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles
{
    /// <summary> Dust wrapped by a particle, allows access to Particle fields, for custom behavior </summary>
    public class DustParticle : Particle
    {
        public override string Texture => Macrocosm.EmptyTexPath;
        public override int MaxPoolCount => 1000;

        /// <summary> The type of Dust to use for this particle. </summary>
        public int DustType { get; set; } = -1;

        /// <summary> Whether the dust should update normally </summary>
        public bool NormalUpdate { get; set; } = true;

        public float FadeIn { get; set; } = 0f;
        public bool NoGravity { get; set; } = false;
        public bool NoLight { get; set; } = false;
        public bool NoLightEmittence { get; set; } = false;
        public object CustomData { get; set; } = null;


        private Dust dust;

        public override void SetDefaults()
        {
            TimeToLive = 3600;
        }

        public override void OnSpawn()
        {
            if (DustType < 0 || (DustType >= DustID.Count && DustLoader.GetDust(DustType) is null))
            {
                Kill();
                return;
            }

            dust = Dust.NewDustPerfect(Position, DustType, Velocity, Color.A, Color, Scale.X);
            dust.rotation = Rotation;
            dust.fadeIn = FadeIn;
            dust.noGravity = NoGravity;
            dust.noLight = NoLight;
            dust.noLightEmittence = NoLightEmittence;
            dust.customData = CustomData;
        }

        public override void AI()
        {
            if (NormalUpdate)
            {
                dust.velocity += Acceleration;
                dust.rotation += RotationVelocity;
                dust.scale += ScaleVelocity.X;
            }
            else
            {
                dust.position = Position;
                dust.type = DustType;
                dust.velocity = Velocity;
                dust.alpha = Color.A;
                dust.color = Color;
                dust.scale = Scale.X;
                dust.rotation = Rotation;
                dust.fadeIn = FadeIn;
                dust.noGravity = NoGravity;
                dust.noLight = NoLight;
                dust.noLightEmittence = NoLightEmittence;
                dust.customData = CustomData;
            }

            if (!dust.active)
                Kill();
        }

        public override void OnKill()
        {
            dust.active = false;
        }

        public override Rectangle? GetFrame() => dust.frame;

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            if (DustLoader.GetDust(dust.type) is not ModDust modDust || modDust.PreDraw(dust))
            {
                Texture2D texture = dust.GetTexture();
                spriteBatch.Draw(texture, dust.position - screenPosition, GetFrame(), lightColor * FadeFactor, dust.GetVisualRotation(), new Vector2(4f, 4f), dust.GetVisualScale(), SpriteEffects.None, 0f);

                if (dust.color != default)
                    spriteBatch.Draw(texture, dust.position - Main.screenPosition, dust.frame, dust.color * FadeFactor, dust.rotation, new Vector2(4f, 4f), dust.GetVisualScale(), SpriteEffects.None, 0f);
            }
        }
    }
}
