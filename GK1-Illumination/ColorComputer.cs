using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using System.Diagnostics;

namespace GK1_Illumination
{
    public interface IColorComputer
    {
        public Color ComputeColor(int x, int y);
    }

    public class ExactColorComputer : IColorComputer
    {
        private Fill fill;
        private LightSource lightSource;
        private float centerOffset;
        private float radius;
        private CameraPos camera;

        public ExactColorComputer(Fill f, LightSource l, CameraPos c, float offset, float r)
        {
            fill = f;
            lightSource = l;
            centerOffset = offset;
            radius = r;
            camera = c;
        }

        public Color ComputeColor(int x, int y)
        {
            return ReflectionComputer.GetColor(x, y, radius, centerOffset, lightSource, camera, fill);
        }
    }

    struct TriangleColorVertex
    {
        public Point p;
        public Color color;

        public TriangleColorVertex(Point p, Color color)
        {
            this.p = p;
            this.color = color;
        }
    }

    public class InterpolatedTriangleColorComputer: IColorComputer
    {
        private Fill fill;
        private LightSource lightSource;
        private float centerOffset;
        private float radius;
        private TriangleColorVertex[] colorVertices;
        private CameraPos camera;

        public InterpolatedTriangleColorComputer(Fill f, LightSource l, CameraPos c, float offset, float r, Point[] triangles)
        {
            fill = f;
            lightSource = l;
            centerOffset = offset;
            radius = r;
            camera = c;

            UpdateColorVertices(triangles);

        }

        public void UpdateColorVertices(Point[] triangles)
        {
            colorVertices = new TriangleColorVertex[3];

            for(int i = 0; i < 3; i++)
            {
                Color c = ReflectionComputer.GetColor(triangles[i].X, triangles[i].Y, radius, centerOffset, lightSource, camera, fill);
                colorVertices[i] = new TriangleColorVertex(triangles[i], c);
            }
        }

        public Color ComputeColor(int x, int y)
        {
            // calculate color from barycentric coordinates
            Point p = new Point(x, y);

            float u = ParallelogramArea(p, colorVertices[0].p, colorVertices[1].p);
            float v = ParallelogramArea(p, colorVertices[1].p, colorVertices[2].p);
            float w = ParallelogramArea(p, colorVertices[0].p, colorVertices[2].p);

            float N = u + v + w;
            u /= N;
            v /= N;
            w /= N;

            int red = Math.Min((int)(v * colorVertices[0].color.R + w * colorVertices[1].color.R + u * colorVertices[2].color.R), 255);
            int green = Math.Min((int)(v * colorVertices[0].color.G + w * colorVertices[1].color.G + u * colorVertices[2].color.G), 255);
            int blue = Math.Min((int)(v * colorVertices[0].color.B + w * colorVertices[1].color.B + u * colorVertices[2].color.B), 255);

            return Color.FromArgb(red, green, blue);
        }

        private float ParallelogramArea(Point p1, Point p2, Point p3)
        {
            float dx1 = p2.X - p1.X;
            float dx2 = p3.X - p1.X;

            float dy1 = p2.Y - p1.Y;
            float dy2 = p3.Y - p1.Y;

            return Math.Abs(dx1 * dy2 - dx2 * dy1);
        }
    }
}
