namespace myHEALTHwareDesktop
{
	public partial class ControlPrintToFax
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
			this.buttonPrintToFaxInstall = new System.Windows.Forms.Button();
			this.labelStatus = new System.Windows.Forms.Label();
			this.pictureStartedStopped = new System.Windows.Forms.PictureBox();
			this.groupBoxPrintToFax = new System.Windows.Forms.GroupBox();
			this.pictureBoxMarketplaceFax = new System.Windows.Forms.PictureBox();
			this.labelFaxLearnMoreLink = new System.Windows.Forms.LinkLabel();
			this.radioButtonSaveDraft = new System.Windows.Forms.RadioButton();
			this.radioButtonPrompt = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.pictureStartedStopped)).BeginInit();
			this.groupBoxPrintToFax.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxMarketplaceFax)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonPrintToFaxInstall
			// 
			this.buttonPrintToFaxInstall.Location = new System.Drawing.Point(429, 146);
			this.buttonPrintToFaxInstall.Name = "buttonPrintToFaxInstall";
			this.buttonPrintToFaxInstall.Size = new System.Drawing.Size(75, 23);
			this.buttonPrintToFaxInstall.TabIndex = 1;
			this.buttonPrintToFaxInstall.Text = "Install Printer";
			this.buttonPrintToFaxInstall.UseVisualStyleBackColor = true;
			this.buttonPrintToFaxInstall.Click += new System.EventHandler(this.buttonPrintToFaxInstall_Click);
			// 
			// labelStatus
			// 
			this.labelStatus.Location = new System.Drawing.Point(77, 151);
			this.labelStatus.Margin = new System.Windows.Forms.Padding(0);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(296, 13);
			this.labelStatus.TabIndex = 2;
			this.labelStatus.Text = "myHEALTHware Fax is not installed on the selected account.";
			this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// pictureStartedStopped
			// 
			this.pictureStartedStopped.Image = global::myHEALTHwareDesktop.Properties.Resources.stopped;
			this.pictureStartedStopped.Location = new System.Drawing.Point(381, 141);
			this.pictureStartedStopped.Name = "pictureStartedStopped";
			this.pictureStartedStopped.Size = new System.Drawing.Size(32, 32);
			this.pictureStartedStopped.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureStartedStopped.TabIndex = 40;
			this.pictureStartedStopped.TabStop = false;
			// 
			// groupBoxPrintToFax
			// 
			this.groupBoxPrintToFax.Controls.Add(this.pictureBoxMarketplaceFax);
			this.groupBoxPrintToFax.Controls.Add(this.labelFaxLearnMoreLink);
			this.groupBoxPrintToFax.Location = new System.Drawing.Point(9, 13);
			this.groupBoxPrintToFax.Name = "groupBoxPrintToFax";
			this.groupBoxPrintToFax.Size = new System.Drawing.Size(516, 117);
			this.groupBoxPrintToFax.TabIndex = 41;
			this.groupBoxPrintToFax.TabStop = false;
			this.groupBoxPrintToFax.Text = "With each print:";
			// 
			// pictureBoxMarketplaceFax
			// 
			this.pictureBoxMarketplaceFax.AccessibleDescription = "myHEALTHware Fax in Marketplace";
			this.pictureBoxMarketplaceFax.AccessibleName = "myHEALTHware Fax in Marketplace";
			this.pictureBoxMarketplaceFax.Image = global::myHEALTHwareDesktop.Properties.Resources.marketplace_fax;
			this.pictureBoxMarketplaceFax.Location = new System.Drawing.Point(413, 12);
			this.pictureBoxMarketplaceFax.Name = "pictureBoxMarketplaceFax";
			this.pictureBoxMarketplaceFax.Size = new System.Drawing.Size(64, 64);
			this.pictureBoxMarketplaceFax.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxMarketplaceFax.TabIndex = 45;
			this.pictureBoxMarketplaceFax.TabStop = false;
			this.pictureBoxMarketplaceFax.Click += new System.EventHandler(this.pictureBoxMarketplaceFax_Click);
			// 
			// labelFaxLearnMoreLink
			// 
			this.labelFaxLearnMoreLink.AutoSize = true;
			this.labelFaxLearnMoreLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelFaxLearnMoreLink.Location = new System.Drawing.Point(223, 79);
			this.labelFaxLearnMoreLink.Name = "labelFaxLearnMoreLink";
			this.labelFaxLearnMoreLink.Size = new System.Drawing.Size(287, 20);
			this.labelFaxLearnMoreLink.TabIndex = 44;
			this.labelFaxLearnMoreLink.TabStop = true;
			this.labelFaxLearnMoreLink.Text = "Learn more about myHEALTHware Fax";
			this.labelFaxLearnMoreLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelFaxLearnMoreLink_LinkClicked);
			// 
			// radioButtonSaveDraft
			// 
			this.radioButtonSaveDraft.AutoSize = true;
			this.radioButtonSaveDraft.Location = new System.Drawing.Point(38, 84);
			this.radioButtonSaveDraft.Name = "radioButtonSaveDraft";
			this.radioButtonSaveDraft.Size = new System.Drawing.Size(151, 17);
			this.radioButtonSaveDraft.TabIndex = 43;
			this.radioButtonSaveDraft.TabStop = true;
			this.radioButtonSaveDraft.Text = "Automatically save as draft";
			this.radioButtonSaveDraft.UseVisualStyleBackColor = true;
			this.radioButtonSaveDraft.CheckedChanged += new System.EventHandler(this.radioButtons_CheckedChanged);
			// 
			// radioButtonPrompt
			// 
			this.radioButtonPrompt.AutoSize = true;
			this.radioButtonPrompt.Location = new System.Drawing.Point(38, 45);
			this.radioButtonPrompt.Name = "radioButtonPrompt";
			this.radioButtonPrompt.Size = new System.Drawing.Size(113, 17);
			this.radioButtonPrompt.TabIndex = 42;
			this.radioButtonPrompt.TabStop = true;
			this.radioButtonPrompt.Text = "Prompt to send fax";
			this.radioButtonPrompt.UseVisualStyleBackColor = true;
			this.radioButtonPrompt.CheckedChanged += new System.EventHandler(this.radioButtons_CheckedChanged);
			// 
			// ControlPrintToFax
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.radioButtonSaveDraft);
			this.Controls.Add(this.radioButtonPrompt);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.pictureStartedStopped);
			this.Controls.Add(this.buttonPrintToFaxInstall);
			this.Controls.Add(this.groupBoxPrintToFax);
			this.Name = "ControlPrintToFax";
			this.Size = new System.Drawing.Size(532, 187);
			((System.ComponentModel.ISupportInitialize)(this.pictureStartedStopped)).EndInit();
			this.groupBoxPrintToFax.ResumeLayout(false);
			this.groupBoxPrintToFax.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxMarketplaceFax)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonPrintToFaxInstall;
		private System.Windows.Forms.Label labelStatus;
		private System.Windows.Forms.PictureBox pictureStartedStopped;
		private System.Windows.Forms.GroupBox groupBoxPrintToFax;
		private System.Windows.Forms.RadioButton radioButtonSaveDraft;
		private System.Windows.Forms.RadioButton radioButtonPrompt;
		private System.Windows.Forms.LinkLabel labelFaxLearnMoreLink;
		private System.Windows.Forms.PictureBox pictureBoxMarketplaceFax;
	}
}
