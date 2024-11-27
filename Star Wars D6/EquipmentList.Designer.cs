namespace Star_Wars_D6
{
    partial class EquipmentList
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
            this.equipmentShop = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.equipmentShop)).BeginInit();
            this.SuspendLayout();
            // 
            // equipmentShop
            // 
            this.equipmentShop.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.equipmentShop.Location = new System.Drawing.Point(12, 12);
            this.equipmentShop.Name = "equipmentShop";
            this.equipmentShop.Size = new System.Drawing.Size(776, 463);
            this.equipmentShop.TabIndex = 0;
            // 
            // EquipmentList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 487);
            this.Controls.Add(this.equipmentShop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EquipmentList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Equipment";
            ((System.ComponentModel.ISupportInitialize)(this.equipmentShop)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView equipmentShop;
    }
}