using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Numerics;

namespace GK1_Illumination
{
    public partial class Form1 : Form
    {
        private Fill fill;
        private Mesh mesh;
        private Bitmap sphereImage;

        private LightAnimation lightAnimation;

        private float vertexRadius;

        private bool grabbing;
        private int selectedVertex;


        public Form1()
        {
            InitializeComponent();

            int frameSize = Math.Min(pictureBox1.Width, pictureBox1.Height);

            fill = new Fill(colorChooserButton.BackColor, Resources.Files.wood, Resources.Files.DefaultNormalMap,
                            (double)kdNumeric.Value, (double)ksNumeric.Value, (double)kNumeric.Value, (int)mNumeric.Value, (int)sphereCutoffNumeric.Value, frameSize);

            sphereImage = new Bitmap(fill.frameSize, fill.frameSize);
            sphereImage.MakeTransparent();

            mesh = new Mesh((int)triangulationDegreeNumeric.Value);

            lightAnimation = new LightAnimation((double)zPlaneNumeric.Value, Color.White, () => { pictureBox1.Refresh(); });
            vertexRadius = 6.0f;

            grabbing = false;
            selectedVertex = -1;
        }
        private void colorChooserButton_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog
            {
                Color = colorChooserButton.BackColor
            };
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                colorChooserButton.BackColor = colorDialog.Color;

                fill.setTypeToColor(colorDialog.Color);

                if(!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void solidColorRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            colorChooserButton.Enabled = solidColorRadioButton.Checked;

            if(colorChooserButton.Enabled)
            {
                fill.setTypeToColor(colorChooserButton.BackColor);

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void chooseTextureButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Image Files(*.bmp;*.jpg;*.jpeg;*.gif;*.png)|*.bmp;*.jpg;*.jpeg;*.gif;*.png"
            };

            if (fileDialog.ShowDialog() == DialogResult.OK) {
                String textureSrc = fileDialog.FileName;
                fill.setTypeToBitmap(textureSrc);

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }

        }

        private void chooseTextureRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            chooseTextureButton.Enabled = chooseTextureRadioButton.Checked;

            if (chooseTextureButton.Enabled)
            {

                fill.setTypeToBitmap();

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (Graphics newGraphics = Graphics.FromImage(sphereImage))
            {
                newGraphics.Clear(Color.Transparent);
            }

            Painter.drawSphereOnBitmap(sphereImage, mesh, fill, lightAnimation.light, lightAnimation.camera);

            e.Graphics.DrawImage(sphereImage, (pictureBox1.Width - sphereImage.Width) / 2, (pictureBox1.Height - sphereImage.Height) / 2);

            if (showMeshCheckbox.Checked)
            {
                mesh.drawMesh(e.Graphics, pictureBox1.Width, pictureBox1.Height);
                mesh.drawVertices(e.Graphics, pictureBox1.Width, pictureBox1.Height, vertexRadius, selectedVertex);
            }
        }

