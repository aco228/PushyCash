namespace PushyCash.Desktop.AfflowLinkInserter
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
			this.textBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSubmit = new System.Windows.Forms.Button();
			this.comboBoxNetworks = new System.Windows.Forms.ComboBox();
			this.textBoxLogs = new System.Windows.Forms.TextBox();
			this.btnAfflowUrls = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Location = new System.Drawing.Point(12, 31);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(751, 646);
			this.textBox.TabIndex = 0;
			this.textBox.WordWrap = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(235, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Link names ( _MA.inwi.mainstream.ios.zeropark )";
			// 
			// btnSubmit
			// 
			this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSubmit.Location = new System.Drawing.Point(962, 688);
			this.btnSubmit.Name = "btnSubmit";
			this.btnSubmit.Size = new System.Drawing.Size(98, 23);
			this.btnSubmit.TabIndex = 2;
			this.btnSubmit.Text = "Submit";
			this.btnSubmit.UseVisualStyleBackColor = true;
			this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
			// 
			// comboBoxNetworks
			// 
			this.comboBoxNetworks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxNetworks.FormattingEnabled = true;
			this.comboBoxNetworks.Location = new System.Drawing.Point(835, 689);
			this.comboBoxNetworks.Name = "comboBoxNetworks";
			this.comboBoxNetworks.Size = new System.Drawing.Size(121, 21);
			this.comboBoxNetworks.TabIndex = 4;
			// 
			// textBoxLogs
			// 
			this.textBoxLogs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLogs.Location = new System.Drawing.Point(769, 31);
			this.textBoxLogs.Multiline = true;
			this.textBoxLogs.Name = "textBoxLogs";
			this.textBoxLogs.Size = new System.Drawing.Size(291, 646);
			this.textBoxLogs.TabIndex = 5;
			this.textBoxLogs.WordWrap = false;
			// 
			// btnAfflowUrls
			// 
			this.btnAfflowUrls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAfflowUrls.Location = new System.Drawing.Point(12, 689);
			this.btnAfflowUrls.Name = "btnAfflowUrls";
			this.btnAfflowUrls.Size = new System.Drawing.Size(98, 23);
			this.btnAfflowUrls.TabIndex = 6;
			this.btnAfflowUrls.Text = "Get afflow urls";
			this.btnAfflowUrls.UseVisualStyleBackColor = true;
			this.btnAfflowUrls.Click += new System.EventHandler(this.btnAfflowUrls_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1072, 721);
			this.Controls.Add(this.btnAfflowUrls);
			this.Controls.Add(this.textBoxLogs);
			this.Controls.Add(this.comboBoxNetworks);
			this.Controls.Add(this.btnSubmit);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox);
			this.Name = "MainForm";
			this.Text = "Afflow link inserter";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSubmit;
		private System.Windows.Forms.ComboBox comboBoxNetworks;
		private System.Windows.Forms.TextBox textBoxLogs;
		private System.Windows.Forms.Button btnAfflowUrls;
	}
}

