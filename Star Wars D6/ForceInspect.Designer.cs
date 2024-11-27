using System.Windows.Forms;

namespace Star_Wars_D6
{
    partial class ForceInspect
    {
        private System.ComponentModel.IContainer components = null;
        private Label forcePower;
        private PictureBox itemPictureBox;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Initialize components and controls in the designer
        private void InitializeComponent()
        {
            this.forcePower = new System.Windows.Forms.Label();
            this.descriptionWebBrowser = new System.Windows.Forms.WebBrowser();
            this.itemPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.itemPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // forcePower
            // 
            this.forcePower.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.forcePower.Location = new System.Drawing.Point(103, 17);
            this.forcePower.Name = "forcePower";
            this.forcePower.Size = new System.Drawing.Size(257, 26);
            this.forcePower.TabIndex = 0;
            this.forcePower.Text = "Item Name";
            // 
            // descriptionWebBrowser
            // 
            this.descriptionWebBrowser.Location = new System.Drawing.Point(12, 141);
            this.descriptionWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.descriptionWebBrowser.Name = "descriptionWebBrowser";
            this.descriptionWebBrowser.Size = new System.Drawing.Size(424, 150);
            this.descriptionWebBrowser.TabIndex = 6;
            // 
            // itemPictureBox
            // 
            this.itemPictureBox.Image = global::Star_Wars_D6.Properties.Resources.DefaultImageforce;
            this.itemPictureBox.Location = new System.Drawing.Point(12, 17);
            this.itemPictureBox.Name = "itemPictureBox";
            this.itemPictureBox.Size = new System.Drawing.Size(85, 69);
            this.itemPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.itemPictureBox.TabIndex = 5;
            this.itemPictureBox.TabStop = false;
            // 
            // ForceInspect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 303);
            this.Controls.Add(this.descriptionWebBrowser);
            this.Controls.Add(this.forcePower);
            this.Controls.Add(this.itemPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ForceInspect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Force Power";
            ((System.ComponentModel.ISupportInitialize)(this.itemPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        private WebBrowser descriptionWebBrowser;
    }
}
