using System;
using System.Drawing;
using System.Numerics;

namespace GK1_Illumination
{
    public enum FillType
    {
        SolidColor,
        Bitmap
    }

    public enum FillMode
    {
        Exact,
        Interpolate
    }

    public enum VMode
    {
        Standard,
        Moving
    }

    public class Fill
    {
        private FillType type;
        public FillMode mode;
        public VMode vmode;

        private Bitmap originalTexture;
        private Bitmap originalNormalMap;

        private int strideTexture;
        private int strideNormal;

        private Byte[] resizedTextureData;
        private Byte[] resizedNormalMapData;

        private Color color;
        public int frameSize;

        public double kd;
        public double ks;
        public int m;

        public double k;

        public double h;

        public Fill(Color c, Bitmap texture, Bitmap normal, double kd, double ks, double k, int m, int h, int frameSize)
        {
            type = FillType.SolidColor;
            mode = FillMode.Exact;
            vmode = VMode.Standard;
            color = c;
            originalTexture = new Bitmap(texture);
            originalNormalMap = new Bitmap(normal);
            this.frameSize = frameSize;

            this.kd = kd;
            this.ks = ks;
            this.m = m;
            this.k = k;
            this.h = h;

            resizedTextureData = updateBitmapData(originalTexture, out strideTexture);
            resizedNormalMapData = updateBitmapData(originalNormalMap, out strideNormal);
        }

        public Color getPixelColor(int x, int y)
        {
            if (type == FillType.SolidColor)
                return color;

            int t = y * strideTexture + x * 4;
            return Color.FromArgb(resizedTextureData[t + 3], resizedTextureData[t + 2], resizedTextureData[t + 1], resizedTextureData[t]);
        }

        public Vector3 getNormalVector(int x, int y)
        {
            int t = y * strideNormal + x * 4;
            Vector3 N = new Vector3(MapColorToNormal(resizedNormalMapData[t + 2]), MapColorToNormal(resizedNormalMapData[t + 1]), MapColorToNormal(resizedNormalMapData[t]));
            return Vector3.Normalize(N);
        }

        private float MapColorToNormal(byte v)
        {
            return ((float)v * 2.0f) / 255.0f - 1.0f;
        }

        public void setTypeToColor(Color c)
        {
            type = FillType.SolidColor;
            color = c;
        }

        public void setTypeToBitmap(Bitmap b)
        {
            type = FillType.Bitmap;
            originalTexture = new Bitmap(b);

            resizedTextureData = updateBitmapData(originalTexture, out strideTexture);
        }

        public void setTypeToBitmap()
        {
            type = FillType.Bitmap;
        }

        public void setTypeToColor()
        {
            type = FillType.SolidColor;
        }

        public void setNormalMap(Bitmap b)
        {
            originalNormalMap = new Bitmap(b);

            resizedNormalMapData = updateBitmapData(originalNormalMap, out strideNormal);
        }

        public void setTypeToBitmap(String s)
        {
            Bitmap tempBitmap;

            try
            {
                tempBitmap = new Bitmap(s);
            } catch
            {
                tempBitmap = new Bitmap(1, 1);
            }

            setTypeToBitmap(tempBitmap);

            tempBitmap.Dispose();
        }

        private byte[] updateBitmapData(Bitmap b, out int stride)
        {
            Bitmap tempResized = new Bitmap(b, frameSize, frameSize);

            Rectangle rect = new Rectangle(0, 0, frameSize, frameSize);

            System.Drawing.Imaging.BitmapData bmpData =
            tempResized.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
            tempResized.PixelFormat);

            bmpData.PixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            int bytes = Math.Abs(bmpData.Stride) * frameSize;
            byte[] argbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, bytes);
            stride = bmpData.Stride;

            tempResized.Dispose();

            return argbValues;
        }

        public void setFrameSize(int d)
        {
            frameSize = d;
            resizedTextureData = updateBitmapData(originalTexture, out strideTexture);
            resizedNormalMapData = updateBitmapData(originalNormalMap, out strideNormal);
        }
    }
}
