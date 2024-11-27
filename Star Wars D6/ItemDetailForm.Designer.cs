using System.Windows.Forms;

namespace Star_Wars_D6
{
    partial class ItemDetailForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label itemNameLabel;
        private Label itemTypeLabel;
        private Label itemAvailabilityLabel;
        private Label itemPriceLabel;
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
            this.itemNameLabel = new System.Windows.Forms.Label();
            this.itemTypeLabel = new System.Windows.Forms.Label();
            this.itemAvailabilityLabel = new System.Windows.Forms.Label();
            this.itemPriceLabel = new System.Windows.Forms.Label();
            this.descriptionWebBrowser = new System.Windows.Forms.WebBrowser();
            this.itemPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.itemPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // itemNameLabel
            // 
            this.itemNameLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.itemNameLabel.Location = new System.Drawing.Point(103, 17);
            this.itemNameLabel.Name = "itemNameLabel";
            this.itemNameLabel.Size = new System.Drawing.Size(257, 26);
            this.itemNameLabel.TabIndex = 0;
            this.itemNameLabel.Text = "Item Name";
            // 
            // itemTypeLabel
            // 
            this.itemTypeLabel.Location = new System.Drawing.Point(103, 52);
            this.itemTypeLabel.Name = "itemTypeLabel";
            this.itemTypeLabel.Size = new System.Drawing.Size(257, 17);
            this.itemTypeLabel.TabIndex = 1;
            this.itemTypeLabel.Text = "Type: ";
            // 
            // itemAvailabilityLabel
            // 
            this.itemAvailabilityLabel.Location = new System.Drawing.Point(103, 78);
            this.itemAvailabilityLabel.Name = "itemAvailabilityLabel";
            this.itemAvailabilityLabel.Size = new System.Drawing.Size(257, 17);
            this.itemAvailabilityLabel.TabIndex = 2;
            this.itemAvailabilityLabel.Text = "Availability: ";
            // 
            // itemPriceLabel
            // 
            this.itemPriceLabel.Location = new System.Drawing.Point(103, 104);
            this.itemPriceLabel.Name = "itemPriceLabel";
            this.itemPriceLabel.Size = new System.Drawing.Size(257, 17);
            this.itemPriceLabel.TabIndex = 3;
            this.itemPriceLabel.Text = "Price: ";
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
            this.itemPictureBox.Location = new System.Drawing.Point(17, 17);
            this.itemPictureBox.Name = "itemPictureBox";
            this.itemPictureBox.Size = new System.Drawing.Size(69, 69);
            this.itemPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.itemPictureBox.TabIndex = 5;
            this.itemPictureBox.TabStop = false;
            // 
            // ItemDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 303);
            this.Controls.Add(this.descriptionWebBrowser);
            this.Controls.Add(this.itemNameLabel);
            this.Controls.Add(this.itemTypeLabel);
            this.Controls.Add(this.itemAvailabilityLabel);
            this.Controls.Add(this.itemPriceLabel);
            this.Controls.Add(this.itemPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ItemDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Item Details";
            ((System.ComponentModel.ISupportInitialize)(this.itemPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        private WebBrowser descriptionWebBrowser;
    }
}
