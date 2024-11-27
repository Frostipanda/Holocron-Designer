using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Star_Wars_D6
{
    public partial class EquipmentList : Form
    {
        public EquipmentList()
        {
            InitializeComponent();
            ConfigureEquipmentShopGrid();
            LoadEquipmentShopItems();
            ConfigureEquipmentShopContextMenu();
        }

        private void ConfigureEquipmentShopGrid()
        {
            equipmentShop.AutoGenerateColumns = false;
            equipmentShop.Columns.Clear();

            // Define columns
            equipmentShop.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                DataPropertyName = "Name",
                Name = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            equipmentShop.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Type",
                DataPropertyName = "Type",
                Name = "Type",
                Width = 100
            });
            equipmentShop.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Cost",
                DataPropertyName = "Cost",
                Name = "Cost",
                Width = 100
            });
            equipmentShop.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Physical Armor",
                DataPropertyName = "PhysicalArmorText",
                Name = "PhysicalArmorText",
                Width = 100
            });
            equipmentShop.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Energy Armor",
                DataPropertyName = "EnergyArmorText",
                Name = "EnergyArmorText",
                Width = 100
            });

            equipmentShop.AllowUserToAddRows = false;
            equipmentShop.ReadOnly = true;
            equipmentShop.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }


        private void LoadEquipmentShopItems()
        {
            try
            {
                // List of JSON file paths
                string[] jsonFiles = {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Equipment.json"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "General_Goods.json"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Weapons.json")
        };

                // Load items from JSON files
                List<EquipmentItem> items = new List<EquipmentItem>();

                foreach (string file in jsonFiles)
                {
                    if (File.Exists(file))
                    {
                        string jsonContent = File.ReadAllText(file);
                        JObject rootObject = JObject.Parse(jsonContent);

                        if (rootObject["items"] is JArray jsonArray)
                        {
                            foreach (var obj in jsonArray)
                            {
                                string cost = obj["system"]?["cost"]?.ToString() ?? "N/A";
                                int physicalArmor = obj["system"]?["pr"]?.ToObject<int>() ?? 0;
                                int energyArmor = obj["system"]?["er"]?.ToObject<int>() ?? 0;

                                items.Add(new EquipmentItem
                                {
                                    Name = obj["name"]?.ToString() ?? "Unknown",
                                    Type = obj["type"]?.ToString() ?? "Gear",
                                    Cost = cost,
                                    PhysicalArmorText = physicalArmor > 0 ? ConvertPipsToDiceNotation(physicalArmor) : "",
                                    EnergyArmorText = energyArmor > 0 ? ConvertPipsToDiceNotation(energyArmor) : ""
                                });
                            }
                        }
                    }
                }

                // Sort and bind to grid
                equipmentShop.DataSource = items.OrderBy(item => item.Type).ThenBy(item => item.Name).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading equipment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private string ConvertPipsToDiceNotation(int pips)
        {
            int dice = pips / 3;
            int remainder = pips % 3;

            if (dice > 0 && remainder > 0)
                return $"{dice}D+{remainder}";
            else if (dice > 0)
                return $"{dice}D";
            else
                return $"+{remainder}";
        }



        


        private ContextMenuStrip equipmentContextMenu;

        private void ConfigureEquipmentShopContextMenu()
        {
            // Create the context menu
            equipmentContextMenu = new ContextMenuStrip();

            // Add "Loot" menu item
            var lootMenuItem = new ToolStripMenuItem("Loot");
            lootMenuItem.Click += LootMenuItem_Click;
            equipmentContextMenu.Items.Add(lootMenuItem);

            // Add "Buy" menu item
            var buyMenuItem = new ToolStripMenuItem("Buy");
            buyMenuItem.Click += BuyMenuItem_Click;
            equipmentContextMenu.Items.Add(buyMenuItem);

            // Add "Inspect" menu item
            var inspectMenuItem = new ToolStripMenuItem("Inspect");
            inspectMenuItem.Click += InspectMenuItem_Click;
            equipmentContextMenu.Items.Add(inspectMenuItem);

            // Attach the context menu to the DataGridView
            equipmentShop.ContextMenuStrip = equipmentContextMenu;

            // Handle right-click events
            equipmentShop.MouseDown += EquipmentShop_MouseDown;
        }


        private void EquipmentShop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = equipmentShop.HitTest(e.X, e.Y);
                if (hitTestInfo.RowIndex >= 0)
                {
                    equipmentShop.ClearSelection(); // Clear any existing selections
                    equipmentShop.Rows[hitTestInfo.RowIndex].Selected = true; // Select the right-clicked row
                }
            }
        }

        private void LootMenuItem_Click(object sender, EventArgs e)
        {
            if (equipmentShop.SelectedRows.Count > 0)
            {
                var selectedItem = equipmentShop.SelectedRows[0].DataBoundItem as EquipmentItem;
                if (selectedItem != null)
                {
                    int quantity = PromptForQuantity(); // Prompt the user for a quantity

                    // Get the reference to Form1
                    var form1 = Application.OpenForms["Form1"] as Form1;
                    if (form1 == null)
                    {
                        MessageBox.Show("Form1 is not accessible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Call AddToInventory from Form1
                    form1.AddToInventory(selectedItem, quantity);
                }
            }
        }


        private void BuyMenuItem_Click(object sender, EventArgs e)
        {
            if (equipmentShop.SelectedRows.Count > 0)
            {
                var selectedItem = equipmentShop.SelectedRows[0].DataBoundItem as EquipmentItem;
                if (selectedItem != null)
                {
                    // Get reference to Form1
                    var form1 = Application.OpenForms["Form1"] as Form1;
                    if (form1 == null)
                    {
                        MessageBox.Show("Form1 is not accessible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Parse player credits from Form1
                    int playerCredits = int.TryParse(form1.playerCreditsTextBox.Text, out int credits) ? credits : 0;
                    int itemCost = int.TryParse(selectedItem.Cost, out int cost) ? cost : 0;
                    int quantity = PromptForQuantity();
                    int totalCost = itemCost * quantity;

                    if (totalCost > 0 && playerCredits >= totalCost)
                    {
                        // Deduct the cost and update credits
                        playerCredits -= totalCost;
                        form1.playerCreditsTextBox.Text = playerCredits.ToString();

                        // Add the item to the inventory on Form1
                        form1.AddToInventory(selectedItem, quantity);
                    }
                    else
                    {
                        MessageBox.Show("Not enough credits or item unavailable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }


        private void InspectMenuItem_Click(object sender, EventArgs e)
        {
            if (equipmentShop.SelectedRows.Count > 0)
            {
                var selectedRow = equipmentShop.SelectedRows[0];

                // Ensure the column "Name" exists
                if (equipmentShop.Columns.Contains("Name"))
                {
                    string selectedName = selectedRow.Cells["Name"].Value?.ToString();
                    if (!string.IsNullOrEmpty(selectedName))
                    {
                        ShowItemDetails(selectedName);
                    }
                    else
                    {
                        MessageBox.Show("No valid item name selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("The 'Name' column is missing in the DataGridView.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No row is selected in the DataGridView.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowItemDetails(string itemName)
        {
            string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            string[] jsonFiles = { "Equipment.json", "General_Goods.json", "Weapons.json" };

            foreach (string fileName in jsonFiles)
            {
                string filePath = Path.Combine(dataDirectory, fileName);

                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);

                    try
                    {
                        JObject rootObject = JObject.Parse(jsonContent);
                        JArray itemsArray = rootObject["items"] as JArray;

                        if (itemsArray != null)
                        {
                            var matchingItem = itemsArray.FirstOrDefault(item =>
                                item["name"]?.ToString().Equals(itemName, StringComparison.OrdinalIgnoreCase) == true);

                            if (matchingItem != null)
                            {
                                // Extract details
                                string name = matchingItem["name"]?.ToString() ?? "Unknown";
                                string type = matchingItem["type"]?.ToString() ?? "Unknown";
                                string availability = matchingItem["system"]?["availability"]?.ToString() ?? "N/A";
                                string cost = matchingItem["system"]?["cost"]?.ToString() ?? "No Price";
                                string description = matchingItem["system"]?["description"]?.ToString() ?? "No Description Available";

                                // Load image
                                string imgPath = matchingItem["img"]?.ToString();
                                Image itemImage = !string.IsNullOrEmpty(imgPath) && File.Exists(imgPath)
                                    ? Image.FromFile(imgPath)
                                    : Properties.Resources.DefaultImage;

                                // Show item details
                                using (var itemDetailForm = new ItemDetailForm())
                                {
                                    itemDetailForm.SetItemDetails(name, type, availability, cost, description, itemImage);
                                    itemDetailForm.ShowDialog();
                                }

                                return; // Exit the loop once found
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error parsing JSON file: {fileName}\n{ex.Message}", "JSON Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            MessageBox.Show("Item not found in the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }










        private int PromptForQuantity()
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter the quantity:", "Quantity", "1");
            return int.TryParse(input, out int quantity) && quantity > 0 ? quantity : 1;
        }


       
    }
}
