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
    class RoundedPanel : Panel
    {
        private int borderSize = 2;
        private Color borderColor = Color.Silver;
        private int borderRadius = 10;
        private Color fillColor = Color.Silver;
        private Color fillColor2 = Color.Silver;


        public RoundedPanel()
        {

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
        public int BorderRadius
        {
            get { return borderRadius; }
            set
            {
                borderRadius = value;
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
        [Browsable(false)]
        public override Color BackColor
        {
            get { return Color.Transparent; }
        }

        [Category("Style")]
        public Color FillColor
        {
            get { return fillColor; }
            set
            {
                if(fillColor2 == fillColor)fillColor2 = value;
                fillColor = value;
                this.Invalidate();
            }
        }

        [Category("Style")]
        public Color FillColor2
        {
            get { return fillColor2; }
            set
            {
                fillColor2 = value;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graph = e.Graphics;
            var pen = new Pen(BorderColor, borderSize);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            graph.SmoothingMode = SmoothingMode.AntiAlias;
            var graphPath = new GraphicsPath();
            var newRegion = new GraphicsPath();

            var bounds = new Rectangle(borderSize, borderSize, Width - borderSize * 2 - 1, Height - borderSize * 2 - 1);
            if (borderRadius > 1 && borderSize > 0)
            {
                graphPath.AddArc(bounds.X, bounds.Y, borderRadius, borderRadius, 180, 90);
                graphPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y, borderRadius, borderRadius, 270, 90);
                graphPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y + bounds.Height - borderRadius, borderRadius, borderRadius, 0, 90);
                graphPath.AddArc(bounds.X, bounds.Y + bounds.Height - borderRadius, borderRadius, borderRadius, 90, 90);
                graphPath.CloseAllFigures();

                newRegion.AddArc(bounds.X - borderSize, bounds.Y - borderSize, borderRadius + borderSize * 2, borderRadius + borderSize * 2, 180, 90);
                newRegion.AddArc(bounds.X - borderSize + bounds.Width - borderRadius, bounds.Y - borderSize, borderRadius + borderSize * 2, borderRadius + borderSize * 2, 270, 90);
                newRegion.AddArc(bounds.X - borderSize + bounds.Width - borderRadius, bounds.Y - borderSize + bounds.Height - borderRadius, borderRadius + borderSize * 2, borderRadius + borderSize * 2, 0, 90);
                newRegion.AddArc(bounds.X - borderSize, bounds.Y - borderSize + bounds.Height - borderRadius, borderRadius + borderSize * 2, borderRadius + borderSize * 2, 90, 90);
                newRegion.CloseAllFigures();
                Region = new Region(newRegion);
            }
            else
            {
                graphPath.AddRectangle(new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height));
            }
            var rectContourSmooth = Rectangle.Inflate(this.ClientRectangle, -1, -1);
            var rectBorder = Rectangle.Inflate(rectContourSmooth, -borderSize, -borderSize);
            var fillGColor = new LinearGradientBrush(rectBorder, fillColor, fillColor2, 50f);

            if(fillColor != Color.Transparent)graph.FillPath(fillGColor, graphPath);
            if (borderSize > 0) graph.DrawPath(pen, graphPath);

            graphPath.Dispose();
            newRegion.Dispose();
            pen.Dispose();
            fillGColor.Dispose();
        }
    }
}
