using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austeroids
{
    class Render
    {
        public static Vector[] DrawCircle(Vector Center, float radius, int points)
        {
            Vector[] Points = new Vector[points];
            for (int i = 0; i < points; i++)
            {
                double rad = (i / (points - 1f)) * Math.PI * 2;
                Points[i] = Center + new Vector((float)Math.Cos(rad) * radius, (float)Math.Sin(rad) * radius);
            }

            return Points;
        }

        public static Vector[] DrawTri(Vector A, Vector B, Vector C, int countPerLine)
        {
            List<Vector> Points = new List<Vector>();
            Points.AddRange(DrawLine(A, B, countPerLine));
            Points.AddRange(DrawLine(B, C, countPerLine));
            Points.AddRange(DrawLine(C, A, countPerLine));

            return Points.ToArray();
        }

        public static Vector[] DrawLine(Vector from, Vector to, int numPoints)
        {
            Vector[] points = new Vector[numPoints];

            for (int i = 0; i < points.Length; i++)
            {
                float perc = i / (points.Length - 1f);
                points[i] = CMath.Lerp(perc, from, to);
            }

            return points;
        }

        public static Vector[] DrawPoly(Vector[] Vertices, int countPerLine)
        {
            List<Vector> Points = new List<Vector>();
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vector B = i < Vertices.Length - 1 ? Vertices[i + 1] : Vertices[0];
                Points.AddRange(DrawLine(Vertices[i], B, countPerLine));
            }

            return Points.ToArray();
        }
    }
    class CMath
    {
        public static Random Rand = new Random();

        public static float Lerp(float perc, float from, float to)
        {
            return from + perc * (to - from);
        }

        public static Vector Lerp(float perc, Vector from, Vector to)
        {
            return new Vector(Lerp(perc, from.X, to.X), Lerp(perc, from.Y, to.Y));
        }

        public static float SinBetween(float time, float low, float high)
        {
            return (float)((Math.Sin(time) + 1) / 2) * (high - low) + low;
        }

        public static int NearestOf(int num, int nearestMultiple)
        {
            return ((int)Math.Round(num / (double)nearestMultiple, MidpointRounding.AwayFromZero)) * nearestMultiple;
        }
    }

    struct Vector
    {
        private float x;
        private float y;

        public float X { get { return x; } }
        public float Y { get { return y; } }

        public static readonly Vector Zero = new Vector(0,0);

        public Vector(float X, float Y)
        {
            x = X;
            y = Y;
        }

        public Vector Rotate(float angle)
        {
            return Rotate(this, angle);
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }

        public float LengthSquared()
        {
            return x * x + y * y;
        }

        public static Vector operator +(Vector A, Vector B)
        {
            return new Vector(A.x + B.x, A.y + B.y);
        }

        public static Vector operator -(Vector A, Vector B)
        {
            return new Vector(A.x - B.x, A.y - B.y);
        }

        public static Vector operator *(Vector A, float scalar)
        {
            return new Vector(A.x * scalar, A.y * scalar);
        }

        public static Vector Rotate(Vector vec, float angle)
        {
            float y = (float)Math.Sin(angle);
            float x = (float)Math.Cos(angle);

            return new Vector(vec.X * x - vec.Y * y,
                vec.X * y + vec.Y * x);

        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", x, y);
        }

        public static Vector RandomVector( float max = 1.0f)
        {
            return new Vector((float)CMath.Rand.NextDouble() * max - max/2, (float)CMath.Rand.NextDouble() * max - max/2);
        }
    }
}
