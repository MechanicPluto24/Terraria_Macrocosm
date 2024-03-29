using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.Particles
{
    public class ItemTransferParticle : Particle
    {
        public override int SpawnTimeLeft => spawnTimeLeft;

        public Vector2 StartPosition;
        public Vector2 EndPosition;

        public Vector2 BezierHelper1;
        public Vector2 BezierHelper2;

        public int ItemType;

        private Item item = new();
        private int spawnTimeLeft;

        public override void OnSpawn()
        {
            if(ContentSamples.ItemsByType.TryGetValue(ItemType, out Item item) && !item.IsAir)
            {
                this.item = new(ItemType);

                spawnTimeLeft = Main.rand.Next(60, 80);

                Vector2 movement = new(0f, -1f);
                float distance = Vector2.Distance(EndPosition, StartPosition);
                BezierHelper1 = movement * distance + Main.rand.NextVector2Circular(32f, 32f);
                BezierHelper2 = -movement * distance + Main.rand.NextVector2Circular(32f, 32f);
            }
            else
            {
                Kill();
            }        
        }

        public override void AI()
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            float progress = TimeLeft / SpawnTimeLeft;

            float bezierProgress1 = Utils.Remap(progress, 0.1f, 0.5f, 0f, 0.85f);
            bezierProgress1 = Utils.Remap(progress, 0.5f, 0.9f, bezierProgress1, 1f);

            Vector2.Hermite(ref StartPosition, ref BezierHelper1, ref EndPosition, ref BezierHelper2, bezierProgress1, out Vector2 position);

            float bezierProgress2 = Utils.Remap(progress, 0f, 0.1f, 0f, 1f);
            bezierProgress2 = Utils.Remap(progress, 0.85f, 0.95f, bezierProgress2, 0f);

            float opacity = Utils.Remap(progress, 0f, 0.25f, 0f, 1f) * Utils.Remap(progress, 0.85f, 0.95f, 1f, 0f);
            ItemSlot.DrawItemIcon(item, ItemSlot.Context.InWorld, spriteBatch, position - screenPosition, item.scale * bezierProgress2, 100f, Utility.Colorize(Color.White, lightColor) * opacity);
        }
    }
}
