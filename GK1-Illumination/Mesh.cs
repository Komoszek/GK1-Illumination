using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Numerics;

namespace GK1_Illumination
{
    public class Mesh
    {
        public List<Vector3> vertices;
        public double ratio = 0.9;
        public double vr = 3;

        public int sliceCount;
        public int polygonDegree;

        public List<int[]> triangles;

        public Mesh(int n, int b)
        {
            SetupMesh(n, b);
        }

        public Mesh(int d)
        {
            SetupMesh(d, d * 4);
        }

        public void SetupMesh(int n, int b)
        {
            vertices = new List<Vector3>();
            triangles = new List<int[]>();
            sliceCount = n;
            polygonDegree = b;
            int k = b;
            vertices.Add(new Vector3(0, 0, 1));

            double alpha = Math.PI / (2.0 * n);
            double ia = alpha;

            for (int i = 0; i < n; i++)
            {
                double r = Math.Sin(ia);
                double z = Math.Cos(ia);
                double beta = Math.PI * 2.0 / k;
                double ib = (-beta / 2) * i;

                for (int j = 0; j < k; j++)
                {
                    double x = r * Math.Cos(ib);
                    double y = r * Math.Sin(ib);
                    vertices.Add(new Vector3((float)x, (float)y, (float)z));

                    ib += beta;
                }

                ia += alpha;
            }

            // add triangles
            int[] tempArr;
            for(int i = 1; i < b; i++)
            {
                tempArr = new int[] { 0, i, i + 1 };
                triangles.Add(tempArr);
            }

            tempArr = new int[] { 0, 1, b };
            triangles.Add(tempArr);

            //down triangles
            
            for(int i = 1; i < vertices.Count - polygonDegree; i++)
            {
                int k1 = i + polygonDegree;
                int k2 = i + polygonDegree + 1;
                if (i % polygonDegree == 0)
                    k2 -= polygonDegree;
                tempArr = new int[] { i, k1, k2 };
                triangles.Add(tempArr);
            }
            
            // up triangles
            
            for (int i = 1 + polygonDegree; i < vertices.Count; i++)
            {
                int k1 = i - polygonDegree;
                int k2 = i - polygonDegree - 1;

                if ((i - 1) % polygonDegree == 0)
                    k2 += polygonDegree;

                tempArr = new int[] { i,  k1, k2 };
                triangles.Add(tempArr);
            }
        }

        public void drawVertices(Graphics g, int width, int height, float vRadius, int selectedVertex)
        {
            int x = width / 2;
            int y = height / 2;
            int R = (int)(Math.Min(x, y) * ratio);

            for(int i = 0; i < vertices.Count; i++)
            {
                g.FillEllipse(i == selectedVertex ? Brushes.Brown : Brushes.Yellow, (int)(vertices[i].X * R) + x - vRadius, (int)(vertices[i].Y * R) + y - vRadius, vRadius * 2, vRadius * 2);
            }
        }

        public void drawMesh(Graphics g, int width, int height)
        {
            int x = width / 2;
            int y = height / 2;
            int R = (int)(Math.Min(x, y) * ratio);

            // draw mesh
            int ipol = 1;

            for (int i = 0; i < sliceCount; i++)
            {
                for (int j = 0; j < polygonDegree - 1; j++)
                {
                    drawMeshLine(g, Pens.Green, vertices[ipol + j], vertices[ipol + j + 1], x, y, R);
                }

                drawMeshLine(g, Pens.Green, vertices[ipol], vertices[ipol + polygonDegree - 1], x, y, R);
                ipol += polygonDegree;
            }

            
            for(int i = 0; i < polygonDegree; i++)
            {
                drawMeshLine(g, i % 2 == 0 ? Pens.Red : Pens.Blue, vertices[0], vertices[1 + i], x, y, R);
            }

            ipol = 1;
            for(int i = 1; i < sliceCount; i++)
            {
                for(int j = 0; j < polygonDegree; j++)
                {                    
                    int k1 = j;
                    int k2 = (j + 1) % polygonDegree;

                    drawMeshLine(g, Pens.Red, vertices[ipol + j], vertices[ipol + polygonDegree + k1], x, y, R);
                    drawMeshLine(g, Pens.Blue, vertices[ipol + j], vertices[ipol + polygonDegree + k2], x, y, R);
                }

                ipol += polygonDegree;
            }
        }

        private void drawMeshLine(Graphics g, Pen pen, Vector3 v1, Vector3 v2, int x, int y, int R)
        {
            g.DrawLine(pen, (int)(v1.X * R) + x, (int)(v1.Y * R) + y, (int)(v2.X * R) + x, (int)(v2.Y * R) + y);
        }
    }
}
