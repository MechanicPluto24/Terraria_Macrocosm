using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace Macrocosm.Content.Particles
{
    // Adapted from ParticleOrchestraType.ItemTransfer, 
    // but this one actually applies light color!
    public class ItemTransferParticle : Particle
    {
        public override string TexturePath => Macrocosm.EmptyTexPath;
        public override bool ShouldUpdatePosition => false;

        public Vector2 StartPosition;
        public Vector2 EndPosition;

        public Vector2 MovementBezier;
        public Vector2 ScaleBezier;

        public float Opacity;

        public int ItemType;
        private Item item = new();

        public override void SetDefaults()
        {
            TimeToLive = 60;

            StartPosition = default;
            EndPosition = default;

            MovementBezier = default;
            ScaleBezier = default;

            Opacity = 0f;
            ItemType = 0;
            item = new();
        }

        public override void OnSpawn()
        {
            if (ContentSamples.ItemsByType.TryGetValue(ItemType, out Item item) && !item.IsAir)
            {
                this.item = item;

                Vector2 movement = new(0f, -1f);
                float distance = Vector2.Distance(StartPosition, EndPosition);
                MovementBezier = movement * distance + Main.rand.NextVector2Circular(32f, 32f);
                ScaleBezier = -movement * distance + Main.rand.NextVector2Circular(32f, 32f);
            }
            else
            {
                Kill();
            }
        }

        public override void AI()
        {
            float progress = (float)TimeLeft / TimeToLive;

            float movementProgress = Utils.Remap(progress, 0.1f, 0.5f, 0f, 0.85f);
            movementProgress = Utils.Remap(progress, 0.5f, 0.9f, movementProgress, 1f);

            Vector2.Hermite(ref EndPosition, ref MovementBezier, ref StartPosition, ref ScaleBezier, movementProgress, out Position);

            float scaleProgress = Utils.Remap(progress, 0f, 0.1f, 0f, 1f);
            scaleProgress = Utils.Remap(progress, 0.85f, 0.95f, scaleProgress, 0f);
            Scale.X = item.scale * scaleProgress;

            Opacity = Utils.Remap(progress, 0f, 0.25f, 0f, 1f) * Utils.Remap(progress, 0.85f, 0.95f, 1f, 0f);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            ItemSlot.DrawItemIcon(item, ItemSlot.Context.InWorld, spriteBatch, Position - screenPosition, Scale.X, 100f, Utility.Colorize(Color.White, lightColor) * Opacity);
        }
    }
}
