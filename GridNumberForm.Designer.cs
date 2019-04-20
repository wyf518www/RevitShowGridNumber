namespace ShowGridNumber
{
	public partial class GridNumberForm : global::System.Windows.Forms.Form
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
            //缩放模式
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = global::System.Drawing.Color.White;
			base.ClientSize = new global::System.Drawing.Size(132, 76);
            //无边框模式
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.None;
			base.Name = "GridNumberForm";
            //是否在任务栏显示
			base.ShowInTaskbar = false;
			base.TransparencyKey = global::System.Drawing.Color.White;
			base.FormClosed += new global::System.Windows.Forms.FormClosedEventHandler(this.GridNumberForm_FormClosed);
			base.Load += new global::System.EventHandler(this.GridNumberForm_Load);
			base.ResumeLayout(false);
		}

		private global::System.ComponentModel.IContainer components=null;
	}
}
