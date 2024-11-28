using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable IDE0251 // Make member 'readonly'

namespace Macrocosm.Common.DataStructures
{
    public struct RectangleTest : IEquatable<RectangleTest>
    {
        #region Public Properties

        /// <summary>
        /// Returns the x coordinate of the left edge of this <see cref="RectangleTest"/>.
        /// </summary>
        public float Left => X;

        /// <summary>
        /// Returns the x coordinate of the right edge of this <see cref="RectangleTest"/>.
        /// </summary>
        public float Right => (X + Width);

        /// <summary>
        /// Returns the y coordinate of the top edge of this <see cref="RectangleTest"/>.
        /// </summary>
        public float Top => Y;

        /// <summary>
        /// Returns the y coordinate of the bottom edge of this <see cref="RectangleTest"/>.
        /// </summary>
        public float Bottom => (Y + Height);

        /// <summary>
        /// The top-left coordinates of this <see cref="RectangleTest"/>.
        /// </summary>
        public Vector2 Location
        {
            get => new(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        /// A <see cref="Point"/> located in the center of this <see cref="RectangleTest"/>'s bounds.
        /// </summary>
        /// <remarks>
        /// If <see cref="Width"/> or <see cref="Height"/> is an odd number,
        /// the center point will be rounded down.
        /// </remarks>
        public Vector2 Center => new(
                    X + (Width / 2f),
                    Y + (Height / 2f)
                );

        /// <summary>
        /// Whether or not this <see cref="RectangleTest"/> has a width and
        /// height of 0, and a position of (0, 0).
        /// </summary>
        public bool IsEmpty => ((Width == 0) &&
                                (Height == 0) &&
                                (X == 0) &&
                                (Y == 0));

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Returns a <see cref="RectangleTest"/> with X=0, Y=0, Width=0, and Height=0.
        /// </summary>
        public static RectangleTest Empty => emptyRectangle;

        #endregion

        #region Internal Properties

        internal string DebugDisplayString => string.Concat(
                    X.ToString(), " ",
                    Y.ToString(), " ",
                    Width.ToString(), " ",
                    Height.ToString()
                );

        #endregion

        #region Public Fields

        /// <summary>
        /// The x coordinate of the top-left corner of this <see cref="RectangleTest"/>.
        /// </summary>
        public float X;

        /// <summary>
        /// The y coordinate of the top-left corner of this <see cref="RectangleTest"/>.
        /// </summary>
        public float Y;

        /// <summary>
        /// The width of this <see cref="RectangleTest"/>.
        /// </summary>
        public float Width;

        /// <summary>
        /// The height of this <see cref="RectangleTest"/>.
        /// </summary>
        public float Height;

        #endregion

        #region Private Static Fields

        private static RectangleTest emptyRectangle = new();

        #endregion

        #region Public Constructors

        /// <summary>
        /// Creates a <see cref="RectangleTest"/> with the specified
        /// position, width, and height.
        /// </summary>
        /// <param name="x">The x coordinate of the top-left corner of the created <see cref="RectangleTest"/>.</param>
        /// <param name="y">The y coordinate of the top-left corner of the created <see cref="RectangleTest"/>.</param>
        /// <param name="width">The width of the created <see cref="RectangleTest"/>.</param>
        /// <param name="height">The height of the created <see cref="RectangleTest"/>.</param>
        public RectangleTest(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        #endregion

        #region Public Methods
        public Rectangle ToRectangleOuter()
        {
            return new Rectangle(
                (int)Math.Floor(X),
                (int)Math.Floor(Y),
                (int)Math.Ceiling(Width),
                (int)Math.Ceiling(Height)
            );
        }

        public Rectangle ToRectangleInner()
        {
            return new Rectangle(
                (int)Math.Ceiling(X),
                (int)Math.Ceiling(Y),
                (int)Math.Floor(Width),
                (int)Math.Floor(Height)
            );
        }

        /// <summary>
        /// Gets whether or not the provided coordinates lie within the bounds of this <see cref="RectangleTest"/>.
        /// </summary>
        /// <param name="x">The x coordinate of the point to check for containment.</param>
        /// <param name="y">The y coordinate of the point to check for containment.</param>
        /// <returns><c>true</c> if the provided coordinates lie inside this <see cref="RectangleTest"/>. <c>false</c> otherwise.</returns>
        public bool Contains(float x, float y)
        {
            return ((X <= x) &&
                    (x < (X + Width)) &&
                    (Y <= y) &&
                    (y < (Y + Height)));
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Vector2"/> lies within the bounds of this <see cref="RectangleTest"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="RectangleTest"/>.</param>
        /// <returns><c>true</c> if the provided <see cref="Vector2"/> lies inside this <see cref="RectangleTest"/>. <c>false</c> otherwise.</returns>
        public bool Contains(Vector2 value)
        {
            return ((X <= value.X) &&
                    (value.X < (X + Width)) &&
                    (Y <= value.Y) &&
                    (value.Y < (Y + Height)));
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="RectangleTest"/> lies within the bounds of this <see cref="RectangleTest"/>.
        /// </summary>
        /// <param name="value">The <see cref="RectangleTest"/> to check for inclusion in this <see cref="RectangleTest"/>.</param>
        /// <returns><c>true</c> if the provided <see cref="RectangleTest"/>'s bounds lie entirely inside this <see cref="RectangleTest"/>. <c>false</c> otherwise.</returns>
        public bool Contains(RectangleTest value)
        {
            return ((X <= value.X) &&
                    ((value.X + value.Width) <= (X + Width)) &&
                    (Y <= value.Y) &&
                    ((value.Y + value.Height) <= (Y + Height)));
        }

        public void Contains(ref Point value, out bool result)
        {
            result = ((X <= value.X) &&
                    (value.X < (X + Width)) &&
                    (Y <= value.Y) &&
                    (value.Y < (Y + Height)));
        }

        public void Contains(ref RectangleTest value, out bool result)
        {
            result = ((X <= value.X) &&
                    ((value.X + value.Width) <= (X + Width)) &&
                    (Y <= value.Y) &&
                    ((value.Y + value.Height) <= (Y + Height)));
        }

        /// <summary>
        /// Increments this <see cref="RectangleTest"/>'s <see cref="Location"/> by the
        /// x and y components of the provided <see cref="Point"/>.
        /// </summary>
        /// <param name="offset">The x and y components to add to this <see cref="RectangleTest"/>'s <see cref="Location"/>.</param>
        public void Offset(Point offset)
        {
            X += offset.X;
            Y += offset.Y;
        }

        /// <summary>
        /// Increments this <see cref="RectangleTest"/>'s <see cref="Location"/> by the
        /// provided x and y coordinates.
        /// </summary>
        /// <param name="offsetX">The x coordinate to add to this <see cref="RectangleTest"/>'s <see cref="Location"/>.</param>
        /// <param name="offsetY">The y coordinate to add to this <see cref="RectangleTest"/>'s <see cref="Location"/>.</param>
        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Inflate(float horizontalValue, float verticalValue)
        {
            X -= horizontalValue;
            Y -= verticalValue;
            Width += horizontalValue * 2;
            Height += verticalValue * 2;
        }

        /// <summary>
        /// Checks whether or not this <see cref="RectangleTest"/> is equivalent
        /// to a provided <see cref="RectangleTest"/>.
        /// </summary>
        /// <param name="other">The <see cref="RectangleTest"/> to test for equality.</param>
        /// <returns>
        /// <c>true</c> if this <see cref="RectangleTest"/>'s x coordinate, y coordinate, width, and height
        /// match the values for the provided <see cref="RectangleTest"/>. <c>false</c> otherwise.
        /// </returns>
        public bool Equals(RectangleTest other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Checks whether or not this <see cref="RectangleTest"/> is equivalent
        /// to a provided object.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to test for equality.</param>
        /// <returns>
        /// <c>true</c> if the provided object is a <see cref="RectangleTest"/>, and this
        /// <see cref="RectangleTest"/>'s x coordinate, y coordinate, width, and height
        /// match the values for the provided <see cref="RectangleTest"/>. <c>false</c> otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is RectangleTest rect) && this == rect;
        }

        public override string ToString()
        {
            return (
                "{X:" + X.ToString() +
                " Y:" + Y.ToString() +
                " Width:" + Width.ToString() +
                " Height:" + Height.ToString() +
                "}"
            );
        }

        /// <summary>
        /// Gets whether or not the other <see cref="RectangleTest"/> intersects with this rectangle.
        /// </summary>
        /// <param name="value">The other rectangle for testing.</param>
        /// <returns><c>true</c> if other <see cref="RectangleTest"/> intersects with this rectangle; <c>false</c> otherwise.</returns>
        public bool Intersects(RectangleTest value)
        {
            return (value.Left < Right &&
                    Left < value.Right &&
                    value.Top < Bottom &&
                    Top < value.Bottom);
        }

        /// <summary>
        /// Gets whether or not the other <see cref="RectangleTest"/> intersects with this rectangle.
        /// </summary>
        /// <param name="value">The other rectangle for testing.</param>
        /// <param name="result"><c>true</c> if other <see cref="RectangleTest"/> intersects with this rectangle; <c>false</c> otherwise. As an output parameter.</param>
        public void Intersects(ref RectangleTest value, out bool result)
        {
            result = (value.Left < Right &&
                    Left < value.Right &&
                    value.Top < Bottom &&
                    Top < value.Bottom);
        }

        #endregion

        #region Public Static Methods

        public static bool operator ==(RectangleTest a, RectangleTest b)
        {
            return ((a.X == b.X) &&
                    (a.Y == b.Y) &&
                    (a.Width == b.Width) &&
                    (a.Height == b.Height));
        }

        public static bool operator !=(RectangleTest a, RectangleTest b)
        {
            return !(a == b);
        }

        public static RectangleTest Intersect(RectangleTest value1, RectangleTest value2)
        {
            Intersect(ref value1, ref value2, out RectangleTest rectangle);
            return rectangle;
        }

        public static void Intersect(
            ref RectangleTest value1,
            ref RectangleTest value2,
            out RectangleTest result
        )
        {
            if (value1.Intersects(value2))
            {
                float right_side = Math.Min(
                    value1.X + value1.Width,
                    value2.X + value2.Width
                );
                float left_side = Math.Max(value1.X, value2.X);
                float top_side = Math.Max(value1.Y, value2.Y);
                float bottom_side = Math.Min(
                    value1.Y + value1.Height,
                    value2.Y + value2.Height
                );
                result = new RectangleTest(
                    left_side,
                    top_side,
                    right_side - left_side,
                    bottom_side - top_side
                );
            }
            else
            {
                result = new RectangleTest(0, 0, 0, 0);
            }
        }

        public static RectangleTest Union(RectangleTest value1, RectangleTest value2)
        {
            float x = Math.Min(value1.X, value2.X);
            float y = Math.Min(value1.Y, value2.Y);
            return new RectangleTest(
                x,
                y,
                Math.Max(value1.Right, value2.Right) - x,
                Math.Max(value1.Bottom, value2.Bottom) - y
            );
        }

        public static void Union(ref RectangleTest value1, ref RectangleTest value2, out RectangleTest result)
        {
            result.X = Math.Min(value1.X, value2.X);
            result.Y = Math.Min(value1.Y, value2.Y);
            result.Width = Math.Max(value1.Right, value2.Right) - result.X;
            result.Height = Math.Max(value1.Bottom, value2.Bottom) - result.Y;
        }
        #endregion
    }

}
