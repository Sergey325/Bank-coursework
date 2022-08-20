namespace Курсовая.CustomControls
{
    partial class CustomTextBox
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
            this.panel = new Курсовая.CustomControls.RoundedPanel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(21)))), ((int)(((byte)(35)))));
            this.panel.BorderRadius = 10;
            this.panel.BorderSize = 2;
            this.panel.Controls.Add(this.pictureBox);
            this.panel.Controls.Add(this.maskedTextBox);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(21)))), ((int)(((byte)(35)))));
            this.panel.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(21)))), ((int)(((byte)(35)))));
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(263, 48);
            this.panel.TabIndex = 2;
            // 
            // pictureBox
            // 
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.pictureBox.Location = new System.Drawing.Point(5, 5);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.pictureBox.Size = new System.Drawing.Size(0, 0);
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            this.pictureBox.Visible = false;
            // 
            // maskedTextBox
            // 
            this.maskedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.maskedTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(21)))), ((int)(((byte)(35)))));
            this.maskedTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.maskedTextBox.Font = new System.Drawing.Font("Nirmala UI", 14.25F);
            this.maskedTextBox.ForeColor = System.Drawing.Color.Gray;
            this.maskedTextBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.maskedTextBox.Location = new System.Drawing.Point(8, 11);
            this.maskedTextBox.Name = "maskedTextBox";
            this.maskedTextBox.PromptChar = '#';
            this.maskedTextBox.ShortcutsEnabled = false;
            this.maskedTextBox.Size = new System.Drawing.Size(246, 26);
            this.maskedTextBox.TabIndex = 0;
            this.maskedTextBox.Text = "Enter Text";
            // 
            // TextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel);
            this.Name = "TextBox";
            this.Size = new System.Drawing.Size(263, 48);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CustomControls.RoundedPanel panel;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.MaskedTextBox maskedTextBox;
    }
}
