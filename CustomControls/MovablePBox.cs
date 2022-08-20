using Bunifu.UI.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Курсовая.CustomControls
{
    class MovablePBox:BunifuPictureBox
    {
        private bool movable = true;
        private ushort indent = 0;
        private float dashLen = 2;
        private float dashIndent = 2;
        private Control client;
        private Point p;
        private int borderSize = 2;
        private Color borderColor = Color.RoyalBlue;
        private Color borderColor2 = Color.HotPink;
        private DashStyle borderLineStyle = DashStyle.Solid;
        private DashCap borderCapStyle = DashCap.Flat;
        private float gradientAngle = 50F;

        public MovablePBox()
        {
            Type = Types.Custom;
        }

        [Category("Border style")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                borderSize = value;
                this.Invalidate();
            }
        }
        [Category("Border style")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                this.Invalidate();
            }
        }
        [Category("Border style")]
        public Color BorderColor2
        {
            get { return borderColor2; }
            set
            {
                borderColor2 = value;
                this.Invalidate();
            }
        }
        [Category("Border style")]
        public DashStyle BorderLineStyle
        {
            get { return borderLineStyle; }
            set
            {
                borderLineStyle = value;
                this.Invalidate();
            }
        }
        [Category("Border style")]
        public DashCap BorderCapStyle
        {
            get { return borderCapStyle; }
            set
            {
                borderCapStyle = value;
                this.Invalidate();
            }
        }
        [Category("Border style")]
        public float GradientAngle
        {
            get { return gradientAngle; }
            set
            {
                gradientAngle = value;
                this.Invalidate();
            }
        }
        [Category("Border style")]
        public float DashLen
        {
            get { return dashLen; }
            set
            {
                dashLen = value;
                this.Invalidate();
            }
        }
        [Category("Border style")]
        public float DashIndent
        {
            get { return dashIndent; }
            set
            {
                dashIndent = value;
                this.Invalidate();
            }
        }
        [Category("Поведение")]
        public ushort Indent
        {
            get { return indent; }
            set
            {
                indent = value;
                this.Invalidate();
            }
        }
        [Category("Поведение")]
        [ParenthesizePropertyName(true)]
        public Control Client
        {
            get { return client; }
            set
            {
                client = value;
                this.Invalidate();
            }
        }
        public Point InitialPosition { get; private set; }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            movable = true;
            Visible = false;
            p = e.Location;
            InitialPosition = Location;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var t = Location;
            Visible = true;
            if (Client != null) 
            { 
                if (e.Button == MouseButtons.Left && movable)
                {
                    Top += e.Y - p.Y;
                    Left += e.X - p.X;
                }
                if (Location.X < Indent || Location.Y < Indent ||
                    Location.X > (Client.Size.Width - Size.Width) - Indent ||
                    Location.Y > (Client.Size.Height - Size.Height) - Indent)
                {
                    movable = false;
                    Location = t;
                }
            }
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            //Fields
            var graph = pe.Graphics;
            var rectContourSmooth = Rectangle.Inflate(this.ClientRectangle, -1, -1);
            var rectBorder = Rectangle.Inflate(rectContourSmooth, -borderSize, -borderSize);
            var smoothSize = borderSize > 0 ? borderSize * 3 : 1;
            using (var borderGColor = new LinearGradientBrush(rectBorder, borderColor, borderColor2, gradientAngle))
            using (var pathRegion = new GraphicsPath())
            using (var penSmooth = new Pen(this.Parent.BackColor, smoothSize))
            using (var penBorder = new Pen(borderGColor, borderSize))
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                penBorder.DashStyle = borderLineStyle;
                penBorder.DashCap = borderCapStyle;

                if (Type == Types.Circle) pathRegion.AddEllipse(rectContourSmooth);
                else
                {
                    penBorder.DashPattern = new float[] { dashLen, dashIndent };
                    pathRegion.AddRectangle(rectContourSmooth);
                }

                this.Region = new Region(pathRegion);
                if(Type == Types.Circle)
                {
                    graph.DrawEllipse(penSmooth, rectContourSmooth);
                    if (borderSize > 0) graph.DrawEllipse(penBorder, rectBorder);
                }
                else
                {
                    graph.DrawRectangle(penSmooth, rectContourSmooth);
                    if (borderSize > 0) graph.DrawRectangle(penBorder, rectBorder);
                }
                
            }
        }
        private void InitializeComponent()
        {
            ((ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            ((ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
