namespace myHEALTHwareDesktop
{
	partial class LoginForm
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
			// loadingControl1
			// 
			this.loadingControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.loadingControl.Location = new System.Drawing.Point(0, 0);
			this.loadingControl.Name = "loadingControl";
			this.loadingControl.Size = new System.Drawing.Size(474, 236);
			this.loadingControl.TabIndex = 0;
			// 
			// LoginForm
			// 
			this.AccessibleDescription = "Log in to myHEALTHware ";
			this.AccessibleName = "myHEALTHware Log in";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
			this.ClientSize = new System.Drawing.Size(474, 236);
			this.Controls.Add(this.loadingControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "myHEALTHware Login";
			this.ResumeLayout(false);

		}

		#endregion

		private LoadingControl loadingControl;


	}
}