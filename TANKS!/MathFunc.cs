using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TANKS_
{
    internal static class MathFunc
    {
        public static Action ActionEmpty = () => { };
        public const float PiOver16 = MathHelper.PiOver4 / 4f;

        const float m = 0.06f;
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Time(this GameTime gametime)
        {
            return (float)(gametime.ElapsedGameTime.TotalMilliseconds * m);
        }

        public static float GetAngleDiff(float a, float b)
        {
            a = MathHelper.WrapAngle(a);
            b = MathHelper.WrapAngle(b);

            float angleDifference = b - a;

            return MathHelper.WrapAngle(angleDifference);
        }

        public static int RotateDirection(float currentAngle, float targetAngle)
        {
            float angleDifference = GetAngleDiff(currentAngle, targetAngle);

            if (angleDifference > 0)
            {
                return 1;
            }
            else if (angleDifference < 0)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        public static Rectangle RectangleFromCenterSize(Point center, Point size)
        {
            return new Rectangle(center - new Point(size.X >> 1, size.Y >> 1), size);
        }
        public static float Normailise(float max, float min, float value)
        {
            return (Math.Max(min, Math.Min(max, value)) - min) / (max - min);
        }

        public static Vector2 UnitVectorPoint(Vector2 fromA, Vector2 toB)
        {
            return Vector2.Normalize(toB - fromA);
        }
        public static float VectorPointAngle(Vector2 PointFrom, Vector2 PointTo)
        {
            return MathF.Atan2(PointTo.Y - PointFrom.Y, PointTo.X - PointFrom.X);
        }

        public static Vector2 AbsCopy(this Vector2 vector2)
        {
            return new Vector2(Math.Abs(vector2.X), Math.Abs(vector2.Y));
        }
        public static Vector2 SetVectorAngle(float rotation, Vector2 vector)
        {
            float magnitude = vector.Length();
            float radians = (float)(rotation * MathF.PI / 180);
            float x = magnitude * (float)MathF.Cos(radians);
            float y = magnitude * (float)MathF.Sin(radians);
            return new Vector2(x, y);
        }

        public static Vector2 RotateVector(Vector2 vector, float angle)
        {
            float angleInRadians = MathHelper.ToRadians(angle);
            float cos = (float)Math.Cos(angleInRadians);
            float sin = (float)Math.Sin(angleInRadians);

            float newX = vector.X * cos - vector.Y * sin;
            float newY = vector.X * sin + vector.Y * cos;

            return new Vector2(newX, newY);
        }
        public static Vector2 RotateVectorRad(Vector2 vector, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            float newX = vector.X * cos - vector.Y * sin;
            float newY = vector.X * sin + vector.Y * cos;

            return new Vector2(newX, newY);
        }
        public static Vector2 RotateVectorOrigin(Vector2 vector, float angle, Vector2 origin)
        {
            Vector2 translatedVector = vector - origin;

            float angleInRadians = MathHelper.ToRadians(angle);
            float cos = (float)Math.Cos(angleInRadians);
            float sin = (float)Math.Sin(angleInRadians);

            float newX = translatedVector.X * cos - translatedVector.Y * sin;
            float newY = translatedVector.X * sin + translatedVector.Y * cos;

            Vector2 rotatedVector = new Vector2(newX, newY) + origin;

            return rotatedVector;
        }

        /// <summary>
        /// From: 
        /// https://forum.unity.com/threads/line-intersection.17384/#post-4442284
        /// </summary>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DoLinesIntersect(Vector2 line1point1, Vector2 line1point2, Vector2 line2point1, Vector2 line2point2)
        {
            Vector2 a = line1point2 - line1point1;
            Vector2 b = line2point1 - line2point2;
            Vector2 c = line1point1 - line2point1;

            float alphaNumerator = b.Y * c.X - b.X * c.Y;
            float betaNumerator = a.X * c.Y - a.Y * c.X;
            float denominator = a.Y * b.X - a.X * b.Y;

            if (denominator == 0)
            {
                return false;
            }
            else if (denominator > 0)
            {
                if (alphaNumerator < 0 || alphaNumerator > denominator || betaNumerator < 0 || betaNumerator > denominator)
                {
                    return false;
                }
            }
            else if (alphaNumerator > 0 || alphaNumerator < denominator || betaNumerator > 0 || betaNumerator < denominator)
            {
                return false;
            }
            return true;
        }
        public static Point Map(this Point point, Func<int, int> mapper)
        {
            return new Point(mapper(point.X), mapper(point.Y));
        }

        public static Vector2 SetDistance(float newDistance, Vector2 vector)
        {
            float currentAngle = MathF.Atan2(vector.Y, vector.X);
            return new Vector2(newDistance * MathF.Cos(currentAngle), newDistance * MathF.Sin(currentAngle));
        }

        public static float AngleBetweenVectors(Vector2 vector1, Vector2 vector2)
        {
            float angleA = GetAngle(vector1) + 720;
            float angleB = GetAngle(vector2) + 720;

            float difference = angleA - angleB;
            if (Math.Abs(difference) > 180)
            {
                difference = angleB - angleA;
            }
            return difference;
        }

        public static Vector2 NormalisedNormalVector(Vector2 point1, Vector2 point2)
        {
            Vector2 direction = point2 - point1;

            Vector2 normalVector = new Vector2(-direction.Y, direction.X);

            return Vector2.Normalize(normalVector);
        }

        /// <summary>
        /// Extension method for generating a random boolean value based on a given probability.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> instance to use for random number generation.</param>
        /// <param name="probability">The probability of returning true, expressed as a float between 0 and 1 (inclusive).</param>
        /// <returns>True with the specified probability, otherwise false.</returns>
        /// <remarks>
        /// The <paramref name="probability"/> parameter represents the likelihood of the method returning true.
        /// If the provided probability is 0, the method will always return false.
        /// If the provided probability is 1, the method will always return true.
        /// </remarks>
        public static bool RandomProb(this Random random, float probability)
        {
            return random.NextDouble() < probability;
        }

        public static bool RandomProb(this Random random, double probability)
        {
            return random.NextDouble() < probability;
        }

        public static float GetAngle(Vector2 value)
        {
            return (float)(MathF.Atan2(value.Y, value.X) * 180 / MathF.PI);
        }
        public static float GetAngleRad(Vector2 value)
        {
            return MathF.Atan2(value.Y, value.X);
        }
        /// <summary>
        /// Gets the Vector version of a point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 V(this Point point)
        {
            return point.ToVector2();
        }
        /// <summary>
        /// Gets the Point version of a vector
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point P(this Vector2 point)
        {
            return point.ToPoint();
        }

        /// <summary>
        /// Multiplies this point and returns a copy
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point MultiCopy(this Point a, Point b)
        {
            return new Point(a.X * b.X, a.Y * b.Y);
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point MultiCopy(this Point a, int b)
        {
            return new Point(a.X * b, a.Y * b);
        }

        public static string CenterStr(string str, Vector2 Location, SpriteFont Font, out Vector2 newLocation, Vector2? fontsize = null)
        {
            newLocation = Location - Font.MeasureString(str) * 0.5f * fontsize ?? Vector2.One;
            return str;
        }

        public static float NextFloat(this Random random, float min, float max)
        {
            return (float)(random.NextDouble() * (max - min)) + min;
        }

        public static void IForEach<T>(List<T> List, Action<T> OnEach)
        {
            for (int i = List.Count - 1; i >= 0; i--)
                OnEach(List[i]);
        }

        public static Vector2 Clockwise(this Vector2 vector2)
        {
            return new Vector2(vector2.Y, -vector2.X);
        }

        public static Vector2 CounterClockwise(this Vector2 vector2)
        {
            return new Vector2(-vector2.Y, vector2.X);
        }

        public static float SineRange(float min, float max, float Mx, float X)
        {
            return (MathF.Sin(X * Mx) + 1) * 0.5f * (max - min) + min;
        }

        public static Vector2 RandomVector()
        {
            return new Vector2((float)Random.Shared.NextDouble() * 2 - 1, (float)Random.Shared.NextDouble() * 2 - 1);
        }

        public static Vector2 PointInRectangle(Rectangle bounds, float inset = 0, Random random = null)
        {
            return new Vector2(
                (random ?? Random.Shared).NextFloat(bounds.Left + inset, bounds.Right - inset),
                (random ?? Random.Shared).NextFloat(bounds.Top + inset, bounds.Bottom - inset)
                );
        }

        public static Rectangle OffsetCopy(this Rectangle rectangle, Point Amount)
        {
            return new Rectangle(rectangle.Location + Amount, rectangle.Size);
        }

        public static int TaxicabDistance(Point A, Point B)
        {
            return Math.Abs(A.X - B.X) + Math.Abs(A.Y - B.Y);
        }

        public static float TaxicabDistance(Vector2 A, Vector2 B)
        {
            return Math.Abs(A.X - B.X) + Math.Abs(A.Y - B.Y);
        }
    }
}
