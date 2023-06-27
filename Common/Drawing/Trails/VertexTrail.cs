using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;

namespace Macrocosm.Common.Drawing.Trails
{
	public abstract class VertexTrail
	{
		public abstract MiscShaderData TrailShader { get; }
		public virtual object Owner { get; set; }

		public virtual float Opacity { get; set; } = 1f;
		public virtual float Saturation { get; set; } = -1f;

		public virtual Color? TrailColor { get; set; }
		public virtual float? TrailWidth { get; set; }

		public virtual Color TrailColors(float progressOnStrip) => TrailColor ?? Color.White;
		public virtual float TrailWidths(float progressOnStrip) => TrailWidth ?? 1f;

		public virtual void Update() { }
		private void InternalUpdate()
		{
			Update();

			TrailShader.UseOpacity(Opacity);
			TrailShader.UseSaturation(Saturation);
		}

		public virtual void Draw()
		{ 
			if(Owner is Projectile projectile)
				Draw(projectile.oldPos, projectile.oldRot, projectile.Size / 2);
			else if (Owner is Particle particle)
				Draw(particle.OldPositions, particle.OldRotations, particle.Size / 2);
		}

		public virtual void Draw(Vector2[] positions, float[] rotations, Vector2 offset)
		{
			VertexStrip vertexStrip = new();

			InternalUpdate();

			TrailShader.Apply();

			vertexStrip.PrepareStripWithProceduralPadding(positions, rotations, TrailColors, TrailWidths, offset - Main.screenPosition, false, true);
			vertexStrip.DrawTrail();
		}
	}
}
