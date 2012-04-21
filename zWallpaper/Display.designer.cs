namespace zWallpaper
{
	partial class Display
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Display));
			this.label2 = new System.Windows.Forms.Label();
			this.selectedImagePath = new System.Windows.Forms.Label();
			this.backgroundColorDialog = new System.Windows.Forms.ColorDialog();
			this.backgroundColorButton = new System.Windows.Forms.Button();
			this.selectImageButton = new System.Windows.Forms.Button();
			this.selectStyle = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.openImageFile = new System.Windows.Forms.OpenFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.clearImageButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 255);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(84, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Selected Image:";
			// 
			// selectedImagePath
			// 
			this.selectedImagePath.AutoEllipsis = true;
			this.selectedImagePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.selectedImagePath.Location = new System.Drawing.Point(102, 255);
			this.selectedImagePath.Name = "selectedImagePath";
			this.selectedImagePath.Size = new System.Drawing.Size(274, 23);
			this.selectedImagePath.TabIndex = 3;
			this.selectedImagePath.Text = "None";
			// 
			// backgroundColorButton
			// 
			this.backgroundColorButton.Location = new System.Drawing.Point(132, 277);
			this.backgroundColorButton.Name = "backgroundColorButton";
			this.backgroundColorButton.Size = new System.Drawing.Size(130, 23);
			this.backgroundColorButton.TabIndex = 4;
			this.backgroundColorButton.Text = "Background Color...";
			this.backgroundColorButton.UseVisualStyleBackColor = true;
			this.backgroundColorButton.Click += new System.EventHandler(this.backgroundColorButton_Click);
			// 
			// selectImageButton
			// 
			this.selectImageButton.Location = new System.Drawing.Point(12, 277);
			this.selectImageButton.Name = "selectImageButton";
			this.selectImageButton.Size = new System.Drawing.Size(114, 23);
			this.selectImageButton.TabIndex = 5;
			this.selectImageButton.Text = "Select Image...";
			this.selectImageButton.UseVisualStyleBackColor = true;
			this.selectImageButton.Click += new System.EventHandler(this.selectImageButton_Click);
			// 
			// selectStyle
			// 
			this.selectStyle.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.selectStyle.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.selectStyle.DisplayMember = "Text";
			this.selectStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.selectStyle.FormattingEnabled = true;
			this.selectStyle.Location = new System.Drawing.Point(307, 279);
			this.selectStyle.Name = "selectStyle";
			this.selectStyle.Size = new System.Drawing.Size(140, 21);
			this.selectStyle.TabIndex = 6;
			this.selectStyle.ValueMember = "Value";
			this.selectStyle.SelectedIndexChanged += new System.EventHandler(this.selectStyle_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(268, 282);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(33, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Style:";
			// 
			// openImageFile
			// 
			this.openImageFile.AutoUpgradeEnabled = false;
			this.openImageFile.InitialDirectory = "%HOMEPATH%";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Preview";
			// 
			// pictureBox
			// 
			this.pictureBox.BackColor = System.Drawing.SystemColors.Control;
			this.pictureBox.Location = new System.Drawing.Point(12, 25);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(435, 219);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			// 
			// clearImageButton
			// 
			this.clearImageButton.Location = new System.Drawing.Point(382, 250);
			this.clearImageButton.Name = "clearImageButton";
			this.clearImageButton.Size = new System.Drawing.Size(65, 23);
			this.clearImageButton.TabIndex = 8;
			this.clearImageButton.Text = "No Image";
			this.clearImageButton.UseVisualStyleBackColor = true;
			this.clearImageButton.Click += new System.EventHandler(this.clearImageButton_Click);
			// 
			// Display
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(459, 313);
			this.Controls.Add(this.clearImageButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.selectStyle);
			this.Controls.Add(this.selectImageButton);
			this.Controls.Add(this.backgroundColorButton);
			this.Controls.Add(this.selectedImagePath);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Display";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Desk Picture";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label selectedImagePath;
		private System.Windows.Forms.ColorDialog backgroundColorDialog;
		private System.Windows.Forms.Button backgroundColorButton;
		private System.Windows.Forms.Button selectImageButton;
		private System.Windows.Forms.ComboBox selectStyle;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.OpenFileDialog openImageFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.Button clearImageButton;
	}
}