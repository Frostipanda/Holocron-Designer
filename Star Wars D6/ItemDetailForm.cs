using System;
using System.Drawing;
using System.Windows.Forms;

namespace Star_Wars_D6
{
    public partial class ItemDetailForm : Form
    {
        public ItemDetailForm()
        {
            InitializeComponent();
        }

        // Set item details to display in the form
        public void SetItemDetails(string name, string type, string availability, string price, string description, Image itemImage)
        {
            itemNameLabel.Text = name;
            itemTypeLabel.Text = $"Type: {type}";
            itemAvailabilityLabel.Text = $"Availability: {availability}";
            itemPriceLabel.Text = $"Price: {price}";
            itemPictureBox.Image = itemImage ?? Properties.Resources.DefaultImage;  // Set default image if null

            // Use WebBrowser control to display HTML-formatted description
            descriptionWebBrowser.DocumentText = description;
        }
    }
}
