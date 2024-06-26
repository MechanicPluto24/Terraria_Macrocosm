﻿using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.Particles
{
    // Adapted from ParticleOrchestraType.ItemTransfer, 
    // but this one actually applies light color!
    public class ItemTransferParticle : Particle
    {
        public override string TexturePath => Macrocosm.EmptyTexPath;
        public override bool ShouldUpdatePosition => false; 

        public override int SpawnTimeLeft => spawnTimeLeft ??= Main.rand.Next(60, 80);
        private int? spawnTimeLeft;

        public Vector2 StartPosition;
        public Vector2 EndPosition;

        public Vector2 MovementBezier;
        public Vector2 ScaleBezier;

        public float Opacity;

        public int ItemType;
        private Item item = new();

        public override void OnSpawn()
        {
            if(ContentSamples.ItemsByType.TryGetValue(ItemType, out Item item) && !item.IsAir)
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
            float progress = (float)TimeLeft / SpawnTimeLeft;

            float movementProgress = Utils.Remap(progress, 0.1f, 0.5f, 0f, 0.85f);
            movementProgress = Utils.Remap(progress, 0.5f, 0.9f, movementProgress, 1f);

            Vector2.Hermite(ref EndPosition, ref MovementBezier, ref StartPosition, ref ScaleBezier, movementProgress, out Position);

            float scaleProgress = Utils.Remap(progress, 0f, 0.1f, 0f, 1f);
            scaleProgress = Utils.Remap(progress, 0.85f, 0.95f, scaleProgress, 0f);
            Scale = item.scale * scaleProgress;

            Opacity = Utils.Remap(progress, 0f, 0.25f, 0f, 1f) * Utils.Remap(progress, 0.85f, 0.95f, 1f, 0f);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            ItemSlot.DrawItemIcon(item, ItemSlot.Context.InWorld, spriteBatch, Position - screenPosition, Scale, 100f, Utility.Colorize(Color.White, lightColor) * Opacity);
        }
    }
}
