using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Star_Wars_D6
{
    public partial class VehicleControl : UserControl
    {
        public VehicleControl()
        {
            InitializeComponent();

            // Initialize the dummyFocusLabel if not already present
            if (dummyFocusLabel == null)
            {
                dummyFocusLabel = new Label
                {
                    Visible = false,
                    Size = new Size(1, 1), // Small size to make it non-intrusive
                    Location = new Point(0, 0) // Place it in a corner
                };

                // Add to the form's control hierarchy
                this.Controls.Add(dummyFocusLabel);
            }

            // Attach event handler for shipImageButton
            shipImageButton.Click += ShipImageButton_Click;

            this.saveShip = new System.Windows.Forms.Button();
            this.loadShip = new System.Windows.Forms.Button();

            printDocument1 = new PrintDocument();
            printDocument1.PrintPage += PrintDocument_PrintPage;

            vehicleWeaponsContextMenu = new ContextMenuStrip();
            var deleteWeaponMenuItem = new ToolStripMenuItem("Delete Weapon");
            deleteWeaponMenuItem.Click += DeleteWeaponMenuItem_Click; // Add click event handler
            vehicleWeaponsContextMenu.Items.Add(deleteWeaponMenuItem);

            vehicleWeapons.ContextMenuStrip = vehicleWeaponsContextMenu;

            vehicleWeapons.MouseDown += VehicleWeapons_MouseDown;

            // Attach event handler for addWeaponButton
            addWeaponButton.Click += AddWeaponButton_Click;

            // Initialize the DataGridView columns
            InitializeVehicleWeaponsGrid();
        }

        private ContextMenuStrip vehicleWeaponsContextMenu;

        // Initialize the vehicleWeapons DataGridView
        private void InitializeVehicleWeaponsGrid()
        {
            // Ensure vehicleWeapons is set up properly
            vehicleWeapons.Columns.Clear();

            // Add columns in the specified order
            vehicleWeapons.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Weapon Name", Name = "WeaponName", Width = 125 });
            vehicleWeapons.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fire Arc", Name = "FireArc", Width = 80 });
            vehicleWeapons.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Crew", Name = "Crew", Width = 80 });
            vehicleWeapons.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Skill", Name = "Skill", Width = 100 });
            vehicleWeapons.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fire Control", Name = "FireControl", Width = 100 });
            vehicleWeapons.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Space Range", Name = "SpaceRange", Width = 100 });
            vehicleWeapons.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Atmosphere Range", Name = "AtmosphereRange", Width = 100 });
            vehicleWeapons.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Damage", Name = "Damage", Width = 80 });

            // Configure the DataGridView
            vehicleWeapons.AllowUserToAddRows = false;
            vehicleWeapons.AllowUserToDeleteRows = true;
            vehicleWeapons.AllowUserToOrderColumns = false;
            vehicleWeapons.ReadOnly = false;
            vehicleWeapons.RowTemplate.Height = 30;
        }

        // Event handler for addWeaponButton click
        private void AddWeaponButton_Click(object sender, EventArgs e)
        {
            // Add a blank row to the DataGridView
            vehicleWeapons.Rows.Add("", "", "", "", "", "", "", "");
        }

        private void VehicleWeapons_MouseDown(object sender, MouseEventArgs e)
        {
            // Show context menu only on right-click and when a row is clicked
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = vehicleWeapons.HitTest(e.X, e.Y);
                if (hitTestInfo.RowIndex >= 0) // Ensure a row was clicked
                {
                    vehicleWeapons.ClearSelection(); // Clear any previous selections
                    vehicleWeapons.Rows[hitTestInfo.RowIndex].Selected = true; // Select the row
                }
                else
                {
                    vehicleWeapons.ContextMenuStrip = null; // Disable context menu if no row is clicked
                }
            }
        }

        private void DeleteWeaponMenuItem_Click(object sender, EventArgs e)
        {
            if (vehicleWeapons.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show(
                    "Are you sure you would like to delete this vehicle weapon?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Delete the selected row
                    foreach (DataGridViewRow row in vehicleWeapons.SelectedRows)
                    {
                        vehicleWeapons.Rows.Remove(row);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "No Row Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void SaveShip_Click(object sender, EventArgs e)
        {
            VehicleIO.SaveVehicle(this);
        }

        private void LoadShip_Click(object sender, EventArgs e)
        {
            VehicleIO.LoadVehicle(this);
        }


        public string SelectedShipImagePath { get; set; }


        private void ShipImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All Files (*.*)|*.*";
                openFileDialog.Title = "Select Ship Image";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Load the selected image into the PictureBox
                        shipImageDisplay.Image = Image.FromFile(openFileDialog.FileName);
                        shipImageDisplay.SizeMode = PictureBoxSizeMode.Zoom;

                        // Save the selected image path
                        SelectedShipImagePath = openFileDialog.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private Bitmap shipBitmap; // Holds the bitmap for printing
        private PrintDocument printDocument1; // The PrintDocument object for printing


        private void shipPrint_Click(object sender, EventArgs e)
        {
            // Hide buttons before starting the print process
            ToggleControlsVisibility(false);

            try
            {
                // Ensure no controls are focused or selected
                ClearSelectionsAndFocus();

                // Capture the content of the form as a bitmap
                shipBitmap = new Bitmap(this.Width, this.Height);
                this.DrawToBitmap(shipBitmap, new Rectangle(0, 0, this.Width, this.Height));

                // Show the Print Dialog
                PrintDialog printDialog = new PrintDialog
                {
                    Document = printDocument1
                };

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    ClearSelectionsAndFocus(); // Ensure no focus before printing
                    printDocument1.Print();
                }
            }
            finally
            {
                // Restore button visibility after printing
                ToggleControlsVisibility(true);
                ClearSelectionsAndFocus(); // Ensure no focus after printing
            }
        }


        private void ClearSelectionsAndFocus()
        {
            // Ensure dummyFocusLabel is initialized and in the control hierarchy
            if (dummyFocusLabel == null)
            {
                dummyFocusLabel = new Label
                {
                    Visible = false,
                    Size = new Size(1, 1),
                    Location = new Point(0, 0)
                };
                this.Controls.Add(dummyFocusLabel);
            }

            // Clear selections in DataGridViews
            foreach (Control control in this.Controls)
            {
                if (control is DataGridView dgv)
                {
                    dgv.ClearSelection();
                }

                // Clear child control selections
                if (control.HasChildren)
                {
                    ClearChildSelectionsAndFocus(control);
                }
            }

            // Force dummy focus
            dummyFocusLabel.Focus();
            this.ActiveControl = dummyFocusLabel; // Ensure the dummy label gets focus
        }




        private void RestoreFocus()
        {
            // Optionally set focus back to the top-leftmost or any desired control
            if (this.Controls.Count > 0)
            {
                foreach (Control control in this.Controls)
                {
                    if (control is TextBox)
                    {
                        control.Focus();
                        break;
                    }
                }
            }
        }



        private void ClearChildSelectionsAndFocus(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is DataGridView dgv)
                {
                    dgv.ClearSelection();
                }
                else if (child is TextBox textBox)
                {
                    textBox.SelectionLength = 0; // Remove selection highlight in textboxes
                }

                if (child.HasChildren)
                {
                    ClearChildSelectionsAndFocus(child);
                }
            }
        }

        private void InitializeDummyFocusControl()
        {
            dummyFocusLabel = new Label
            {
                Size = new Size(1, 1), // Small size
                Location = new Point(0, 0), // Place in an unobtrusive location
                Visible = false // Invisible to the user
            };
            this.Controls.Add(dummyFocusLabel);
        }


        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (shipBitmap != null)
            {
                // Calculate scaling to fit the image to the page
                float xScale = (float)e.MarginBounds.Width / shipBitmap.Width;
                float yScale = (float)e.MarginBounds.Height / shipBitmap.Height;
                float scale = Math.Min(xScale, yScale); // Maintain aspect ratio

                // Calculate the destination rectangle
                int destWidth = (int)(shipBitmap.Width * scale);
                int destHeight = (int)(shipBitmap.Height * scale);
                int destX = e.MarginBounds.Left + (e.MarginBounds.Width - destWidth) / 2;
                int destY = e.MarginBounds.Top + (e.MarginBounds.Height - destHeight) / 2;

                // Draw the scaled image on the page
                e.Graphics.DrawImage(shipBitmap, destX, destY, destWidth, destHeight);
            }

            // Indicate no more pages to print
            e.HasMorePages = false;
        }


        private void ToggleControlsVisibility(bool isVisible)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    control.Visible = isVisible;
                }

                // Check inside nested containers like panels
                if (control.HasChildren)
                {
                    ToggleChildControlsVisibility(control, isVisible);
                }
            }
        }

        private void ToggleChildControlsVisibility(Control parent, bool isVisible)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is Button)
                {
                    child.Visible = isVisible;
                }

                if (child.HasChildren)
                {
                    ToggleChildControlsVisibility(child, isVisible);
                }
            }
        }



    }
}
