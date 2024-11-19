using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        #region Interpolation

        /// <summary>
        /// Calculates the inverse lerp value representing the relative position of the input value in the specified range.
        /// </summary>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <param name="t">The input value to calculate the inverse lerp for.</param>
        /// <param name="clamped">Specifies whether the result should be clamped between 0 and 1.</param>
        public static float InverseLerp(float from, float to, float value, bool clamped = false)
            => Terraria.Utils.GetLerpValue(from, to, value, clamped);

        /// <summary> Performs a logarithmic interpolation between 0 and 1 based on the given value and logarithmic base. </summary>
        /// /// <param name="t">The input value between 0 and 1.</param>
        /// <param name="logBase">The logarithmic base to use (optional, default = 10).</param>
        /// <returns>The interpolated value between 0 and 1 based on the logarithmic scale.</returns>
        public static float LogarithmicLerp(float t, int logBase = 10)
        {
            t = MathHelper.Clamp(t, 0, 1);
            float lerpValue = MathF.Pow(logBase, t) - 1f;
            float maxLerpValue = MathF.Pow(logBase, 1f) - 1f;
            return lerpValue / maxLerpValue;
        }
        public static float WrapProgress(float progress)
        {
            if (progress < 0.0f)
                progress = 1.0f + (progress % 1.0f);

            return progress % 1.0f;
        }
        public static float WrapLerpAngle(float a, float b, float t)
        {
            float difference = b - a;
            if (Math.Abs(difference) > MathHelper.Pi)
            {
                if (difference > 0f)
                {
                    difference -= MathHelper.TwoPi;
                }
                else
                {
                    difference += MathHelper.TwoPi;
                }
            }
            return a + difference * t;
        }

        #endregion

        #region Vectors

        public static Vector2 Absolute(this Vector2 vector)
        {
            float x = Math.Abs(vector.X);
            float y = Math.Abs(vector.Y);
            return new Vector2(x, y);
        }

        public static Vector2 ClampOutsideCircle(Vector2 point, Vector2 circleCenter, float radius)
        {
            Vector2 fromCenterToPoint = point - circleCenter;
            float distance = fromCenterToPoint.Length();

            if (distance <= radius)
            {
                if (distance == 0)
                {
                    return circleCenter + new Vector2(radius, 0);
                }
                else
                {
                    fromCenterToPoint.Normalize();
                    point = circleCenter + fromCenterToPoint * (radius + 1.0f);
                }
            }

            return point;
        }

        #endregion

        #region Rectangles

        public static System.Drawing.RectangleF ToRectangleF(this Rectangle rectangle)
        {
            return new System.Drawing.RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToRectangle(this System.Drawing.RectangleF rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        public static Rectangle ShrinkRectangle(this Rectangle rectangle, int shrinkAmount)
        {
            int newWidth = rectangle.Width - (2 * shrinkAmount);
            int newHeight = rectangle.Height - (2 * shrinkAmount);

            // Calculate the new position based on the center
            int newX = rectangle.X + shrinkAmount;
            int newY = rectangle.Y + shrinkAmount;

            return new Rectangle(newX, newY, newWidth, newHeight);
        }

        public static Rectangle AdjustAspectRatio(this Rectangle rectangle, int targetWidth, int targetHeight)
        {
            float aspectRatio = (float)rectangle.Width / rectangle.Height;
            float targetAspectRatio = (float)targetWidth / targetHeight;

            if (aspectRatio > targetAspectRatio)
            {
                // Reduce the width while maintaining the aspect ratio
                int newWidth = (int)(targetHeight * aspectRatio);
                int deltaWidth = targetWidth - newWidth;
                int newX = rectangle.X + (deltaWidth / 2);

                return new Rectangle(newX, rectangle.Y, newWidth, targetHeight);
            }
            else if (aspectRatio < targetAspectRatio)
            {
                // Reduce the height while maintaining the aspect ratio
                int newHeight = (int)(targetWidth / aspectRatio);
                int deltaHeight = targetHeight - newHeight;
                int newY = rectangle.Y + (deltaHeight / 2);

                return new Rectangle(rectangle.X, newY, targetWidth, newHeight);
            }

            return new Rectangle(rectangle.X, rectangle.Y, targetWidth, targetHeight);
        }

        public static Vector4 Normalize(this Rectangle rectangle, Vector2 maxSize)
        {
            float x = rectangle.X / maxSize.X;
            float y = rectangle.Y / maxSize.Y;
            float width = rectangle.Width / maxSize.X;
            float height = rectangle.Height / maxSize.Y;
            return new Vector4(x, y, width, height);
        }
        #endregion

        #region Rotation & angles

        /// <summary>
        /// Converts the polar coordinates of a point to it's cartesian ones
        /// </summary>
        /// <param name="radius"> The radius, i.e. the L2 distance between the point and the origin </param>
        /// <param name="theta"> The angle (in radians) with respect to the positive X axis </param>
        /// <returns> The point's cartesian coordinates relative to the origin  </returns>
        public static Vector2 PolarVector(float radius, float theta)
            => new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;

        /// <summary>
        /// Gets the position of a point rotating about an origin using a rotation matrix
        /// </summary>
        /// <param name="origin"> The rotation origin </param>
        /// <param name="offset"> The offset of the point from the origin when there is no rotation (<paramref name="theta"> = 0f) </param>
        /// <param name="theta"> The rotation angle (in radians) </param>
        /// <returns> The rotated point coordinates </returns>
        public static Vector2 RotatingPoint(Vector2 origin, Vector2 offset, float theta)
        {
            offset += origin;
            return new(
                origin.X + (offset.X - origin.X) * (float)Math.Cos(theta) - (offset.Y - origin.Y) * (float)Math.Sin(theta),
                origin.Y + (offset.X - origin.X) * (float)Math.Sin(theta) + (offset.Y - origin.Y) * (float)Math.Cos(theta)
            );
        }

        public static float SquareDiagonal(float sideLength)
        {
            return MathF.Sqrt(2 * MathF.Pow(sideLength, 2));
        }
        public static int SquareDiagonal(int sideLength)
        {
            return (int)MathF.Sqrt(2 * MathF.Pow(sideLength, 2));
        }

        public static float GeneralPythagoras(float width, float height)
        {
            return MathF.Sqrt(MathF.Pow(width, 2) + MathF.Pow(height, 2));
        }
        public static int GeneralPythagoras(int width, int height)
        {
            return (int)MathF.Sqrt(MathF.Pow(width, 2) + MathF.Pow(height, 2));
        }

        #endregion

        #region Signals

        /// <summary> Generates a triangle wave based on the given period and phase. Use exclusively for visual effects. </summary>
        /// <param name="period">The duration of one complete cycle of the wave.</param>
        /// <param name="phase">The starting phase offset of the wave (optional).</param>
        /// <returns>The value of the triangle wave ranging from -1 to 1.</returns>
        public static float TriangleWave(float period, float phase = 0)
        {
            float time = ((float)Main.timeForVisualEffects + phase) % period;
            float position = time / period;
            float slope = position < 0.5 ? 4 * position - 1 : -4 * position + 3;
            return 2 * (slope - 0.5f);
        }

        /// <summary> Generates a sine wave based on the given period and phase. Use exclusively for visual effects.</summary>
        /// <param name="period">The duration of one complete cycle of the wave.</param>
        /// <param name="phase">The starting phase offset of the wave (optional).</param>
        /// <returns>The value of the sine wave ranging from -1 to 1.</returns>
        public static float SineWave(float period, float phase = 0)
        {
            float time = ((float)Main.timeForVisualEffects) % period;
            float angle = (2 * MathF.PI * (1 / period) * time) + phase;
            return MathF.Sin(angle);
        }

        public static float PositiveTriangleWave(float period, float phase = 0)
            => (TriangleWave(period, phase) + 1f) * 0.5f;


        /// <summary> Generates a positive-only sine wave based on the given period and phase. Use exclusively for visual effects.</summary>
        /// <param name="period">The duration of one complete cycle of the wave.</param>
        /// <param name="phase">The starting phase offset of the wave (optional).</param>
        /// <returns> The value of the positive-only sine wave ranging from 0 to 1.</returns>
        public static float PositiveSineWave(float period, float phase = 0)
            => (SineWave(period, phase) + 1f) * 0.5f;

        #endregion

        #region Easing functions

        /// <summary> Linear easing (for consistency purposes) </summary>
        public static float EaseLinear(float t) => t;

        /// <summary> Applies quadratic easing-in to the input value.  </summary>
        public static float QuadraticEaseIn(float t) => t * t;

        /// <summary> Applies quadratic easing-out to the input value. </summary>
        public static float QuadraticEaseOut(float t) => t * (2f - t);

        /// <summary>  Applies quadratic easing-in-out to the input value.  </summary>
        public static float QuadraticEaseInOut(float t)
            => (t < 0.5) ? (2f * t * t) : (-1f + (4f - 2f * t) * t);

        /// <summary> Applies cubic easing-in to the input value.  </summary>
        public static float CubicEaseIn(float t) => t * t * t;

        /// <summary> Applies cubic easing-out to the input value. </summary>
        public static float CubicEaseOut(float t)
            => (t - 1f) * (t - 1f) * (t - 1f) + 1f;

        /// <summary>  Applies cubic easing-in-out to the input value.  </summary>
        public static float CubicEaseInOut(float t)
            => (t < 0.5) ? (4f * t * t * t) : 1 - MathF.Pow(-2 * t + 2, 3) / 2;

        public static float QuartEaseIn(float t) => t * t * t * t;
        public static float QuartEaseOut(float t) => 1 - QuartEaseIn(1 - t);
        public static float QuartEaseInOut(float t)
        {
            if (t < 0.5) return QuartEaseIn(t * 2) / 2;
            return 1 - QuartEaseIn((1 - t) * 2) / 2;
        }

        public static float QuintEaseIn(float t) => t * t * t * t * t;
        public static float QuintEaseOut(float t) => 1 - QuintEaseIn(1 - t);
        public static float QuintEaseInOut(float t)
        {
            if (t < 0.5) return QuintEaseIn(t * 2) / 2;
            return 1 - QuintEaseIn((1 - t) * 2) / 2;
        }

        public static float SineEaseIn(float t) => (float)-Math.Cos(t * Math.PI / 2);
        public static float SineEaseOut(float t) => (float)Math.Sin(t * Math.PI / 2);
        public static float SineEaseInOut(float t) => (float)(Math.Cos(t * Math.PI) - 1) / -2;

        public static float ExpoEaseIn(float t) => (float)Math.Pow(2, 10 * (t - 1));
        public static float ExpoEaseOut(float t) => 1 - ExpoEaseIn(1 - t);
        public static float ExpoEaseInOut(float t)
        {
            if (t < 0.5) return ExpoEaseIn(t * 2) / 2;
            return 1 - ExpoEaseIn((1 - t) * 2) / 2;
        }

        public static float CircEaseIn(float t) => -((float)Math.Sqrt(1 - t * t) - 1);
        public static float CircEaseOut(float t) => 1 - CircEaseIn(1 - t);
        public static float CircEaseInOut(float t)
        {
            if (t < 0.5) return CircEaseIn(t * 2) / 2;
            return 1 - CircEaseIn((1 - t) * 2) / 2;
        }

        public static float ElasticEaseIn(float t) => 1 - ElasticEaseOut(1 - t);
        public static float ElasticEaseOut(float t)
        {
            float p = 0.3f;
            return (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t - p / 4) * (2 * Math.PI) / p) + 1;
        }
        public static float ElasticEaseInOut(float t)
        {
            if (t < 0.5) return ElasticEaseIn(t * 2) / 2;
            return 1 - ElasticEaseIn((1 - t) * 2) / 2;
        }

        public static float BackEaseIn(float t)
        {
            float s = 1.70158f;
            return t * t * ((s + 1) * t - s);
        }
        public static float BackEaseOut(float t) => 1 - BackEaseIn(1 - t);
        public static float BackEaseInOut(float t)
        {
            if (t < 0.5) return BackEaseIn(t * 2) / 2;
            return 1 - BackEaseIn((1 - t) * 2) / 2;
        }

        public static float BounceEaseIn(float t) => 1 - BounceEaseOut(1 - t);
        public static float BounceEaseOut(float t)
        {
            float div = 2.75f;
            float mult = 7.5625f;

            if (t < 1 / div)
            {
                return mult * t * t;
            }
            else if (t < 2 / div)
            {
                t -= 1.5f / div;
                return mult * t * t + 0.75f;
            }
            else if (t < 2.5 / div)
            {
                t -= 2.25f / div;
                return mult * t * t + 0.9375f;
            }
            else
            {
                t -= 2.625f / div;
                return mult * t * t + 0.984375f;
            }
        }
        public static float BounceEaseInOut(float t)
        {
            if (t < 0.5) return BounceEaseIn(t * 2) / 2;
            return 1 - BounceEaseIn((1 - t) * 2) / 2;
        }

        #endregion

        #region Derivatives

        /// <summary>
        /// Applies a logarithmic derivative to <paramref name="value"/>
        /// </summary>
        /// <param name="value">The initial value</param>
        /// <param name="targetValue">The target value</param>
        /// <param name="scaleFactor">The factor (commonly notated as <c>k</c>) which affects how quickly or slowly <paramref name="value"/> reaches <paramref name="targetValue"/></param>
        /// <param name="deltaTime">The change in time</param>
        /// <returns>The updated value</returns>
        public static float ScaleLogarithmic(float value, float targetValue, float scaleFactor, float deltaTime)
        {
            float dt = scaleFactor * (targetValue - value) * deltaTime;
            value += dt;
            return value;
        }

        // Thank you code from TerrariaAmbience - Ryan
        /// <summary> Creates a gradient value based on the given input value and range. </summary>
        /// <param name="value">The input value.</param>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="max">The maximum value of the range.</param>
        /// <returns>The gradient value between 0 and 1 based on the input value's position within the range.</returns>
        public static double CreateGradientValue(double value, double min, double max)
        {
            double mid = (max + min) / 2;
            double returnValue;

            if (value > mid)
            {
                var thing = 1f - (value - min) / (max - min) * 2;
                returnValue = 1f + thing;
                return returnValue;
            }
            returnValue = (value - min) / (max - min) * 2;
            returnValue = Terraria.Utils.Clamp(returnValue, 0, 1);
            return returnValue;
        }

        /// <summary> Creates a gradient value based on the given input value and range. </summary>
        /// <param name="value">The input value.</param>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="max">The maximum value of the range.</param>
        /// <returns>The gradient value between 0 and 1 based on the input value's position within the range.</returns>
        public static float CreateGradientValue(float value, float min, float max)
        {
            float mid = (max + min) / 2;
            float returnValue;

            if (value > mid)
            {
                var thing = 1f - (value - min) / (max - min) * 2;
                returnValue = 1f + thing;
                return returnValue;
            }
            returnValue = (value - min) / (max - min) * 2;
            returnValue = Terraria.Utils.Clamp(returnValue, 0, 1);
            return returnValue;
        }

        #endregion
    }
}