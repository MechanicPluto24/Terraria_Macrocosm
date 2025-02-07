using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.DataStructures
{
    public struct RotatedRectangle
    {
        public Vector2 Position { get; set; } 
        public Vector2 Origin { get; }      
        public int Width { get; }
        public int Height { get; }
        public float Rotation { get; private set; }

        public Vector2 Center => Position + Origin; 
        public Vector2[] Corners { get; private set; }
        public Vector2 Size => new(Width, Height);

        public RotatedRectangle(Vector2 position, Vector2 origin, int width, int height, float rotation = 0f)
        {
            Position = position;
            Origin = origin;
            Width = width;
            Height = height;
            Rotation = rotation;
            Corners = new Vector2[4];
            UpdateCorners();
        }

        public RotatedRectangle(Vector2 center, int width, int height, float rotation = 0f)
        : this(
            position: center - new Vector2(width / 2f, height / 2f),
            origin: new Vector2(width / 2f, height / 2f),
            width,
            height,
            rotation
        ){}

        public RotatedRectangle(Rectangle rect, float rotation = 0f)
        : this(
            new Vector2(rect.X, rect.Y),
            new Vector2(rect.Width / 2f, rect.Height / 2f),
            rect.Width,
            rect.Height,
            rotation
        ) { }

        private void UpdateCorners()
        {
            Vector2[] localCorners =
            [
                new Vector2(0, 0),         
                new Vector2(Width, 0),     
                new Vector2(Width, Height),
                new Vector2(0, Height)     
            ];

            for (int i = 0; i < 4; i++)
            {
                Corners[i] = Position + RotateVector(localCorners[i] - Origin, Rotation) + Origin;
            }
        }

        private Vector2 RotateVector(Vector2 v, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            return new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
        }

        public void SetRotation(float angle)
        {
            Rotation = angle;
            UpdateCorners();
        }

        public bool Contains(Vector2 point)
        {
            Vector2 localPoint = RotateVector(point - Position, -Rotation);
            localPoint += Origin;
            return localPoint.X >= 0 && localPoint.X <= Width &&
                   localPoint.Y >= 0 && localPoint.Y <= Height;
        }

        public bool InPlayerInteractionRange(TileReachCheckSettings settings)
        {
            Vector2 playerPosition = Main.LocalPlayer.Center;
            Vector2 closestCorner = Corners[0];
            float minDistance = Vector2.Distance(playerPosition, Corners[0]);
            for (int i = 1; i < Corners.Length; i++)
            {
                float distance = Vector2.Distance(playerPosition, Corners[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCorner = Corners[i];
                }
            }

            Point tileLocation = closestCorner.ToTileCoordinates();
            return Main.LocalPlayer.IsInTileInteractionRange(tileLocation.X, tileLocation.Y, settings);
        }


        public bool Intersects(RotatedRectangle other) => SATCollisionCheck(this, other) && SATCollisionCheck(other, this);
        public bool Intersects(Rectangle other) => Intersects(new RotatedRectangle(other));

        private bool SATCollisionCheck(RotatedRectangle rectA, RotatedRectangle rectB)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 edge = rectA.Corners[(i + 1) % 4] - rectA.Corners[i];
                Vector2 axis = new(-edge.Y, edge.X);
                axis.Normalize();

                (float minA, float maxA) = ProjectRectangle(rectA, axis);
                (float minB, float maxB) = ProjectRectangle(rectB, axis);

                if (maxA < minB || maxB < minA)
                    return false;
            }

            return true;
        }

        private readonly (float, float) ProjectRectangle(RotatedRectangle rect, Vector2 axis)
        {
            float min = Vector2.Dot(axis, rect.Corners[0]);
            float max = min;

            for (int i = 1; i < 4; i++)
            {
                float projection = Vector2.Dot(axis, rect.Corners[i]);
                if (projection < min) min = projection;
                if (projection > max) max = projection;
            }

            return (min, max);
        }

        public void DrawDebugBounds(SpriteBatch spriteBatch, Color color)
        {
            static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
            {
                Vector2 edge = end - start;
                float angle = (float)Math.Atan2(edge.Y, edge.X);
                float length = edge.Length();

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, start, new Rectangle(0, 0, 1, 1), color, angle, Vector2.Zero, new Vector2(length, 2), SpriteEffects.None, 0);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 start = Corners[i] - Main.screenPosition;
                Vector2 end = Corners[(i + 1) % 4] - Main.screenPosition;
                DrawLine(spriteBatch, start, end, color);
            }
        }
    }
}
