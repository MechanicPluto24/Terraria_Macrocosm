using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.Graphics;
using Terraria.ModLoader;
using Macrocosm.Content.Players;

namespace Macrocosm.Content.Particles
{
	public class CelestialBulwarkDashParticle : Particle
	{
		public override int SpawnTimeLeft => 1000;
		public override string TexturePath => Macrocosm.EmptyTexPath;
		public override ParticleDrawLayer DrawLayer => ParticleDrawLayer.BeforeNPCs;
        public override int TrailCacheLenght => 24;

        public int PlayerID;
		public Color Color;
        public float Opacity;

        private float defScale;
        private float defRotation;
        private bool spawned;
        private bool collided;

        public Player Player => Main.player[PlayerID];
        public DashPlayer DashPlayer => Player.GetModPlayer<DashPlayer>();
        public float Progress => (float)DashPlayer.DashTimer / DashPlayer.AccDashDuration;

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
            Texture2D slash = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Slash1").Value;
            Texture2D glow = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Circle5").Value;
            //Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Star2").Value;

            for (int i = 0; i < TrailCacheLenght; i++)
            {
                float trailProgress = MathHelper.Clamp((float)i / TrailCacheLenght, 0f, 1f);
                float scale = defScale - (Scale * trailProgress * 5f);
                Color color = scale < 0 ? Color * Progress * (1f - trailProgress) : Color * Progress;
                Vector2 position = scale < 0 ? OldPositions[i] + new Vector2(0, 55).RotatedBy(OldRotations[i]) : Vector2.Lerp(OldPositions[i], Center, Progress * (1f - trailProgress));
                spriteBatch.Draw(slash, position - screenPosition, null, color, OldRotations[i], slash.Size() / 2, scale, SpriteEffects.None, 0f);
            }

			spriteBatch.Draw(slash, Center - screenPosition, null, Color * Progress, Rotation, slash.Size() / 2, Scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(glow, Center - screenPosition, null, Color.Lerp(Color.White, Color, 0.75f).WithOpacity(0.5f) * Progress, defRotation, glow.Size() / 2, Utility.QuadraticEaseIn(Progress) * 0.7f, SpriteEffects.None, 0f);
			//spriteBatch.Draw(flare, Center + new Vector2(0, 30).RotatedBy(defRotation) - screenPosition, null, Color.Lerp(Color.White, Color, 0.5f).WithOpacity(0.5f) * Progress, 0f, flare.Size() / 2, Utility.CubicEaseIn(Progress) * 0.8f, SpriteEffects.None, 0f);
			//spriteBatch.Draw(flare, Center + new Vector2(0, 30).RotatedBy(defRotation) - screenPosition, null, Color.Lerp(Color.White, Color, 0.5f).WithOpacity(0.5f) * Progress, MathHelper.PiOver2, flare.Size() / 2, Utility.QuadraticEaseIn(Progress) * 1.6f, SpriteEffects.None, 0f);
            return false;
		}

        public override void AI()
		{
			if (!spawned)
			{
				spawned = true;
                defScale = Scale;
                defRotation = Rotation;
			}

            if (DashPlayer.DashTimer <= 0)
                Kill();

            Scale = MathHelper.Lerp(Scale * 0.8f, defScale, Progress);

            if (collided)
                 Color *= 0.9f;
            else
                 collided = DashPlayer.CollidedWithNPC;
 
            if (Player.velocity.Length() > 0.5f)
            {
                if(!collided)
                    Rotation = Player.velocity.ToRotation() - MathHelper.PiOver2;

                Position = Player.Center + new Vector2(0, 15).RotatedBy(Rotation);
            }
		}
	}
}
