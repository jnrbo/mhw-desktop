using System.IO;

namespace myHEALTHwareDesktop
{
	partial class MhwDesktopForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MhwDesktopForm));
			this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeThisMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.differentContextMenuExampleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeThisMenuToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.runAtSystemStartupCheckBox = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.accountsComboBox = new System.Windows.Forms.ComboBox();
			this.buttonLogin = new System.Windows.Forms.Button();
			this.labelDisplayName = new System.Windows.Forms.Label();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabPagePrintToDrive = new System.Windows.Forms.TabPage();
			this.controlPrintToDrive = new myHEALTHwareDesktop.ControlPrintToDrive();
			this.tabPageFoldertoDrive = new System.Windows.Forms.TabPage();
			this.controlFolderToDrive = new myHEALTHwareDesktop.ControlFolderToDrive();
			this.tabPagePrintToFax = new System.Windows.Forms.TabPage();
			this.controlPrintToFax = new myHEALTHwareDesktop.ControlPrintToFax();
			this.tabPageAbout = new System.Windows.Forms.TabPage();
			this.controlAbout = new myHEALTHwareDesktop.ControlAbout();
			this.pictureBoxUser = new System.Windows.Forms.PictureBox();
			this.pictureBoxActingAs = new System.Windows.Forms.PictureBox();
			this.contextMenuStrip1.SuspendLayout();
			this.contextMenuStrip2.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabPagePrintToDrive.SuspendLayout();
			this.tabPageFoldertoDrive.SuspendLayout();
			this.tabPagePrintToFax.SuspendLayout();
			this.tabPageAbout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxUser)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxActingAs)).BeginInit();
			this.SuspendLayout();
			// 
			// trayIcon
			// 
			this.trayIcon.ContextMenuStrip = this.contextMenuStrip1;
			this.trayIcon.Icon = global::myHEALTHwareDesktop.Properties.Resources.myHEALTHware;
			this.trayIcon.Text = "myHEALTHware Desktop";
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.quitToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(117, 70);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.settingsToolStripMenuItem.Text = "Settings";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.aboutToolStripMenuItem.Text = "About";
			// 
			// quitToolStripMenuItem
			// 
			this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			this.quitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.quitToolStripMenuItem.Text = "Quit";
			this.quitToolStripMenuItem.ToolTipText = "Quit myHEALTHware for Windows";
			// 
			// closeThisMenuToolStripMenuItem
			// 
			this.closeThisMenuToolStripMenuItem.Name = "closeThisMenuToolStripMenuItem";
			this.closeThisMenuToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
			this.closeThisMenuToolStripMenuItem.Text = "Close This Menu";
			// 
			// contextMenuStrip2
			// 
			this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.differentContextMenuExampleToolStripMenuItem,
            this.closeThisMenuToolStripMenuItem1});
			this.contextMenuStrip2.Name = "contextMenuStrip2";
			this.contextMenuStrip2.Size = new System.Drawing.Size(246, 48);
			// 
			// differentContextMenuExampleToolStripMenuItem
			// 
			this.differentContextMenuExampleToolStripMenuItem.Name = "differentContextMenuExampleToolStripMenuItem";
			this.differentContextMenuExampleToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
			this.differentContextMenuExampleToolStripMenuItem.Text = "Different Context Menu Example";
			// 
			// closeThisMenuToolStripMenuItem1
			// 
			this.closeThisMenuToolStripMenuItem1.Name = "closeThisMenuToolStripMenuItem1";
			this.closeThisMenuToolStripMenuItem1.Size = new System.Drawing.Size(245, 22);
			this.closeThisMenuToolStripMenuItem1.Text = "Close This Menu";
			// 
			// runAtSystemStartupCheckBox
			// 
			this.runAtSystemStartupCheckBox.AutoSize = true;
			this.runAtSystemStartupCheckBox.Checked = true;
			this.runAtSystemStartupCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.runAtSystemStartupCheckBox.Location = new System.Drawing.Point(396, 330);
			this.runAtSystemStartupCheckBox.Name = "runAtSystemStartupCheckBox";
			this.runAtSystemStartupCheckBox.Size = new System.Drawing.Size(241, 17);
			this.runAtSystemStartupCheckBox.TabIndex = 2;
			this.runAtSystemStartupCheckBox.Text = "Run when Windows starts up (recommended)";
			this.runAtSystemStartupCheckBox.UseVisualStyleBackColor = true;
			this.runAtSystemStartupCheckBox.CheckedChanged += new System.EventHandler(this.RunAtSystemStartupCheckBoxCheckedChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(237, 21);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(130, 17);
			this.label5.TabIndex = 23;
			this.label5.Text = "Acting on behalf of:";
			// 
			// accountsComboBox
			// 
			this.accountsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.accountsComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.accountsComboBox.FormattingEnabled = true;
			this.accountsComboBox.Location = new System.Drawing.Point(369, 15);
			this.accountsComboBox.Name = "accountsComboBox";
			this.accountsComboBox.Size = new System.Drawing.Size(233, 28);
			this.accountsComboBox.TabIndex = 24;
			this.accountsComboBox.SelectionChangeCommitted += new System.EventHandler(this.AccountsComboBoxSelectionChangeCommitted);
			// 
			// buttonLogin
			// 
			this.buttonLogin.Location = new System.Drawing.Point(41, 49);
			this.buttonLogin.Name = "buttonLogin";
			this.buttonLogin.Size = new System.Drawing.Size(76, 31);
			this.buttonLogin.TabIndex = 25;
			this.buttonLogin.Text = "Log Out";
			this.buttonLogin.UseVisualStyleBackColor = true;
			this.buttonLogin.Click += new System.EventHandler(this.ButtonLoginClick);
			// 
			// labelDisplayName
			// 
			this.labelDisplayName.AutoSize = true;
			this.labelDisplayName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDisplayName.Location = new System.Drawing.Point(51, 18);
			this.labelDisplayName.Name = "labelDisplayName";
			this.labelDisplayName.Size = new System.Drawing.Size(98, 20);
			this.labelDisplayName.TabIndex = 26;
			this.labelDisplayName.Text = "User\'s name";
			// 
			// tabs
			// 
			this.tabs.Controls.Add(this.tabPagePrintToDrive);
			this.tabs.Controls.Add(this.tabPageFoldertoDrive);
			this.tabs.Controls.Add(this.tabPagePrintToFax);
			this.tabs.Controls.Add(this.tabPageAbout);
			this.tabs.Location = new System.Drawing.Point(11, 83);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(636, 237);
			this.tabs.TabIndex = 31;
			this.tabs.SelectedIndexChanged += new System.EventHandler(this.TabsSelectedIndexChanged);
			this.tabs.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.TabsSelecting);
			// 
			// tabPagePrintToDrive
			// 
			this.tabPagePrintToDrive.Controls.Add(this.controlPrintToDrive);
			this.tabPagePrintToDrive.Location = new System.Drawing.Point(4, 22);
			this.tabPagePrintToDrive.Name = "tabPagePrintToDrive";
			this.tabPagePrintToDrive.Padding = new System.Windows.Forms.Padding(3);
			this.tabPagePrintToDrive.Size = new System.Drawing.Size(628, 211);
			this.tabPagePrintToDrive.TabIndex = 2;
			this.tabPagePrintToDrive.Text = "Print to Drive";
			this.tabPagePrintToDrive.UseVisualStyleBackColor = true;
			// 
			// controlPrintToDrive
			// 
			this.controlPrintToDrive.Location = new System.Drawing.Point(25, 18);
			this.controlPrintToDrive.Name = "controlPrintToDrive";
			this.controlPrintToDrive.Size = new System.Drawing.Size(574, 173);
			this.controlPrintToDrive.TabIndex = 0;
			// 
			// tabPageFoldertoDrive
			// 
			this.tabPageFoldertoDrive.Controls.Add(this.controlFolderToDrive);
			this.tabPageFoldertoDrive.Location = new System.Drawing.Point(4, 22);
			this.tabPageFoldertoDrive.Name = "tabPageFoldertoDrive";
			this.tabPageFoldertoDrive.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageFoldertoDrive.Size = new System.Drawing.Size(628, 211);
			this.tabPageFoldertoDrive.TabIndex = 1;
			this.tabPageFoldertoDrive.Text = "Folder to Drive";
			this.tabPageFoldertoDrive.UseVisualStyleBackColor = true;
			// 
			// controlFolderToDrive
			// 
			this.controlFolderToDrive.Location = new System.Drawing.Point(25, 36);
			this.controlFolderToDrive.Name = "controlFolderToDrive";
			this.controlFolderToDrive.Size = new System.Drawing.Size(578, 138);
			this.controlFolderToDrive.TabIndex = 0;
			// 
			// tabPagePrintToFax
			// 
			this.tabPagePrintToFax.Controls.Add(this.controlPrintToFax);
			this.tabPagePrintToFax.Location = new System.Drawing.Point(4, 22);
			this.tabPagePrintToFax.Name = "tabPagePrintToFax";
			this.tabPagePrintToFax.Padding = new System.Windows.Forms.Padding(3);
			this.tabPagePrintToFax.Size = new System.Drawing.Size(628, 211);
			this.tabPagePrintToFax.TabIndex = 3;
			this.tabPagePrintToFax.Text = "Print to Fax";
			this.tabPagePrintToFax.UseVisualStyleBackColor = true;
			// 
			// controlPrintToFax
			// 
			this.controlPrintToFax.Location = new System.Drawing.Point(25, 18);
			this.controlPrintToFax.Name = "controlPrintToFax";
			this.controlPrintToFax.Size = new System.Drawing.Size(576, 173);
			this.controlPrintToFax.TabIndex = 0;
			// 
			// tabPageAbout
			// 
			this.tabPageAbout.Controls.Add(this.controlAbout);
			this.tabPageAbout.Location = new System.Drawing.Point(4, 22);
			this.tabPageAbout.Name = "tabPageAbout";
			this.tabPageAbout.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageAbout.Size = new System.Drawing.Size(628, 211);
			this.tabPageAbout.TabIndex = 4;
			this.tabPageAbout.Text = "About";
			this.tabPageAbout.UseVisualStyleBackColor = true;
			// 
			// controlAbout
			// 
			this.controlAbout.Location = new System.Drawing.Point(63, 15);
			this.controlAbout.Name = "controlAbout";
			this.controlAbout.Size = new System.Drawing.Size(492, 179);
			this.controlAbout.TabIndex = 0;
			// 
			// pictureBoxUser
			// 
			this.pictureBoxUser.Location = new System.Drawing.Point(12, 12);
			this.pictureBoxUser.Name = "pictureBoxUser";
			this.pictureBoxUser.Size = new System.Drawing.Size(35, 33);
			this.pictureBoxUser.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxUser.TabIndex = 27;
			this.pictureBoxUser.TabStop = false;
			// 
			// pictureBoxActingAs
			// 
			this.pictureBoxActingAs.Location = new System.Drawing.Point(608, 12);
			this.pictureBoxActingAs.Name = "pictureBoxActingAs";
			this.pictureBoxActingAs.Size = new System.Drawing.Size(35, 33);
			this.pictureBoxActingAs.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxActingAs.TabIndex = 32;
			this.pictureBoxActingAs.TabStop = false;
			// 
			// MhwDesktopForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(655, 355);
			this.Controls.Add(this.runAtSystemStartupCheckBox);
			this.Controls.Add(this.pictureBoxUser);
			this.Controls.Add(this.buttonLogin);
			this.Controls.Add(this.pictureBoxActingAs);
			this.Controls.Add(this.labelDisplayName);
			this.Controls.Add(this.tabs);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.accountsComboBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MhwDesktopForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "myHEALTHware Desktop Settings";
			this.Load += new System.EventHandler(this.MhwFormInitialize);
			this.contextMenuStrip1.ResumeLayout(false);
			this.contextMenuStrip2.ResumeLayout(false);
			this.tabs.ResumeLayout(false);
			this.tabPagePrintToDrive.ResumeLayout(false);
			this.tabPageFoldertoDrive.ResumeLayout(false);
			this.tabPagePrintToFax.ResumeLayout(false);
			this.tabPageAbout.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxUser)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxActingAs)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NotifyIcon trayIcon;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeThisMenuToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
		private System.Windows.Forms.ToolStripMenuItem differentContextMenuExampleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeThisMenuToolStripMenuItem1;
		private System.Windows.Forms.CheckBox runAtSystemStartupCheckBox;
		//private System.Windows.Forms.NotifyIcon notLoggedInIcon;
		//private System.Windows.Forms.NotifyIcon uploadingIcon;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox accountsComboBox;
		private System.Windows.Forms.Button buttonLogin;
		private System.Windows.Forms.Label labelDisplayName;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabPageFoldertoDrive;
		private System.Windows.Forms.TabPage tabPagePrintToDrive;
		private System.Windows.Forms.TabPage tabPagePrintToFax;
		private System.Windows.Forms.TabPage tabPageAbout;
		private System.Windows.Forms.PictureBox pictureBoxUser;
		private System.Windows.Forms.PictureBox pictureBoxActingAs;
		private ControlFolderToDrive controlFolderToDrive;
		private ControlPrintToDrive controlPrintToDrive;
		private ControlPrintToFax controlPrintToFax;
		private ControlAbout controlAbout;
	}
}

