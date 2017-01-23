using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myHEALTHwareDesktop
{
    partial class ChromiumBrowserUserControl
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
			this.browserPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// browserPanel
			// 
			this.browserPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.browserPanel.Location = new System.Drawing.Point(0, 0);
			this.browserPanel.Name = "browserPanel";
			this.browserPanel.Size = new System.Drawing.Size(540, 400);
			this.browserPanel.TabIndex = 2;
			// 
			// ChromiumBrowserUsercontrol
			// 
			this.Controls.Add(this.browserPanel);
			this.Name = "ChromiumBrowserUserControl";
			this.Size = new System.Drawing.Size(730, 490);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel browserPanel;
	}
}