        private void kNumeric_ValueChanged(object sender, EventArgs e)
        {
            kTrackBar.Value = (int)(kNumeric.Value * 100);

            if (fill.k != (double)kNumeric.Value)
            {
                fill.k = (double)kNumeric.Value;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void kTrackBar_Scroll(object sender, EventArgs e)
        {
            kNumeric.Value = ((decimal)kTrackBar.Value) / 100.0m;

            if(fill.k != (double)kNumeric.Value)
            {
                fill.k = (double)kNumeric.Value;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void fillExactRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fillExactRadioButton.Checked)
            {
                fill.mode = FillMode.Exact;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void fillInterpolateRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fillInterpolateRadioButton.Checked)
            {
                fill.mode = FillMode.Interpolate;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void lightColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog
            {
                Color = lightColorButton.BackColor
            };
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                lightColorButton.BackColor = colorDialog.Color;
                lightAnimation.light.color = lightColorButton.BackColor;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void ksTrackBar_Scroll(object sender, EventArgs e)
        {
            ksNumeric.Value = ((decimal)ksTrackBar.Value) / 100.0m;

            if(fill.ks != (double)ksNumeric.Value)
            {
                fill.ks = (double)ksNumeric.Value;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
            
        }

        private void ksNumeric_ValueChanged(object sender, EventArgs e)
        {
            ksTrackBar.Value = (int)(ksNumeric.Value * 100);

            if (fill.ks != (double)ksNumeric.Value)
            {
                fill.ks = (double)ksNumeric.Value;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void kdTrackBar_Scroll(object sender, EventArgs e)
        {
            kdNumeric.Value = ((decimal)kdTrackBar.Value) / 100.0m;

            if (fill.kd != (double)kdNumeric.Value)
            {
                fill.kd = (double)kdNumeric.Value;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void kdNumeric_ValueChanged(object sender, EventArgs e)
        {
            kdTrackBar.Value = (int)(kdNumeric.Value * 100);

            if (fill.kd != (double)kdNumeric.Value)
            {
                fill.kd = (double)kdNumeric.Value;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void mTrackBar_Scroll(object sender, EventArgs e)
        {
            mNumeric.Value = (decimal)mTrackBar.Value;

            if (fill.m != (int)mNumeric.Value)
            {
                fill.m = (int)mNumeric.Value;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void mNumeric_ValueChanged(object sender, EventArgs e)
        {
            mTrackBar.Value = (int)mNumeric.Value;

            if (fill.m != (int)mNumeric.Value)
            {
                fill.m = (int)mNumeric.Value;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void zPlaneNumeric_ValueChanged(object sender, EventArgs e)
        {
            zPlaneTrackBar.Value = (int)(zPlaneNumeric.Value * 10);

            if(lightAnimation.light.Z != (double)zPlaneNumeric.Value)
            {
                lightAnimation.light.Z = (double)zPlaneNumeric.Value;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }

        }

        private void zPlaneTrackBar_Scroll(object sender, EventArgs e)
        {
            zPlaneNumeric.Value = ((decimal)zPlaneTrackBar.Value) / 10.0m;

            if (lightAnimation.light.Z != (double)zPlaneNumeric.Value)
            {
                lightAnimation.light.Z = (double)zPlaneNumeric.Value;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }

        }

        private void triangulationDegreeNumeric_ValueChanged(object sender, EventArgs e)
        {
            int d = (int)triangulationDegreeNumeric.Value;

            if(triangulationDegreeTrackBar.Value != d)
            {
                triangulationDegreeTrackBar.Value = d;

                mesh = new Mesh(d);

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void triangulationDegreeTrackBar_Scroll(object sender, EventArgs e)
        {
            int d = triangulationDegreeTrackBar.Value;
            triangulationDegreeNumeric.Value = d;

            mesh = new Mesh(d);

            if (!lightAnimation.isRunning)
                pictureBox1.Refresh();
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            int frameSize = Math.Min(pictureBox1.Width, pictureBox1.Height);
            fill.setFrameSize(frameSize);

            sphereImage = new Bitmap(frameSize, frameSize);

            if (!lightAnimation.isRunning)
                pictureBox1.Refresh();
        }

        private void toggleAnimationButton_Click(object sender, EventArgs e)
        {
            if (lightAnimation.isRunning)
            {
                lightAnimation.Stop();
            } 
            else
            {
                lightAnimation.Start();
            }

            updateToggleAnimationButtonText(lightAnimation.isRunning);
        }

        private void resetAnimationButton_Click(object sender, EventArgs e)
        {
            lightAnimation.Reset();
            updateToggleAnimationButtonText(false);
            if (!lightAnimation.isRunning)
                pictureBox1.Refresh();
        }

        private void updateToggleAnimationButtonText(bool running)
        {
            toggleAnimationButton.Text = running ? "Stop" : "Start";
        }

        private void showMeshCheckbox_CheckedChanged(object sender, EventArgs e)
        {            
            if (!lightAnimation.isRunning)
                pictureBox1.Refresh();
        }


        private PointF TranslatePBPosToMeshPos(Mesh m, Point position, int frameSize)
        {
            float x = position.X;
            float y = position.Y;

            if (frameSize == pictureBox1.Width)
            {
                float d = (pictureBox1.Height - frameSize) / 2;
                y -= d;
            }
            else
            {
                float d = (pictureBox1.Width - frameSize) / 2;
                x -= d;
            }

            x = (float)((x * 2.0f / frameSize - 1.0f) / m.ratio);
            y = (float)((y * 2.0f / frameSize - 1.0f) / m.ratio);


            return new PointF(x, y);
        }

        private void updateVertexPosition(int v, Point mouseLocation)
        {
            PointF meshPos = TranslatePBPosToMeshPos(mesh, mouseLocation, fill.frameSize);
            float dxsqr = meshPos.X * meshPos.X;
            float dysqr = meshPos.Y * meshPos.Y;

            if (dxsqr + dysqr < 1)
            {
                Vector3 newVPos = new Vector3(meshPos.X, meshPos.Y, (float)Math.Sqrt(1 - dxsqr - dysqr));
                mesh.vertices[v] = newVPos;
            }

        }

        private void UpdateVertexHover(Point mouseLocation)
        {
            PointF meshPos = TranslatePBPosToMeshPos(mesh, mouseLocation, fill.frameSize);
            float R2 = (float)vertexRadius * 2.0f/ (float)(fill.frameSize);

            R2 *= R2;

            float minDistance = float.PositiveInfinity;
            int minSelectedVertex = -1;

            for (int i = 0; i < mesh.vertices.Count; i++)
            {
                float dx = meshPos.X - mesh.vertices[i].X;
                float dy = meshPos.Y - mesh.vertices[i].Y;
                float tempDistance = dx * dx + dy * dy;
                if (tempDistance < R2 && tempDistance < minDistance)
                {
                    minDistance = tempDistance;
                    minSelectedVertex = i;
                }
            }

            selectedVertex = minSelectedVertex;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!showMeshCheckbox.Checked) return;

            if (grabbing)
            {
                updateVertexPosition(selectedVertex, e.Location);

                if (!lightAnimation.isRunning)
                {
                    pictureBox1.Refresh();
                }
                return;
            }

            int lastVertex = selectedVertex;
            UpdateVertexHover(e.Location);

            if(selectedVertex != lastVertex && !lightAnimation.isRunning)
            {
                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(selectedVertex != -1)
            {
                grabbing = true;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            grabbing = false;
        }

        private void chooseNormalMapButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Image Files(*.bmp;*.jpg;*.jpeg;*.gif;*.png)|*.bmp;*.jpg;*.jpeg;*.gif;*.png"
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string normalMapSrc = fileDialog.FileName;
                fill.setNormalMap(new Bitmap(normalMapSrc));

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void movingVCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (movingVCheckbox.Checked)
            {
                fill.vmode = VMode.Moving;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void standardVCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (standardVCheckbox.Checked)
            {
                fill.vmode = VMode.Standard;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }

        private void sphereCutoffTrackbar_Scroll(object sender, EventArgs e)
        {

            sphereCutoffNumeric.Value = ((decimal)sphereCutoffTrackbar.Value) / 100.0m;

            if (fill.h != (double)sphereCutoffNumeric.Value)
            {
                fill.h = (double)sphereCutoffNumeric.Value;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }

        }

        private void sphereCutoffNumeric_ValueChanged(object sender, EventArgs e)
        {
            sphereCutoffTrackbar.Value = (int)(sphereCutoffNumeric.Value * 100);

            if (fill.h != (double)sphereCutoffNumeric.Value)
            {
                fill.h = (double)sphereCutoffNumeric.Value;

                if (!lightAnimation.isRunning)
                    pictureBox1.Refresh();
            }
        }
    }
}
