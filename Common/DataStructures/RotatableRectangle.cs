
using Microsoft.Xna.Framework;
using System;

namespace Macrocosm.Common.DataStructures
{
    public class RotatedRectangle
    {
        public Vector2 Center { get; set; } 
        public int Width { get; }
        public int Height { get; }
        public float Rotation { get; private set; } 

        public Vector2[] Corners { get; private set; }

        public RotatedRectangle(Vector2 center, int width, int height, float rotation = 0f)
        {
            Center = center;
            Width = width;
            Height = height;
            Rotation = rotation;
            UpdateCorners();
        }

        private void UpdateCorners()
        {
            Vector2 halfSize = new(Width / 2f, Height / 2f);

            Vector2[] localCorners =
            [
            new(-halfSize.X, -halfSize.Y), // Top-left
            new(halfSize.X, -halfSize.Y),  // Top-right
            new(halfSize.X, halfSize.Y),   // Bottom-right
            new(-halfSize.X, halfSize.Y)   // Bottom-left
            ];

            Corners = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                Corners[i] = RotateVector(localCorners[i], Rotation) + Center;
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

        public bool Intersects(RotatedRectangle other) => SATCollisionCheck(this, other) && SATCollisionCheck(other, this);

        /// <summary> Separating Axis Theorem (SAT) collision detection </summary>
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

        /// <summary> Projects a rectangle onto an axis </summary>
        private (float, float) ProjectRectangle(RotatedRectangle rect, Vector2 axis)
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
    }

}
