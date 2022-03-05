using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace GK1_Illumination
{
    public static class Painter
    {
        private static byte[] argbValues;

        public static void drawSphereOnBitmap(Bitmap b, Mesh m, Fill f, LightSource l, CameraPos c)
        {
            float offset = b.Width / 2;
            float R = ((offset) * (float)m.ratio);

            Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);

            System.Drawing.Imaging.BitmapData bmpData =
            b.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
            b.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            int bytes = Math.Abs(bmpData.Stride) * b.Height;

            if (argbValues == null || argbValues.Length < bytes)
                argbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, bytes);

            Parallel.ForEach(m.triangles, (triangle) =>
            //foreach (int[] triangle in m.triangles)
            {
                Point[] poly = new Point[triangle.Length];
                for (int i = 0; i < poly.Length; i++)
                {
                    Vector3 v = m.vertices[triangle[i]];
                    poly[i] = new Point((int)(v.X * R) + (int)offset, (int)(v.Y * R) + (int)offset);
                }

                IColorComputer colorComputer = null;

                if (f.mode == FillMode.Exact)
                {
                    colorComputer = new ExactColorComputer(f, l, c, offset, R);
                }
                else if (f.mode == FillMode.Interpolate)
                {
                    colorComputer = new InterpolatedTriangleColorComputer(f, l, c, offset, R, poly);
                }

                fillPolygon(argbValues, bmpData.Stride, poly, colorComputer);
            }
           );

            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, bytes);

            b.UnlockBits(bmpData);
        }
        
        public static void fillPolygon(Byte[] argbData, int width, Point[] vertices, IColorComputer computer)
        {
            int[] idx = new int[vertices.Length];
            for (int i = 0; i < idx.Length; i++)
            {
                idx[i] = i;
            }

            Array.Sort(idx, (int a, int b) =>
            {
                int cmp = vertices[a].Y.CompareTo(vertices[b].Y);
                return cmp == 0 ? vertices[a].X.CompareTo(vertices[b].X) : cmp;
            });

            List<AETPointer> AET = new List<AETPointer>();
            int yMin = vertices[idx[0]].Y;
            int yMax = vertices[idx[idx.Length - 1]].Y;

            int k = 0;
            for (int y = yMin; y <= yMax; y++)
            {
                while (k < idx.Length && vertices[idx[k]].Y < y)
                {
                    int curr = idx[k];

                    if (vertices[curr].Y + 1 == y)
                    {
                        int prev = (curr - 1 + idx.Length) % idx.Length;
                        int next = (curr + 1) % idx.Length;

                        if (vertices[next].Y > vertices[curr].Y)
                        {
                            AET.Add(new AETPointer(curr, next, vertices[curr].X, vertices[curr].Y, vertices[next].X, vertices[next].Y));
                        }
                        else
                        {
                            AET.removeAETPointerWithIds(next, curr);
                        }

                        if (vertices[prev].Y > vertices[curr].Y)
                        {
                            AET.Add(new AETPointer(curr, prev, vertices[curr].X, vertices[curr].Y, vertices[prev].X, vertices[prev].Y));
                        }
                        else
                        {
                            AET.removeAETPointerWithIds(prev, curr);
                        }
                    }

                    k++;
                    if (k >= idx.Length)
                        break;
                }


                AET.Sort((AETPointer a, AETPointer b) =>
                {
                    return a.x.CompareTo(b.x);
                });

                for (int i = 0; i < AET.Count; i += 2)
                {
                    int xMin = (int)AET[i].x;
                    int xMax = (int)AET[i + 1].x;
                    for (int x = xMin; x < xMax; x++)
                    {
                        Color c = computer.ComputeColor(x, y);

                        int t = (y * width + x * 4);

                        argbData[t] = c.B; // B
                        argbData[t + 1] = c.G; // G
                        argbData[t + 2] = c.R; // R
                        argbData[t + 3] = 255; //A
                    }
                }

                for (int i = 0; i < AET.Count; i++)
                {
                    AET[i].x += AET[i].mInv;
                }
            }
        }

    }
}
