using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreisannäherung
{
    class Vector
    {
        public double X;
        public double Y;

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double GetMagnitude()
        {
            double magnitude = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
            return magnitude;
        }
        public Vector GetUnitVector()
        {
            Vector unitVector = new Vector(X / GetMagnitude(), Y / GetMagnitude());
            return unitVector;
        }
        public double GetDegrees()
        {
            double degrees = Math.Atan2(Y, X) * 180 / Math.PI;
            return degrees;
        }
        public double GetRadians()
        {
            return GetDegrees() / 180 * Math.PI;
        }
        public PointF GetPointF()
        {
            return new PointF((float)X, (float)Y);
        }
        public void SetDegrees(double degrees)
        {
            double radians = degrees / 180 * Math.PI;
            double magnitude = GetMagnitude();
            X = Math.Cos(radians) * magnitude;
            Y = Math.Sin(radians) * magnitude;
        }
    }
}
