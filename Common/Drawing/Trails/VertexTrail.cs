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

        public virtual void Draw(Projectile projectile, Vector2 offset = default) => Draw(projectile.oldPos, projectile.oldRot, offset);
        public virtual void Draw(NPC npc, Vector2 offset = default) => Draw(npc.oldPos, npc.oldRot, offset);
        public virtual void Draw(Particle particle, Vector2 offset = default) => Draw(particle.OldPositions, particle.OldRotations, offset);

        public virtual void Draw(Vector2[] positions, float[] rotations, Vector2 offset = default)
        {
            VertexStrip vertexStrip = new();

            InternalUpdate();

            TrailShader.Apply();

            vertexStrip.PrepareStripWithProceduralPadding(positions[StartIndex..], rotations[StartIndex..], TrailColors, TrailWidths, offset - Main.screenPosition, false, true);
            vertexStrip.DrawTrail();
        }
    }
}
