using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Common.Drawing.Trails
{
    public abstract class VertexTrail : IModType
    {
        public Mod Mod { get; }
        public string Name { get; }
        public string FullName { get; }

        public abstract MiscShaderData TrailShader { get; }
        public virtual object Owner { get; set; }

        public virtual float Opacity { get; set; } = 1f;
        public virtual float Saturation { get; set; } = -1f;

        public virtual Color? TrailColor { get; set; }
        public virtual float? TrailWidth { get; set; }

        public virtual Color TrailColors(float progressOnStrip) => TrailColor ?? Color.White;
        public virtual float TrailWidths(float progressOnStrip) => TrailWidth ?? 1f;

        public virtual int StartIndex => 1;

        public virtual void Update() { }
        private void InternalUpdate()
        {
            Update();

            TrailShader.UseOpacity(Opacity);
            TrailShader.UseSaturation(Saturation);
        }

        public virtual void Draw(Vector2 offset = default)
        {
            if (Owner is Projectile projectile)
                Draw(projectile.oldPos, projectile.oldRot, offset);
            else if (Owner is Particle particle)
                Draw(particle.OldPositions, particle.OldRotations, particle.Size / 2 + offset);
        }

        public virtual void Draw(Vector2[] positions, float[] rotations, Vector2 offset)
        {
            VertexStrip vertexStrip = new();

            InternalUpdate();

            TrailShader.Apply();

            vertexStrip.PrepareStripWithProceduralPadding(positions[StartIndex..], rotations[StartIndex..], TrailColors, TrailWidths, offset - Main.screenPosition, false, true);
            vertexStrip.DrawTrail();
        }
    }
}
