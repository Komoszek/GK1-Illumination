using System;
using System.Numerics;
using System.Drawing;

namespace GK1_Illumination
{

    static class ReflectionComputer
    {

        private static bool inLeftCircle(float x, float y, float r, float smallr)
        {
            float dx = x - (-r + 2 * smallr);
            float dy = y;

            return dx * dx + dy * dy < smallr * smallr;
        }
        private static bool inRightCircle(float x, float y, float r, float smallr)
        {
            float dx = x - (r - 2 * smallr);
            float dy = y;

            return dx * dx + dy * dy < smallr * smallr;
        }
        private static bool inUpCircle(float x, float y, float r, float smallr)
        {
            float dx = x;
            float dy = y - (-r + 2 * smallr);

            return dx * dx + dy * dy < smallr * smallr;
        }
        private static bool inDownCircle(float x, float y, float r, float smallr)
        {
            float dx = x;
            float dy = y - (r - 2 * smallr);

            return dx * dx + dy * dy < smallr * smallr;
        }

        public static Color GetColor(int x, int y, float R, float offset, LightSource light, CameraPos camera, Fill fill)
        {
            Color iO = fill.getPixelColor(x, y);

            float xd = x - offset;
            float yd = y - offset;

            float z = R * R - xd * xd - yd * yd;

            z = z > 0 ? (float)Math.Sqrt(z) : 0;


            Vector3 Nk;

            float fillhAdjusted = (float)fill.h * R;

            if(z > fillhAdjusted)
            {

                float r = (float)Math.Sqrt(R * R - fillhAdjusted * fillhAdjusted);
                float smallr = r / 6.0f;


                if(inLeftCircle(xd, yd, r, smallr))
                {
                    Nk = Vector3.UnitX;

                    float sxd = xd - (-r + 2 * smallr);
                    float syd = yd;

                    float zsd = smallr * smallr - sxd * sxd - syd * syd;

                    zsd = zsd > 0 ? (float)Math.Sqrt(zsd) : 0;
                    Nk = Vector3.Normalize(new Vector3(-sxd, -syd, zsd));

                }
                else if (inRightCircle(xd, yd, r, smallr))
                {
                    float sxd = xd - (r - 2 * smallr);
                    float syd = yd;

                    float zsd = smallr * smallr - sxd * sxd - syd * syd;

                    zsd = zsd > 0 ? (float)Math.Sqrt(zsd) : 0;
                    Nk = Vector3.Normalize(new Vector3(-sxd, -syd, zsd));


                }
                else if (inUpCircle(xd, yd, r, smallr))
                {
                    float sxd = xd;
                    float syd = yd - (-r + 2 * smallr);

                    float zsd = smallr * smallr - sxd * sxd - syd * syd;

                    zsd = zsd > 0 ? (float)Math.Sqrt(zsd) : 0;
                    Nk = Vector3.Normalize(new Vector3(-sxd, -syd, zsd));


                }
                else if (inDownCircle(xd, yd, r, smallr))
                {

                    float sxd = xd;
                    float syd = yd - (r - 2 * smallr);

                    float zsd = smallr * smallr - sxd * sxd - syd * syd;

                    zsd = zsd > 0 ? (float)Math.Sqrt(zsd) : 0;
                    Nk = Vector3.Normalize(new Vector3(-sxd, -syd, zsd));
                }
                else
                {
                    Nk = Vector3.UnitZ;
                }
            } 
            else
            {
                Nk = Vector3.Normalize(new Vector3(xd, yd, z));
            }

            Vector3 Nt = fill.getNormalVector(x, y);

            Vector3 B;

            if (Nk.Equals(Vector3.UnitZ))
            {
                B = Vector3.UnitY;
            }
            else
            {
                B = Vector3.Cross(Nk, Vector3.UnitZ);
            }

            Vector3 T = Vector3.Cross(B, Nk);

            Vector3 Nz = MatrixMMultiply(T, B, Nk, Nt);

            Vector3 N = Nk * (float)fill.k + (float)(1 - fill.k) * Nz;

            float xL = (float)light.X * R;
            float yL = (float)light.Y * R;
            float zL = (float)light.Z * R;

            float xV = (float)camera.X * R;
            float yV = (float)camera.Y * R;
            float zV = (float)camera.Z * R;

            Vector3 L = Vector3.Normalize(new Vector3(xL - xd, yL - yd, zL - z));

            Vector3 V;

            if(fill.vmode == VMode.Standard)
            {
                V = Vector3.UnitZ;
            } else
            {
                V = Vector3.Normalize(new Vector3(xV - xd, yV - yd, zV - z));
            }

            if (Vector3.Dot(Nk, V) < 0) return Color.Black;

            return compute(fill.kd, fill.ks, fill.m, N, L, iO, light.color, V);
        }

        private static Vector3 MatrixMMultiply(Vector3 T, Vector3 B, Vector3 Nk, Vector3 Nt)
        {
            return new Vector3(CellCalculate(T.X, B.X, Nk.X, Nt), CellCalculate(T.Y, B.Y, Nk.Y, Nt), CellCalculate(T.Z, B.Z, Nk.Z, Nt));
        }

        private static float CellCalculate(float x, float y, float z, Vector3 v)
        {
            return x * v.X + y * v.Y + z * v.Z;
        }

        private static Color compute(double kd, double ks, int m, Vector3 N, Vector3 L, Color iO, Color iL, Vector3 V)
        {
            float NLDot = Math.Max(Vector3.Dot(N, L), 0);

            Vector3 R = (2 * NLDot) * N - L;

            double VRDot = Math.Max(Vector3.Dot(V, R), 0);

            double I = Math.Max(kd * NLDot + ks * Math.Pow(VRDot, m), 0);

            int Red = (int)(I * (iO.R * iL.R) / 255.0);
            int Green = (int)(I * (iO.G * iL.G) / 255.0);
            int Blue = (int)(I * (iO.B * iL.B) / 255.0);

            Red = Math.Min(Red, 255);
            Green = Math.Min(Green, 255);
            Blue = Math.Min(Blue, 255);

            return Color.FromArgb(255, Red, Green, Blue);
        }
    }
}
