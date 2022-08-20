using Bunifu.UI.WinForms;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Курсовая.CustomControls
{
    class MovableLabel : BunifuLabel
    {
        private bool statik = false;
        private bool movable = false;
        private ushort indent = 0;
        private Control client;
        private Point p;
        //private Point initialPosition;
        public bool Statik
        {
            get { return statik; }
            set { statik = value; }
        }


        public MovableLabel()
        {
            AutoSize = false;
            Text = "";
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
        [ParenthesizePropertyName(true)]//Привязка любого контрола
        public Control Client
        {
            get { return client; }
            set
            {
                client = value;
                this.Invalidate();
            }
        }
        [Category("Поведение")]
        [ParenthesizePropertyName(true)]//Дроп давн булевого значения
        public bool TTTT
        {
            get;
            set;
        }
        //public Point InitialPosition { get; private set; }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!statik)
            {
                movable = true;
                Visible = false;
                p = e.Location;
                //InitialPosition = Location;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!statik)
            {
                var t = Location;
                if (!Visible) Visible = true;
                if (Client != null && e.Button == MouseButtons.Left && movable)
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
