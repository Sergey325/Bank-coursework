using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Курсовая.CustomControls
{
    class CustomPanel : PictureBox
    {
        private int borderSize = 0;
        private float dashLen = 2;
        private float dashIndent = 2;
        private Color borderColor = Color.RoyalBlue;
        private Color borderColor2 = Color.Magenta;
        private DashStyle borderLineStyle = DashStyle.Solid;
        private DashCap borderCapStyle = DashCap.Flat;
        private float gradientAngle = 50F;
        private bool fillingBack = true;

        //public void Active()//Это нужно было в предыдущем проекте
        //{
        //    BorderColor = Color.RoyalBlue;
        //    BorderColor2 = Color.Magenta;
        //    this.Invalidate();
        //}
        //public void Inactive()
        //{
        //    BorderColor = Color.Gray;
        //    BorderColor2 = Color.FromArgb(64, 64, 64);
        //    this.Invalidate();
        //}
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
        [Category("Style")]
        public bool FillingBack
        {
            get { return fillingBack; }
            set
            {
                fillingBack = value;
                this.Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
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
                if(borderLineStyle == DashStyle.Dash) penBorder.DashPattern = new float[] { dashLen, dashIndent };
                pathRegion.AddRectangle(rectContourSmooth);
                this.Region = new Region(pathRegion);
                graph.DrawRectangle(penSmooth, rectContourSmooth);
                if (borderSize > 0)
                {
                    graph.DrawRectangle(penBorder, rectBorder);
                }
                if (fillingBack) graph.FillPath(borderGColor, pathRegion);
            }
        }
    }
}
