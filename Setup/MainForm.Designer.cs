namespace Setup
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.buttonExit = new System.Windows.Forms.Button();
			this.listViewLog = new System.Windows.Forms.ListView();
			this.columnHeaderMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label1 = new System.Windows.Forms.Label();
			this.buttonCopy = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonExit
			// 
			this.buttonExit.Enabled = false;
			this.buttonExit.Location = new System.Drawing.Point(177, 345);
			this.buttonExit.Name = "buttonExit";
			this.buttonExit.Size = new System.Drawing.Size(78, 27);
			this.buttonExit.TabIndex = 1;
			this.buttonExit.Text = "Okay";
			this.buttonExit.UseVisualStyleBackColor = true;
			this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
			// 
			// listViewLog
			// 
			this.listViewLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderMessage});
			this.listViewLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listViewLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewLog.Location = new System.Drawing.Point(21, 50);
			this.listViewLog.Name = "listViewLog";
			this.listViewLog.Size = new System.Drawing.Size(535, 279);
			this.listViewLog.TabIndex = 2;
			this.listViewLog.UseCompatibleStateImageBehavior = false;
			this.listViewLog.View = System.Windows.Forms.View.Details;
			this.listViewLog.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownHandler);
			// 
			// columnHeaderMessage
			// 
			this.columnHeaderMessage.Width = 531;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(28, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Log:";
			// 
			// buttonCopy
			// 
			this.buttonCopy.Location = new System.Drawing.Point(342, 345);
			this.buttonCopy.Name = "buttonCopy";
			this.buttonCopy.Size = new System.Drawing.Size(80, 27);
			this.buttonCopy.TabIndex = 4;
			this.buttonCopy.Text = "Copy Log";
			this.buttonCopy.UseVisualStyleBackColor = true;
			this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(581, 390);
			this.Controls.Add(this.buttonCopy);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listViewLog);
			this.Controls.Add(this.buttonExit);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "myHEALTHware Printer Setup";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonExit;
		private System.Windows.Forms.ListView listViewLog;
		private System.Windows.Forms.ColumnHeader columnHeaderMessage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonCopy;
	}
}

