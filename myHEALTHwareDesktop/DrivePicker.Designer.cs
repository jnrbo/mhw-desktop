namespace myHEALTHwareDesktop
{
	partial class DrivePicker
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
			fileName = this.textBoxFileName.Text;

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
			this.textBoxFileName = new System.Windows.Forms.TextBox();
			this.labelFileName = new System.Windows.Forms.Label();
			this.panelFileName = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// textBoxFileName
			// 
			this.textBoxFileName.AcceptsReturn = true;
			this.textBoxFileName.Location = new System.Drawing.Point(74, 14);
			this.textBoxFileName.Name = "textBoxFileName";
			this.textBoxFileName.Size = new System.Drawing.Size(289, 20);
			this.textBoxFileName.TabIndex = 0;
			// 
			// labelFileName
			// 
			this.labelFileName.AutoSize = true;
			this.labelFileName.Location = new System.Drawing.Point(13, 17);
			this.labelFileName.Name = "labelFileName";
			this.labelFileName.Size = new System.Drawing.Size(55, 13);
			this.labelFileName.TabIndex = 1;
			this.labelFileName.Text = "File name:";
			// 
			// panelFileName
			// 
			this.panelFileName.Location = new System.Drawing.Point(1, 2);
			this.panelFileName.Name = "panelFileName";
			this.panelFileName.Size = new System.Drawing.Size(382, 49);
			this.panelFileName.TabIndex = 2;
			// 
			// DrivePicker
			// 
			this.AccessibleDescription = "Pick a folder in myHEALTHware Drive";
			this.AccessibleName = "Drive Folder Picker";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
			this.ClientSize = new System.Drawing.Size(384, 541);
			this.Controls.Add(this.labelFileName);
			this.Controls.Add(this.textBoxFileName);
			this.Controls.Add(this.panelFileName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(400, 580);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 580);
			this.Name = "DrivePicker";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "myHEALTHware Drive";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxFileName;
		private System.Windows.Forms.Label labelFileName;
		private System.Windows.Forms.Panel panelFileName;
	}
}