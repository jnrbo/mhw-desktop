namespace myHEALTHwareDesktop
{
	partial class MhwMessageForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MhwMessageForm));
			this.mhwLogoPictureBox = new System.Windows.Forms.PictureBox();
			this.messageLabel = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.mhwLogoPictureBox)).BeginInit();
			this.layoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// mhwLogoPictureBox
			// 
			this.mhwLogoPictureBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.mhwLogoPictureBox.Image = global::myHEALTHwareDesktop.Properties.Resources.mhw_logo_224;
			this.mhwLogoPictureBox.Location = new System.Drawing.Point(3, 34);
			this.mhwLogoPictureBox.Name = "mhwLogoPictureBox";
			this.mhwLogoPictureBox.Size = new System.Drawing.Size(90, 64);
			this.mhwLogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.mhwLogoPictureBox.TabIndex = 0;
			this.mhwLogoPictureBox.TabStop = false;
			// 
			// messageLabel
			// 
			this.messageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.messageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.messageLabel.Location = new System.Drawing.Point(99, 31);
			this.messageLabel.Name = "messageLabel";
			this.messageLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this.messageLabel.Size = new System.Drawing.Size(326, 93);
			this.messageLabel.TabIndex = 1;
			this.messageLabel.Text = "You aren\'t allowed to print to The Family Clinic\'s Drive folder. Please contact a" +
    "n administrator to get this corrected.";
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(350, 134);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 2;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.OkButtonClick);
			// 
			// layoutPanel
			// 
			this.layoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.layoutPanel.ColumnCount = 2;
			this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
			this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.layoutPanel.Controls.Add(this.mhwLogoPictureBox, 0, 1);
			this.layoutPanel.Controls.Add(this.okButton, 1, 2);
			this.layoutPanel.Controls.Add(this.messageLabel, 1, 1);
			this.layoutPanel.Location = new System.Drawing.Point(12, 12);
			this.layoutPanel.Name = "layoutPanel";
			this.layoutPanel.RowCount = 3;
			this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.81196F));
			this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 58.43159F));
			this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 21.75644F));
			this.layoutPanel.Size = new System.Drawing.Size(428, 160);
			this.layoutPanel.TabIndex = 3;
			// 
			// MhwMessageForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(452, 184);
			this.Controls.Add(this.layoutPanel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MhwMessageForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Print to Drive";
			this.Shown += new System.EventHandler(this.MhwMessageFormShown);
			((System.ComponentModel.ISupportInitialize)(this.mhwLogoPictureBox)).EndInit();
			this.layoutPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox mhwLogoPictureBox;
		private System.Windows.Forms.Label messageLabel;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.TableLayoutPanel layoutPanel;
	}
}