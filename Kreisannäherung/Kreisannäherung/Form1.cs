using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Vector unitVector = new Vector(X/GetMagnitude(), Y/GetMagnitude());
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

    public partial class Form1 : Form
    {
        float ts = 150; // tilesize
        List<Vector> verteces = new List<Vector> { new Vector(0, 0), new Vector(1, 0), new Vector(1, 1), new Vector(0, 1) };
        public Form1()
        {
            InitializeComponent();

            Paint += Form1_Paint;
            KeyPress += Form1_KeyPress;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Go between each two verteces and create a new vertex in the middle, offset it at angle
            float anglePerVertex = 360 / (verteces.Count*2);
            for (int v = 0; v < verteces.Count; v+=2)
            {
                Vector p1 = verteces[v];
                Vector p2;
                if (v == verteces.Count - 1)
                    p2 = verteces[0];
                else
                    p2 = verteces[v + 1];

                Vector P1P2 = new Vector(p2.X - p1.X, p2.Y - p1.Y);
                Vector halfP1P2 = new Vector((p2.X - p1.X) / 2, (p2.Y - p1.Y) / 2);
                // Intersection between hypotenuse and cathetus
                Vector hypotenuse = new Vector(p2.X - p1.X, p2.Y - p1.Y); // A
                hypotenuse.SetDegrees(hypotenuse.GetDegrees() - anglePerVertex/2);
                Vector cathetus = new Vector(p2.X - p1.X, p2.Y - p1.Y); // B
                cathetus.SetDegrees(cathetus.GetDegrees() - 90);

                double A1 = (p1.Y + hypotenuse.Y) - p1.Y;
                double B1 = p1.X - (p1.X + hypotenuse.X);
                double C1 = A1 * p1.X + B1 * p1.Y;

                double A2 = (p1.Y + halfP1P2.Y + cathetus.Y) - (p1.Y + halfP1P2.Y);
                double B2 = (p1.X + halfP1P2.X) - (p1.X + halfP1P2.X + cathetus.X);
                double C2 = A2 * (p1.X + halfP1P2.X) + B2 * (p1.Y + halfP1P2.Y);

                double det = A1 * B2 - A2 * B1;
                if (det == 0)
                {
                    MessageBox.Show("parallel");
                }
                else
                {
                    double x = (B2 * C1 - B1 * C2) / det;
                    double y = (A1 * C2 - A2 * C1) / det;

                    Vector newPoint = new Vector(x, y);
                    verteces.Insert(v+1, newPoint);
                }
            }
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(50, 50);
            e.Graphics.ScaleTransform(ts, ts);
            Pen pen1 = new Pen(Color.Black, 3 / ts);
            Pen pen2 = new Pen(Color.Black, 1 / ts);
            Pen pen3 = new Pen(Color.Blue, 3 / ts);
            // Grid
            for (float y = -1; y < 1.6; y+=0.2f)
            {
                Pen pen = pen2;
                if (Math.Round(y,1) == 0)
                    pen = pen3;
                e.Graphics.DrawLine(pen, -1, y, 1.6f, y);
            }
            for (float x = -1; x < 1.6; x += 0.2f)
            {
                Pen pen = pen2;
                if (Math.Round(x,1) == 0)
                    pen = pen3;
                e.Graphics.DrawLine(pen, x, -1, x, 1.6f);
            }
            int finalMarkerWidth = 5;
            double circumference1 = 0;
            for (int v = 0; v < verteces.Count - 1; v++)
            {
                e.Graphics.DrawLine(pen1, verteces[v].GetPointF(), verteces[v + 1].GetPointF() );
                e.Graphics.FillEllipse(Brushes.Red, (float)verteces[v].X - (finalMarkerWidth / ts) / 2, (float)verteces[v].Y - (finalMarkerWidth / ts) / 2, finalMarkerWidth / ts, finalMarkerWidth / ts);

                circumference1 += new Vector(verteces[v + 1].X - verteces[v].X, verteces[v + 1].Y - verteces[v].Y).GetMagnitude();
            }
            e.Graphics.DrawLine(pen1, verteces.Last().GetPointF(), verteces[0].GetPointF());
            e.Graphics.FillEllipse(Brushes.Red, (float)verteces.Last().X - (finalMarkerWidth / ts) / 2, (float)verteces.Last().Y - (finalMarkerWidth / ts) / 2, finalMarkerWidth / ts, finalMarkerWidth / ts);

            circumference1 += new Vector(verteces[0].X - verteces.Last().X, verteces[0].Y - verteces.Last().Y).GetMagnitude();
            double circumference2 = circumference1 / new Vector(verteces[verteces.Count / 2].X - verteces[0].X, verteces[verteces.Count / 2].Y - verteces[0].Y).GetMagnitude();

            string info = "Verteces: " + verteces.Count() + "\n" + 
                          "Circumference r=1.41: " + circumference1 + "\n" + 
                          "Circumference r=0.5:   " + circumference2;
            e.Graphics.DrawString(info, new Font(FontFamily.GenericSansSerif, 10/ts), Brushes.Black, Width/2/ts, 0);
        }
    }
}