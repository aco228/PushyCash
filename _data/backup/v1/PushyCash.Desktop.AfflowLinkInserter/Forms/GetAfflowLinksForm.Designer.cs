﻿namespace PushyCash.Desktop.AfflowLinkInserter.Forms
{
	partial class GetAfflowLinksForm
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
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Location = new System.Drawing.Point(12, 12);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(1193, 750);
			this.textBox.TabIndex = 1;
			this.textBox.WordWrap = false;
			// 
			// GetAfflowLinksForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1217, 774);
			this.Controls.Add(this.textBox);
			this.Name = "GetAfflowLinksForm";
			this.Text = "GetAfflowLinksForm";
			this.Load += new System.EventHandler(this.GetAfflowLinksForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox;
	}
}