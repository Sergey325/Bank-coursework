using Bunifu.UI.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Курсовая.CustomControls
{
    public partial class DropDownMenu : UserControl
    {
        private bool minimized = true;
        private int indent = 2;
        private int scaleMouseEnter = 3;
        private Dictionary<string, Image> imageDict = new Dictionary<string, Image>();
        private Color backColorImages = Color.Black;
        private Image expandImage;
        private Image collapseImage;
        public Action elementChanged;
        public bool Minimized 
        {
            get => minimized; 
        }

        public DropDownMenu()
        {
            InitializeComponent();
            pbDropDown.Click += (s, a) =>
            {
                if (minimized)Expand();
                else Collapse();
            };
        }

        [Category("Border style")]
        public override Color BackColor
        {
            get { return panel.BackgroundColor; }
            set
            {
                panel.BackgroundColor = value;
                this.Invalidate();
            }
        }

        [Category("Border style")]
        public Color BorderColor
        {
            get { return panel.BorderColor; }
            set
            {
                panel.BorderColor = value;
                this.Invalidate();
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
        public int BorderThickness
        {
            get { return panel.BorderThickness; }
            set
            {
                panel.BorderThickness = value;
                this.Invalidate();
            }
        }

        [Category("Style")]
        public Image ExpandImage
        {
            get { return expandImage; }
            set
            {
                expandImage = value;
                if (pbDropDown.BackgroundImage == null) pbDropDown.BackgroundImage = value;
                this.Invalidate();
            }
        }
        [Category("Style")]
        public Image CollapseImage
        {
            get { return collapseImage; }
            set
            {
                collapseImage = value;
                this.Invalidate();
            }
        }

        [Category("Style")]
        public int Indent
        {
            get { return indent; }
            set
            {
                indent = value;
                this.Invalidate();
            }
        }

        [Category("Style")]
        public int ScaleMouseEnter
        {
            get { return scaleMouseEnter; }
            set
            {
                scaleMouseEnter = value;
                this.Invalidate();
            }
        }

        [Category("Style")]
        public Color BackColorImages
        {
            get { return backColorImages; }
            set
            {
                backColorImages = value;
                this.Invalidate();
            }
        }

        public void AddImage(Image image, string imageName)
        {
            imageDict.Add(imageName, image);
            RebuildLabels();
        }
        public void AddImageRange(Dictionary<string, Image> dict)
        {
            imageDict = imageDict.Concat(dict).ToDictionary(x => x.Key, x => x.Value);
            RebuildLabels();
        }
        public void ClearImages()
        {
            imageDict.Clear();
            RebuildLabels();
        }
        public void RemoveImage(string imageName)
        {
            imageDict.Remove(imageName);
            RebuildLabels();
        }
        public void RebuildLabels()
        {
            var c = pbDropDown;
            panel.Controls.Clear();
            panel.Controls.Add(c);

            for (int i = 0; i < imageDict.Count; i++)
            {
                var label = new BunifuLabel
                {
                    Name = $"{i+1}",
                    AutoSize = false,
                    BackColor = Color.Transparent,
                    BackgroundImage = imageDict.ElementAt(i).Value,
                    BackgroundImageLayout = ImageLayout.Stretch,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Size = new Size(Width - 30, Width - 30),
                    Location = new Point(2 + indent, Indent + (i == 0 ? 0 : 1) + (Width-30)  * i),
                    Tag = imageDict.ElementAt(i).Key
                };
                label.MouseEnter += (s, a) =>
                {
                    if(!minimized && label.Location.Y != Indent) label.Size = new Size(label.Size.Width + scaleMouseEnter, label.Size.Height + scaleMouseEnter);
                };
                label.MouseLeave += (s, a) =>
                {
                    if (!minimized && label.Location.Y != Indent) label.Size = new Size(label.Size.Width - scaleMouseEnter, label.Size.Height - scaleMouseEnter);
                };
                label.Click += (s, a) =>
                {
                    if (label.Location.Y != Indent)
                    {
                        panel.Controls["1"].Location = label.Location;
                        panel.Controls["1"].Name = label.Name;
                        label.Location = new Point(2 + indent, indent);
                        label.Size = new Size(Width - 30, Width - 30);
                        label.Name = "1";
                        Collapse();
                        elementChanged?.Invoke();
                        this.Invalidate();
                    }
                };
                panel.Controls.Add(label);
            }
            this.Invalidate();
        }
        public async void Collapse()
        {
            minimized = true;
            pbDropDown.BackgroundImage = expandImage;
            while (Height >= panel.Controls[0].Height + indent * 2)
            {
                Height -= 8;
                await Task.Delay(1);
            }
        }
        public async void Expand()
        {
            minimized = false;
            pbDropDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbDropDown.BackgroundImage = collapseImage;
            foreach (Control ctrl in panel.Controls.OfType<BunifuLabel>()) ctrl.BackColor = backColorImages;
            var control = panel.Controls[imageDict.Count.ToString()];
            while (Height < control.Location.Y + control.Height + Indent)
            {
                Height += 8;
                await Task.Delay(1);
            }
            foreach (Control ctrl in panel.Controls.OfType<BunifuLabel>()) ctrl.BackColor = Color.Transparent;
        }
        public string CurrentElementName => panel.Controls["1"].Tag.ToString();
        public Image CurrentElementImage => imageDict[panel.Controls["1"].Tag.ToString()];
    }
}
