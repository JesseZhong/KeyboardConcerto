namespace KeyboardConcerto {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.TextBox = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// TextBox
			// 
			this.TextBox.BackColor = System.Drawing.SystemColors.Window;
			this.TextBox.Location = new System.Drawing.Point(17, 16);
			this.TextBox.Margin = new System.Windows.Forms.Padding(4);
			this.TextBox.Name = "TextBox";
			this.TextBox.ReadOnly = true;
			this.TextBox.Size = new System.Drawing.Size(781, 469);
			this.TextBox.TabIndex = 0;
			this.TextBox.Text = "";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(816, 501);
			this.Controls.Add(this.TextBox);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "MainForm";
			this.Text = "Concerto";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox TextBox;

	}
}

