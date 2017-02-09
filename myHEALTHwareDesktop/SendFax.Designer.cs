namespace myHEALTHwareDesktop
{
	partial class SendFax
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
			this.loadingControl = new myHEALTHwareDesktop.LoadingControl();
			this.SuspendLayout();
			// 
			// loadingControl
			// 
			this.loadingControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.loadingControl.Location = new System.Drawing.Point(0, 0);
			this.loadingControl.Name = "loadingControl";
			this.loadingControl.Size = new System.Drawing.Size(534, 531);
			this.loadingControl.TabIndex = 0;
			this.loadingControl.UseWaitCursor = true;
			// 
			// SendFax
			// 
			this.AccessibleDescription = "Send as fax in myHEALTHware ";
			this.AccessibleName = "myHEALTHware Send Fax";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
			this.ClientSize = new System.Drawing.Size(534, 531);
			this.Controls.Add(this.loadingControl);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 500);
			this.Name = "SendFax";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "myHEALTHware Fax";
			this.UseWaitCursor = true;
			this.Shown += new System.EventHandler(this.SendFaxShown);
			this.ResumeLayout(false);

		}

		#endregion

		private LoadingControl loadingControl;
	}
}