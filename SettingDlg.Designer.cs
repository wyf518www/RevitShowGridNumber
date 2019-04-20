namespace ShowGridNumber
{
    partial class SettingDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingDlg));
      this.btn_Switcher = new System.Windows.Forms.Button();
      this.btn_Color = new System.Windows.Forms.Button();
      this.numericUpDown = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // btn_Switcher
      // 
      this.btn_Switcher.Location = new System.Drawing.Point(9, 9);
      this.btn_Switcher.Name = "btn_Switcher";
      this.btn_Switcher.Size = new System.Drawing.Size(75, 23);
      this.btn_Switcher.TabIndex = 0;
      this.btn_Switcher.Text = "轴号开关";
      this.btn_Switcher.UseVisualStyleBackColor = true;
      this.btn_Switcher.Click += new System.EventHandler(this.btn_Switcher_Click);
      // 
      // btn_Color
      // 
      this.btn_Color.ForeColor = System.Drawing.Color.Red;
      this.btn_Color.Location = new System.Drawing.Point(90, 9);
      this.btn_Color.Name = "btn_Color";
      this.btn_Color.Size = new System.Drawing.Size(75, 23);
      this.btn_Color.TabIndex = 0;
      this.btn_Color.Text = "文字颜色";
      this.btn_Color.UseVisualStyleBackColor = true;
      this.btn_Color.Click += new System.EventHandler(this.btn_Color_Click);
      // 
      // numericUpDown
      // 
      this.numericUpDown.Location = new System.Drawing.Point(232, 11);
      this.numericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.numericUpDown.Name = "numericUpDown";
      this.numericUpDown.Size = new System.Drawing.Size(40, 21);
      this.numericUpDown.TabIndex = 1;
      this.numericUpDown.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
      this.numericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
      this.label1.Location = new System.Drawing.Point(171, 14);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(59, 12);
      this.label1.TabIndex = 2;
      this.label1.Text = "文字大小:";
      // 
      // SettingDlg
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 41);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.numericUpDown);
      this.Controls.Add(this.btn_Color);
      this.Controls.Add(this.btn_Switcher);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SettingDlg";
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = "全局轴号设置";
      this.TopMost = true;
      this.Load += new System.EventHandler(this.SettingDlg_Load);
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Switcher;
        private System.Windows.Forms.Button btn_Color;
        private System.Windows.Forms.NumericUpDown numericUpDown;
        private System.Windows.Forms.Label label1;
    }
}