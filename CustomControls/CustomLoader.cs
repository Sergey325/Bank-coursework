using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Курсовая.CustomControls
{
    public partial class CustomLoader : UserControl
    {
        private float progress = 40;
        private float angle = 260;
        private int speed = 7;
        private int thickness = 5;
        private Color color = Color.Magenta;
        private Timer timer;

        public CustomLoader()
        {
            AutoSize = false;
            Size = new Size(40, 40);
            BackColor = Color.Transparent;
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint, value: true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, value: true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
            timer = new Timer
            {
                Enabled = true,
                Interval = 1
            };
            timer.Tick += (s, a) =>
            {
                Refresh();
            };
        }

        [Category("Style")]
        public Color Color
        {
            get => color;
            set 
            { 
                color = value; 
                Invalidate(); 
            }
        }

        [Category("Style")]
        public int Thickness
        {
            get => thickness;
            set 
            { 
                thickness = value; 
                Invalidate(); 
            }
        }

        [Category("Style")]
        public float Angle
        {
            get => angle;
            set
            {
                angle = value;
                Invalidate();
            }
        }

        [Category("Behaviour")]
        public int Speed
        {
            get => speed;
            set 
            { 
                speed = value; 
                Invalidate(); 
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graph = e.Graphics;
            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graph.CompositingMode = CompositingMode.SourceCopy;
            graph.SmoothingMode = SmoothingMode.HighQuality;
            progress += Speed;
            using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color, Color.Transparent, progress))
            using (Pen pen = new Pen(linearGradientBrush, Thickness))
            {

                //pen.DashStyle = DashStyle.Dot;
                //float[] dashPattern = new float[Width];
                //for (int i = 0; i < Width; i++)
                //{
                //    dashPattern[i] = 0.5f;
                //}
                //pen.DashStyle = DashStyle.Dash;
                //pen.DashPattern = dashPattern;
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                graph.DrawArc(pen, Thickness / 2, Thickness / 2, Width - Thickness - 1, Height - Thickness - 1, progress, Angle);
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(Width, Width);
        }
    }
}