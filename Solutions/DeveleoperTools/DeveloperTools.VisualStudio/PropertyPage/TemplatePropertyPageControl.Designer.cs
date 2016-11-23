namespace SharePointPnP.DeveloperTools.VisualStudio
{
	partial class TemplatePropertyPageControl
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
			this.lblDisplayName = new System.Windows.Forms.Label();
			this.textDisplayName = new System.Windows.Forms.TextBox();
			this.textAuthor = new System.Windows.Forms.TextBox();
			this.lblAuthor = new System.Windows.Forms.Label();
			this.chkSupportSPO = new System.Windows.Forms.CheckBox();
			this.chkSupportSP13 = new System.Windows.Forms.CheckBox();
			this.chkSupportSP16 = new System.Windows.Forms.CheckBox();
			this.textImagePreviewUrl = new System.Windows.Forms.TextBox();
			this.lblImagePreviewUrl = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblDisplayName
			// 
			this.lblDisplayName.AutoSize = true;
			this.lblDisplayName.Location = new System.Drawing.Point(3, 0);
			this.lblDisplayName.Name = "lblDisplayName";
			this.lblDisplayName.Size = new System.Drawing.Size(73, 13);
			this.lblDisplayName.TabIndex = 0;
			this.lblDisplayName.Text = "Display name:";
			// 
			// textDisplayName
			// 
			this.textDisplayName.Location = new System.Drawing.Point(6, 17);
			this.textDisplayName.Name = "textDisplayName";
			this.textDisplayName.Size = new System.Drawing.Size(324, 20);
			this.textDisplayName.TabIndex = 1;
			// 
			// textAuthor
			// 
			this.textAuthor.Location = new System.Drawing.Point(6, 60);
			this.textAuthor.Name = "textAuthor";
			this.textAuthor.Size = new System.Drawing.Size(324, 20);
			this.textAuthor.TabIndex = 3;
			// 
			// lblAuthor
			// 
			this.lblAuthor.AutoSize = true;
			this.lblAuthor.Location = new System.Drawing.Point(3, 43);
			this.lblAuthor.Name = "lblAuthor";
			this.lblAuthor.Size = new System.Drawing.Size(41, 13);
			this.lblAuthor.TabIndex = 2;
			this.lblAuthor.Text = "Author:";
			// 
			// chkSupportSPO
			// 
			this.chkSupportSPO.AutoSize = true;
			this.chkSupportSPO.Location = new System.Drawing.Point(6, 133);
			this.chkSupportSPO.Name = "chkSupportSPO";
			this.chkSupportSPO.Size = new System.Drawing.Size(156, 17);
			this.chkSupportSPO.TabIndex = 4;
			this.chkSupportSPO.Text = "Supports SharePoint Online";
			this.chkSupportSPO.UseVisualStyleBackColor = true;
			// 
			// chkSupportSP13
			// 
			this.chkSupportSP13.AutoSize = true;
			this.chkSupportSP13.Location = new System.Drawing.Point(6, 156);
			this.chkSupportSP13.Name = "chkSupportSP13";
			this.chkSupportSP13.Size = new System.Drawing.Size(150, 17);
			this.chkSupportSP13.TabIndex = 5;
			this.chkSupportSP13.Text = "Supports SharePoint 2013";
			this.chkSupportSP13.UseVisualStyleBackColor = true;
			// 
			// chkSupportSP16
			// 
			this.chkSupportSP16.AutoSize = true;
			this.chkSupportSP16.Location = new System.Drawing.Point(6, 179);
			this.chkSupportSP16.Name = "chkSupportSP16";
			this.chkSupportSP16.Size = new System.Drawing.Size(150, 17);
			this.chkSupportSP16.TabIndex = 6;
			this.chkSupportSP16.Text = "Supports SharePoint 2016";
			this.chkSupportSP16.UseVisualStyleBackColor = true;
			// 
			// textImagePreviewUrl
			// 
			this.textImagePreviewUrl.Location = new System.Drawing.Point(6, 102);
			this.textImagePreviewUrl.Name = "textImagePreviewUrl";
			this.textImagePreviewUrl.Size = new System.Drawing.Size(324, 20);
			this.textImagePreviewUrl.TabIndex = 8;
			// 
			// lblImagePreviewUrl
			// 
			this.lblImagePreviewUrl.AutoSize = true;
			this.lblImagePreviewUrl.Location = new System.Drawing.Point(3, 86);
			this.lblImagePreviewUrl.Name = "lblImagePreviewUrl";
			this.lblImagePreviewUrl.Size = new System.Drawing.Size(104, 13);
			this.lblImagePreviewUrl.TabIndex = 7;
			this.lblImagePreviewUrl.Text = "Image preview URL:";
			// 
			// TemplatePropertyPageControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textImagePreviewUrl);
			this.Controls.Add(this.lblImagePreviewUrl);
			this.Controls.Add(this.chkSupportSP16);
			this.Controls.Add(this.chkSupportSP13);
			this.Controls.Add(this.chkSupportSPO);
			this.Controls.Add(this.textAuthor);
			this.Controls.Add(this.lblAuthor);
			this.Controls.Add(this.textDisplayName);
			this.Controls.Add(this.lblDisplayName);
			this.Name = "TemplatePropertyPageControl";
			this.Size = new System.Drawing.Size(677, 313);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblDisplayName;
		private System.Windows.Forms.TextBox textDisplayName;
		private System.Windows.Forms.TextBox textAuthor;
		private System.Windows.Forms.Label lblAuthor;
		private System.Windows.Forms.CheckBox chkSupportSPO;
		private System.Windows.Forms.CheckBox chkSupportSP13;
		private System.Windows.Forms.CheckBox chkSupportSP16;
		private System.Windows.Forms.TextBox textImagePreviewUrl;
		private System.Windows.Forms.Label lblImagePreviewUrl;
	}
}
