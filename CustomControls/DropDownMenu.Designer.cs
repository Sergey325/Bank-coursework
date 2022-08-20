namespace Курсовая.CustomControls
{
    partial class DropDownMenu
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DropDownMenu));
            this.panel = new Bunifu.UI.WinForms.BunifuPanel();
            this.pbDropDown = new System.Windows.Forms.PictureBox();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDropDown)).BeginInit();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.BackgroundColor = System.Drawing.Color.Transparent;
            this.panel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel.BackgroundImage")));
            this.panel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel.BorderColor = System.Drawing.Color.Silver;
            this.panel.BorderRadius = 5;
            this.panel.BorderThickness = 1;
            this.panel.Controls.Add(this.pbDropDown);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.ShowBorders = true;
            this.panel.Size = new System.Drawing.Size(64, 39);
            this.panel.TabIndex = 5;
            // 
            // pbDropDown
            // 
            this.pbDropDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbDropDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbDropDown.Location = new System.Drawing.Point(39, 0);
            this.pbDropDown.Name = "pbDropDown";
            this.pbDropDown.Size = new System.Drawing.Size(17, 36);
            this.pbDropDown.TabIndex = 4;
            this.pbDropDown.TabStop = false;
            // 
            // DropDownMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel);
            this.Name = "DropDownMenu";
            this.Size = new System.Drawing.Size(64, 39);
            this.panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbDropDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.PictureBox pbDropDown;
        public Bunifu.UI.WinForms.BunifuPanel panel;
    }
}
