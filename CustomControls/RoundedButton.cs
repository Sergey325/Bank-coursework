using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Курсовая.CustomControls
{
    class RoundedButton : UserControl
    {
        private int borderSize = 2;
        private Color borderColor = Color.Silver;
        private int borderRadius = 10;
        private Label label;
        private Color fillColor;

        public RoundedButton()
        {
            BackColor = Color.Transparent;
            AutoSize = false;
            Size = new Size(159, 36);
            label = new Label
            {
                AutoSize = false,
                Text = "RoundedButton",
                ForeColor = Color.Silver,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(familyName: "Leelawadee UI", emSize: 10)
            };
            ///Нужно прикрепить события контрола к событиям лейбла
            label.MouseEnter += (s, e) => OnMouseEnter(e);
            label.MouseLeave += (s, e) => OnMouseLeave(e);
            label.Click += (s, e) => OnClick(e);
            
          Controls.Add(label);
        }

        [Browsable(false)]
        public override Color BackColor
        {
            get { return Color.Transparent; }
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
        [Category("TextStyle")]
        public override Font Font
        {
            get { return label.Font; }
            set
            {
                label.Font = value;
                this.Invalidate();
            }
        }
        [Category("TextStyle")]
        public override Color ForeColor
        {
            get { return label.ForeColor; }
            set
            {
                label.ForeColor = value;
                this.Invalidate();
            }
        }
        [Category("Style")]
        public Color FillColor
        {
            get { return fillColor; }
            set
            {
                fillColor = value;
                this.Invalidate();
            }
        }
        public string text
        {
            get { return label.Text; }
            set
            {
                label.Text = value;
                this.Invalidate();
            }
        }
        [Category("TextStyle")]
        public ContentAlignment TextAlign
        {
            get { return label.TextAlign; }
            set
            {
                label.TextAlign = value;
                this.Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            label.Size = new Size(Size.Width - borderSize * 2, Size.Height - borderSize * 2);
            label.Location = new Point(borderSize, borderSize);
            var graph = pe.Graphics;
            var pen = new Pen(BorderColor, borderSize);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            graph.SmoothingMode = SmoothingMode.AntiAlias;
            var graphPath = new GraphicsPath();
            var newRegion = new GraphicsPath();

            var bounds = new Rectangle(borderSize, borderSize, Width - borderSize * 2 - 1, Height - borderSize * 2 - 1);
            if (borderRadius > 1  && borderSize > 0)
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
            //можешь добавить свойства fillColor2 и gradientAngle(float) для градиентной заливки
            var fillGColor = new LinearGradientBrush(rectBorder, fillColor, fillColor, 50f);
            graph.FillPath(fillGColor, graphPath);
            if(borderSize > 0) graph.DrawPath(pen, graphPath);

            graphPath.Dispose();
            newRegion.Dispose();
            pen.Dispose();
            fillGColor.Dispose();
        }
    }
}
