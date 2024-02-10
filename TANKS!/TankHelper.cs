using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TANKS_
{
    public static class Helper
    {
        /// <summary>
        /// Radian representation of 90 degrees
        /// </summary>
        public static readonly float NinetyDegrees = MathHelper.ToRadians(90);
        /// <summary>
        /// Radian representation of 180 degrees
        /// </summary>
        public static readonly float OneEightyDegrees = MathHelper.ToRadians(180);
        /// <summary>
        /// Radian representation of 60 degrees
        /// </summary>
        public static readonly float SixtyDegrees = MathHelper.ToRadians(60);
        /// <summary>
        /// Radian representation of 30 degrees
        /// </summary>
        public static readonly float ThirtyDegrees = MathHelper.ToRadians(30);
        /// <summary>
        /// Radian representation of 45 degrees
        /// </summary>
        public static readonly float FourtyFiveDegrees = MathHelper.ToRadians(45);

        /// <summary>
        /// Rotates a vector by the specified number of radians
        /// </summary>
        /// <param name="value">The vector to be rotated.</param>
        /// <param name="radians">The amount to rotate the vector.</param>
        /// <returns>A rotated copy of value.</returns>
        /// <remarks>
        /// A positive angle and negative angle
        /// would rotate counterclockwise and clockwise,
        /// respectively
        /// </remarks>
        public static Vector2 Rotate(Vector2 value, float radians)
        {
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);

            return new Vector2(value.X * cos - value.Y * sin, value.X * sin + value.Y * cos);
        }

        /// <summary>
        /// Rotates a <see cref="Vector2"/> around another <see cref="Vector2"/> representing a location
        /// </summary>
        /// <param name="value">The <see cref="Vector2"/> to be rotated</param>
        /// <param name="origin">The origin location to be rotated around</param>
        /// <param name="radians">The amount to rotate by in radians</param>
        /// <returns>The rotated <see cref="Vector2"/></returns>
        /// <remarks>
        /// A positive angle and negative angle
        /// would rotate counterclockwise and clockwise,
        /// respectively
        /// </remarks>
        public static Vector2 RotateAround(Vector2 value, Vector2 origin, float radians)
        {
            return Rotate(value - origin, radians) + origin;
        }

        /// <summary>
        /// Gets the angles of a vector with origin at 0,0
        /// </summary>
        /// <param name="value">The vector to measure</param>
        /// <returns>A float representing the angle of the vector in radians</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MeasureAngle(Vector2 value)
        {
            return MathF.Atan2(value.Y, value.X);
        }

        /// <summary>
        /// Generates a random boolean value based on a given probability.
        /// </summary>
        /// <param name="probability">The probability of returning true, expressed as a float between 0 and 1 (inclusive).</param>
        /// <returns>True with the specified probability, otherwise false.</returns>
        /// <remarks>
        /// The <paramref name="probability"/> parameter represents the likelihood of the method returning true.
        /// If the provided probability is 0, the method will always return false.
        /// If the provided probability is 1, the method will always return true.
        /// </remarks>
        public static bool RandomChance(float probability)
        {
            return Random.Shared.NextSingle() < probability;
        }
    }

    public static class Vector2Extensions
    {
        /// <summary>
        /// Rotates a <see cref="Vector2"/> around another <see cref="Vector2"/> representing a location
        /// </summary>
        /// <param name="origin">The origin location to be rotated around</param>
        /// <param name="radians">The amount to rotate by in radians</param>
        /// <remarks>
        /// A positive angle and negative angle
        /// would rotate counterclockwise and clockwise,
        /// respectively
        public static void RotateAround(ref this Vector2 @this, Vector2 origin, float radians)
        {
            @this -= origin;
            Helper.Rotate(@this, radians);
            @this += origin;
        }

        /// <summary>
        /// Rotates a <see cref="Vector2"/> by the specified number of radians
        /// </summary>
        /// <param name="radians">The amount to rotate this <see cref="Vector2"/>.</param>
        /// <remarks>
        /// A positive angle and negative angle
        /// would rotate counterclockwise and clockwise,
        /// respectively
        /// </remarks>
        public static void Rotate(ref this Vector2 @this, float radians)
        {
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);

            float oldx = @this.X;

            @this.X = @this.X * cos - @this.Y * sin;
            @this.Y = oldx * sin + @this.Y * cos;
        }
    }
}
