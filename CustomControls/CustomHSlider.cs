using Bunifu.UI.WinForms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace Курсовая.CustomControls
{
    class CustomHSlider : BunifuSeparator
    {

        private int borderThickness = 2;
        private Color borderColor = Color.DodgerBlue;
        private Color fillColor = Color.DodgerBlue;
        private Size circleSize = new Size(15, 15);
        private int count = 4;
        public Action elementChanged;

        public CustomHSlider()
        {
            LineThickness = 2;
            this.Size = new Size(Width, Height + circleSize.Height + 2);
            BuildCircles();
        }

        [Category("Behaviour")]
        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                Rebuild();
            }
        }

        [Category("Circles style")]
        public int BorderSize
        {
            get { return borderThickness; }
            set
            {
                borderThickness = value;
                Rebuild();
            }
        }

        [Category("Circles style")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                Rebuild();
            }
        }

        [Category("Circles style")]
        public Color FillColor
        {
            get { return fillColor; }
            set
            {
                fillColor = value;
                Rebuild();
            }
        }

        [Category("Circles style")]
        public Size CirclesSize
        {
            get { return circleSize; }
            set
            {
                circleSize = value;
                Rebuild();
            }
        }

        private void BuildCircles()
        {
            Controls.Clear();
            var y = (Height - CirclesSize.Height) / 2;
            var indent = (double)(Width - CirclesSize.Width * count) / (count - 1);
            for (int i = 0; i < Count; i++)
            {
                var circle = new BunifuShapes
                {
                    BorderColor = borderColor,
                    BorderThickness = borderThickness,
                    FillColor = Color.FromArgb(34, 31, 46),
                    FillShape = true,
                    Location = new Point((int)(indent * i + CirclesSize.Width * i), y),
                    Name = $"{i + 1}",
                    Shape = BunifuShapes.Shapes.Circle,
                    Sides = 5,
                    Size = CirclesSize,
                    TabIndex = i,
                };
                circle.Click += (s, a) =>
                {

                    if (circle.FillColor != fillColor)
                    {
                        Controls.OfType<BunifuShapes>().Take(int.Parse(circle.Name)).ToList().ForEach(x => x.FillColor = fillColor);
                    }
                    else
                    {
                        Controls.OfType<BunifuShapes>().Skip(int.Parse(circle.Name)).ToList().ForEach(x => x.FillColor = Parent.BackColor);
                    }
                    elementChanged?.Invoke();
                    ///
                };
                Controls.Add(circle);
            }
        }
        public void Rebuild()
        {
            BuildCircles();
            this.Invalidate();

        }
        protected override void OnResize(EventArgs e)
        {
            BuildCircles();
            base.OnResize(e);
        }
        public int CurrentElement => Controls.OfType<BunifuShapes>().Any(x => x.FillColor == fillColor) ? int.Parse(Controls.OfType<BunifuShapes>().Last(x => x.FillColor == fillColor).Name) : 0;
        
        private void InitializeComponent()
        {
            ((ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            ((ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
