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
			this.textAuthor = new System.Windows.Forms.TextBox();
			this.lblAuthor = new System.Windows.Forms.Label();
			this.textProvisionSiteUrl = new System.Windows.Forms.TextBox();
			this.lblProvisionSiteUrl = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textAuthor
			// 
			this.textAuthor.Location = new System.Drawing.Point(6, 21);
			this.textAuthor.Name = "textAuthor";
			this.textAuthor.Size = new System.Drawing.Size(324, 20);
			this.textAuthor.TabIndex = 3;
			// 
			// lblAuthor
			// 
			this.lblAuthor.AutoSize = true;
			this.lblAuthor.Location = new System.Drawing.Point(3, 4);
			this.lblAuthor.Name = "lblAuthor";
			this.lblAuthor.Size = new System.Drawing.Size(41, 13);
			this.lblAuthor.TabIndex = 2;
			this.lblAuthor.Text = "Author:";
			// 
			// textProvisionSiteUrl
			// 
			this.textProvisionSiteUrl.Location = new System.Drawing.Point(6, 67);
			this.textProvisionSiteUrl.Name = "textProvisionSiteUrl";
			this.textProvisionSiteUrl.Size = new System.Drawing.Size(324, 20);
			this.textProvisionSiteUrl.TabIndex = 8;
			// 
			// lblProvisionSiteUrl
			// 
			this.lblProvisionSiteUrl.AutoSize = true;
			this.lblProvisionSiteUrl.Location = new System.Drawing.Point(3, 51);
			this.lblProvisionSiteUrl.Name = "lblProvisionSiteUrl";
			this.lblProvisionSiteUrl.Size = new System.Drawing.Size(97, 13);
			this.lblProvisionSiteUrl.TabIndex = 7;
			this.lblProvisionSiteUrl.Text = "Provision site URL:";
			// 
			// TemplatePropertyPageControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textProvisionSiteUrl);
			this.Controls.Add(this.lblProvisionSiteUrl);
			this.Controls.Add(this.textAuthor);
			this.Controls.Add(this.lblAuthor);
			this.Name = "TemplatePropertyPageControl";
			this.Size = new System.Drawing.Size(677, 313);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox textAuthor;
		private System.Windows.Forms.Label lblAuthor;
		private System.Windows.Forms.TextBox textProvisionSiteUrl;
		private System.Windows.Forms.Label lblProvisionSiteUrl;
	}
}
