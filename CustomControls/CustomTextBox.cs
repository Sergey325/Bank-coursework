using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Курсовая.CustomControls
{
    public partial class CustomTextBox : UserControl
    {
        private Color placeholderForeColor = Color.FromArgb(87, 83, 103);
        private string placeholderText = "Enter Text";
        private Color borderColorHover = Color.FromArgb(137, 16, 221);
        private Color borderColorIdle = Color.FromArgb(23, 21, 35);
        private Color foreClr = Color.FromArgb(137, 16, 221);
        private string txt = "";

        public CustomTextBox()
        {
            InitializeComponent();
            maskedTextBox.ForeColor = placeholderForeColor;
        }

        protected override void OnLoad(EventArgs a)
        {
            base.OnLoad(a);
            panel.Click += (s, e) => OnClick(e);
            pictureBox.Click += (s, e) => OnClick(e);
            maskedTextBox.Click += (s, e) => OnClick(e);
            MouseEnter += (s, e) => { panel.BorderColor = borderColorHover; };
            MouseLeave += (s, e) => { panel.BorderColor = borderColorIdle; };
            Click += (s, e) =>
            {
                maskedTextBox.Focus();
                if (txt == "") maskedTextBox.SelectionStart = 0;
            };
            maskedTextBox.MouseEnter += (s, e) => OnMouseEnter(e);
            maskedTextBox.MouseLeave += (s, e) => OnMouseLeave(e);
            panel.MouseEnter += (s, e) => OnMouseEnter(e);
            panel.MouseLeave += (s, e) => OnMouseLeave(e);
            pictureBox.MouseEnter += (s, e) => OnMouseEnter(e);
            pictureBox.MouseLeave += (s, e) => OnMouseLeave(e);
            maskedTextBox.TextChanged += (s, e) =>
            {
                if (maskedTextBox.Text.Length == 0)
                {
                    maskedTextBox.ForeColor = placeholderForeColor;
                    maskedTextBox.Text = placeholderText;
                    maskedTextBox.SelectionStart = 0;
                    text = "";
                }
                else if (maskedTextBox.Text != placeholderText && maskedTextBox.Mask != "")
                {
                    maskedTextBox.ForeColor = foreClr;
                    maskedTextBox.Text = maskedTextBox.Text;
                    maskedTextBox.SelectionStart = 0;
                }
                else if (maskedTextBox.Text.Length == placeholderText.Length + 1 && maskedTextBox.Text.EndsWith(placeholderText))
                {
                    maskedTextBox.ForeColor = foreClr;
                    maskedTextBox.Text = maskedTextBox.Text.Replace(placeholderText, "");
                    maskedTextBox.SelectionStart = 1;
                }
                if (maskedTextBox.Text == placeholderText) { }
                else
                {
                    text = maskedTextBox.Text;
                }
            };
            maskedTextBox.KeyDown += (s, e) =>
            {
                if (maskedTextBox.Text.EndsWith(placeholderText) && ForeColor == placeholderForeColor)
                {
                    e.Handled = e.KeyData == Keys.Right || e.KeyData == Keys.Down || maskedTextBox.SelectionLength > 0 || e.KeyData == Keys.Delete;
                }
            };
            maskedTextBox.KeyPress += (s, e) =>
            {
                if (maskedTextBox.Text.EndsWith(placeholderText) && ForeColor == placeholderForeColor)
                {
                    e.Handled = e.KeyChar == (char)Keys.Back || maskedTextBox.SelectionLength > 0;
                }
            };
        }

        public event EventHandler TextChange
        {
            add
            {
                maskedTextBox.TextChanged += value;
            }
            remove
            {
                maskedTextBox.TextChanged -= value;
            }
        }

        [Category("Border style")]
        public int BorderRadius
        {
            get { return panel.BorderRadius; }
            set
            {
                panel.BorderRadius = value;
                this.Invalidate();
            }
        }

        [Category("Border style")]
        public Color BorderColor
        {
            get { return panel.BorderColor; }
            set
            {
                borderColorIdle = value;
                panel.BorderColor = value;
                this.Invalidate();
            }
        }
        [Category("Border style")]
        public Color BorderColorIdle
        {
            get { return borderColorIdle; }
            set
            {
                borderColorIdle = value;
                panel.BorderColor = value;
                this.Invalidate();
            }
        }
        [Category("Border style")]
        public Color BorderColorHover
        {
            get { return borderColorHover; }
            set
            {
                borderColorHover = value;
                this.Invalidate();
            }
        }

        [Category("Border style")]
        public int BorderThickness
        {
            get { return panel.BorderSize; }
            set
            {
                panel.BorderSize = value;
                this.Invalidate();
            }
        }

        [Category("Style")]
        public Color FillColor
        {
            get { return maskedTextBox.BackColor; }
            set
            {
                panel.FillColor = value;
                maskedTextBox.BackColor = value;
                this.Invalidate();
            }
        }

        [Category("TextStyle")]
        public HorizontalAlignment TextAlign
        {
            get { return maskedTextBox.TextAlign; }
            set
            {
                maskedTextBox.TextAlign = value;
                this.Invalidate();
            }
        }

        [Category("TextStyle")]
        public Color PlaceholderForeColor
        {
            get { return placeholderForeColor; }
            set
            {
                placeholderForeColor = value;
                if (txt == "") maskedTextBox.ForeColor = placeholderForeColor;
                this.Invalidate();
            }
        }

        [Category("TextStyle")]
        public string PlaceholderText
        {
            get { return placeholderText; }
            set
            {
                placeholderText = value;
                if (txt == "") maskedTextBox.Text = value;
                this.Invalidate();
            }
        }
        [Category("TextStyle")]
        public override Font Font
        {
            get { return maskedTextBox.Font; }
            set
            {
                maskedTextBox.Font = value;
                this.Invalidate();
            }
        }

        [Category("TextStyle")]
        public char PasswordChar
        {
            get { return maskedTextBox.PasswordChar; }
            set
            {
                maskedTextBox.PasswordChar = value;
                this.Invalidate();
            }
        }

        [Category("TextStyle")]
        public char PromptChar
        {
            get { return maskedTextBox.PromptChar; }
            set
            {
                maskedTextBox.PromptChar = value;
                this.Invalidate();
            }
        }

        [Category("TextStyle")]
        //Вероятно, это окно возвращает нал, так что маску менять или в коде или писать строку (не открывая окно)
        [Editor("System.Windows.Forms.Design.MaskPropertyEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string Mask
        {
            get { return maskedTextBox.Mask; }
            set
            {
                maskedTextBox.Mask = value;
                this.Invalidate();
            }
        }

        [Category("Style")]
        public Image LeftIcon
        {
            get { return pictureBox.BackgroundImage; }
            set
            {
                if (value != null) AddImage(value);
                else RemoveImage();
                this.Invalidate();
            }
        }

        [Category("Style")]
        public ImageLayout LeftIconLayout
        {
            get { return pictureBox.BackgroundImageLayout; }
            set
            {
                pictureBox.BackgroundImageLayout = value;
                this.Invalidate();
            }
        }

        [Category("TextStyle")]
        public Color foreColor
        {
            get { return foreClr; }
            set
            {
                foreClr = value;
                this.Invalidate();
            }
        }

        public string text
        {
            get { return txt; }
            set
            {
                txt = value;
                if (txt == "")
                {
                    maskedTextBox.ForeColor = placeholderForeColor;
                    maskedTextBox.Text = placeholderText;
                }
                else
                {
                    maskedTextBox.ForeColor = foreClr;
                }
                maskedTextBox.Text = txt == "" ? placeholderText : txt;

                this.Invalidate();
            }
        }

        [Obsolete]
        public override string Text
        {
            get { return maskedTextBox.Text; }
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Color ForeColor
        {
            get { return Color.Black; }
        }

        private void AddImage(Image image)
        {
            if (pictureBox.BackgroundImage == null)
            {
                pictureBox.BackgroundImage = image;
                pictureBox.Size = new Size(Height - 10, Height - 10);
                maskedTextBox.Location += new Size(Height - 10, 0);
                maskedTextBox.Size -= new Size(Height - 10, 0);
                pictureBox.Visible = true;
            }
            else
            {
                pictureBox.BackgroundImage = image;
            }
        }
        private void RemoveImage()
        {
            pictureBox.Image = null;
            pictureBox.Location = new Point(5, 5);
            pictureBox.Size = new Size(0, 0);
            pictureBox.Visible = false;
            maskedTextBox.Location = new Point(8, 11);
            maskedTextBox.Width = Width - 17;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            pictureBox.Size = new Size(Height - 10, Height - 10);
            if (pictureBox.BackgroundImage != null)
            {
                maskedTextBox.Location = new Point(8 + pictureBox.Width, maskedTextBox.Location.Y);
                maskedTextBox.Width = Width - (5 + pictureBox.Width + 16);
            }
            else
            {
                maskedTextBox.Location = new Point(maskedTextBox.Location.X, (Height - maskedTextBox.Height) / 2);
                maskedTextBox.Width = Width - 17;
            }

        }
    }
}