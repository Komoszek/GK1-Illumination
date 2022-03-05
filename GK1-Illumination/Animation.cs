using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Numerics;

namespace GK1_Illumination
{

    public class CameraPos
    {
        public double X;
        public double Y;
        public double Z;
        public double R;

        public CameraPos()
        {
            R = 10;
            X = R;
            Y = 0;
            Z = 2;
        }
    }
    public class LightAnimation
    {
        public LightSource light;
        public CameraPos camera;

        private Stopwatch stopwatch;
        private Timer timer;

        private Action animationCallback;

        public LightAnimation(double z, Color c, Action callback) {
            light = new LightSource(z, c);
            camera = new CameraPos();

            animationCallback = callback;

            stopwatch = new Stopwatch();
            timer = new Timer
            {
                Interval = 33
            };
            timer.Tick += new EventHandler(OnTimedEvent);        
        }

        private void OnTimedEvent(Object source, EventArgs e)
        {
            UpdateAnimation(stopwatch.ElapsedMilliseconds / 1000.0);
            animationCallback.DynamicInvoke();
        }

        private void UpdateAnimation(double t)
        {
            double phi = Math.PI * t;

            light.X = Math.Sqrt(phi) * Math.Cos(phi);
            light.Y = Math.Sqrt(phi) * Math.Sin(phi);

            camera.X = camera.R * Math.Cos(phi);
            camera.Y = camera.R * Math.Sin(phi);
        }

        public void Start()
        {
            timer.Start();
            stopwatch.Start();
        }

        public void Stop()
        {
            timer.Stop();
            stopwatch.Stop();
        }
        
        public void Reset()
        {
            timer.Stop();
            light.X = 0;
            light.Y = 0;
            stopwatch.Reset();
            animationCallback();
        }

        public bool isRunning
        {
            get => stopwatch.IsRunning;
        }
    }
    public class LightSource
    {
        public double Z;
        public double X;
        public double Y;
        public Color color;

        public LightSource(double z, Color c)
        {
            X = 0;
            Y = 0;
            Z = z;
            color = c;
        }
    }
}
