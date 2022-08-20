using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Курсовая.CustomControls
{

    public partial class LabelEx : Label
    {
        public enum GradientMode
        {
            vertical = 0,
            horizontal = 1,
        }

        private Color color = Color.RoyalBlue;
        private Color color2 = Color.HotPink;
        private float gradientAngle = 0f;
        public LabelEx()
        {
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //using (StringFormat format = CreateStringFormat())
            //{   
            //    using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color, Color2,
            //    Gradientmode == 0 ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal))
            //    {
            //        Font font = new Font(Font.Name, Font.Size, Font.Style);
            //        e.Graphics.DrawString(Text, font, brush, 0, 0, format);
            //    }
            //}
            Font font = new Font(Font.Name, Font.Size, Font.Style);
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color, Color2, gradientAngle);
            //StringFormat sf = new StringFormat();
            //sf.Alignment = StringAlignment.Far;
            e.Graphics.DrawString(Text, font, brush, 0, 0);
        }

        [Category("Style")]
        public float GradientAngle
        {
            get { return gradientAngle; }
            set
            {
                gradientAngle = value;
                this.Invalidate();
            }
        }

        [Category("Style")]
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                this.Invalidate();
            }
        }
        [Category("Style")]
        public Color Color2
        {
            get { return color2; }
            set
            {
                color2 = value;
                this.Invalidate();
            }
        }
        
        [Category("CatAppearance")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("LabelBackgroundImageDescr")]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
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
