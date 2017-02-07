namespace myHEALTHwareDesktop
{
	partial class LoadingControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.loadingLabel = new System.Windows.Forms.Label();
			this.logoPictureBox = new System.Windows.Forms.PictureBox();
			this.timer = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// loadingLabel
			// 
			this.loadingLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.loadingLabel.AutoSize = true;
			this.loadingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.loadingLabel.ForeColor = System.Drawing.Color.DimGray;
			this.loadingLabel.Location = new System.Drawing.Point(104, 106);
			this.loadingLabel.Name = "loadingLabel";
			this.loadingLabel.Size = new System.Drawing.Size(78, 20);
			this.loadingLabel.TabIndex = 4;
			this.loadingLabel.Text = "Loading...";
			this.loadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// logoPictureBox
			// 
			this.logoPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.logoPictureBox.Image = global::myHEALTHwareDesktop.Properties.Resources.mhw_logo_bg_72;
			this.logoPictureBox.Location = new System.Drawing.Point(3, 31);
			this.logoPictureBox.Name = "logoPictureBox";
			this.logoPictureBox.Size = new System.Drawing.Size(279, 72);
			this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.logoPictureBox.TabIndex = 5;
			this.logoPictureBox.TabStop = false;
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Interval = 1000;
			this.timer.Tick += new System.EventHandler(this.TimerTick);
			// 
			// LoadingControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.logoPictureBox);
			this.Controls.Add(this.loadingLabel);
			this.Name = "LoadingControl";
			this.Size = new System.Drawing.Size(285, 160);
			((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox logoPictureBox;
		private System.Windows.Forms.Label loadingLabel;
		private System.Windows.Forms.Timer timer;
	}
}
